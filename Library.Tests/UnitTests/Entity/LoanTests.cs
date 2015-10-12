﻿using System;
using System.Configuration;
using Library.Entities;
using Library.Interfaces.Entities;
using NSubstitute;
using Xunit;

namespace Library.Tests.UnitTests.Entity
{
    [Trait("Category", "Loan Tests")]
    public class LoanTests
    {
        [Fact]
        public void CanCreateLoan()
        {
            var book = Substitute.For<IBook>();
            var member = Substitute.For<IMember>();
            DateTime borrowDate = DateTime.Today;
            DateTime dueDate = DateTime.Today;

            // Jim has stated that the initial spec is incorrect and it should not take an ID, but should return a default ID of 0.
            var loan = new Loan(book, member, borrowDate, dueDate);

            Assert.NotNull(loan);

            // By new rule, assert ID = 0.
            Assert.Equal(0, loan.ID);
        }

        [Fact]
        public void LoanCtorThrowsIllegalParameterException()
        {
            var book = Substitute.For<IBook>();
            var member = Substitute.For<IMember>();
            DateTime borrowDate = DateTime.Today;
            DateTime dueDate = DateTime.Today;

            var ex = Assert.Throws<ArgumentException>(() =>
            {
                var loan = new Loan(null, member, borrowDate, dueDate);
            });

            Assert.Equal("Book needs to be provided", ex.Message);

            ex = Assert.Throws<ArgumentException>(() =>
            {
                new Loan(book, null, borrowDate, dueDate);
            });

            Assert.Equal("Member needs to be provided", ex.Message);

            ex = Assert.Throws<ArgumentException>(() =>
            {
                // DateTime can't be null in .NET
                new Loan(book, member, DateTime.MinValue, dueDate);
            });

            Assert.Equal("Borrow date needs to be provided", ex.Message);

            ex = Assert.Throws<ArgumentException>(() =>
            {
                // DateTime can't be null in .NET
                new Loan(book, member, borrowDate, DateTime.MinValue);
            });

            Assert.Equal("Due date needs to be provided", ex.Message);

            ex = Assert.Throws<ArgumentException>(() =>
            {
                // DateTime can't be null in .NET
                new Loan(book, member, borrowDate, borrowDate.AddDays(-1));
            });

            Assert.Equal("Due date cannot be before Borrow date", ex.Message);
        }

        [Fact]
        public void CanCommitLoan()
        {
            var book = Substitute.For<IBook>();
            var member = Substitute.For<IMember>();
            var loanId = 1;

            var loan = new Loan(book, member, DateTime.Today, DateTime.Today.AddDays(1));

            loan.Commit(loanId);
        }

        [Fact]
        public void CommitLoanSetsStateToCurrent()
        {
            var book = Substitute.For<IBook>();
            var member = Substitute.For<IMember>();
            var loanId = 1;

            var loan = new Loan(book, member, DateTime.Today, DateTime.Today.AddDays(1));

            loan.Commit(loanId);

            Assert.Equal(LoanState.CURRENT, loan.State);
        }

        [Fact]
        public void CommitLoanCallsBookBorrow()
        {
            var book = Substitute.For<IBook>();
            var member = Substitute.For<IMember>();
            var loanId = 1;

            var loan = new Loan(book, member, DateTime.Today, DateTime.Today.AddDays(1));

            // Call the commit and expect that it will call book.Borrow.
            loan.Commit(loanId);

            // When loan is committed, assert book.Borrow will be called with the loan.
            book.Received().Borrow(loan);
        }

        [Fact]
        public void CommitLoanCallsBorrowerAddLoan()
        {
            var book = Substitute.For<IBook>();
            var borrower = Substitute.For<IMember>();
            var loanId = 1;

            var loan = new Loan(book, borrower, DateTime.Today, DateTime.Today.AddDays(1));

            // Call the commit and expect that it will call book.Borrow.
            loan.Commit(loanId);

            // When loan is committed, assert borrower.AddLoan will be called with the loan.
            borrower.Received().AddLoan(loan);
        }

        [Fact]
        public void CommitLoanThrowsExceptionIfLoanStateNotPending()
        {
            var book = Substitute.For<IBook>();
            var borrower = Substitute.For<IMember>();
            var loanId = 1;

            var loan = new Loan(book, borrower, DateTime.Today, DateTime.Today.AddDays(1));

            // Set loan state to pending
            loan.State = LoanState.PENDING;

            // Call the commit and expect that it will call book.Borrow.
            loan.Commit(loanId);

            // Just make a general check it was successful and changed state to current.
            Assert.Equal(LoanState.CURRENT, loan.State);

            // Set loan state to other than pending
            loan.State = LoanState.COMPLETE;

            var ex = Assert.Throws<InvalidOperationException>(() => loan.Commit(loanId));

            Assert.Equal("Loan cannot be committed unless state is Pending", ex.Message);
        }

        [Fact]
        public void CanCompleteLoan()
        {
            var book = Substitute.For<IBook>();
            var borrower = Substitute.For<IMember>();

            var loan = new Loan(book, borrower, DateTime.Today, DateTime.Today.AddDays(1));

            loan.Commit(1);

            loan.Complete();

            Assert.Equal(LoanState.COMPLETE, loan.State);
        }

        [Fact]
        public void CommitLoanSetsStateToComplete()
        {
            var book = Substitute.For<IBook>();
            var borrower = Substitute.For<IMember>();

            var loan = new Loan(book, borrower, DateTime.Today, DateTime.Today.AddDays(1));

            loan.Commit(1);

            loan.Complete();

            Assert.Equal(LoanState.COMPLETE, loan.State);
        }

        [Theory]
        [InlineData(LoanState.PENDING)]
        public void CommitLoanThrowsRuntimeException(LoanState state)
        {
            var book = Substitute.For<IBook>();
            var borrower = Substitute.For<IMember>();

            var loan = new Loan(book, borrower, DateTime.Today, DateTime.Today.AddDays(1));

            // Set the state to one passed in
            loan.State = state;

            var ex = Assert.Throws<InvalidOperationException>(() => loan.Complete());

            Assert.Equal("Cannot complete a loan if it's not Current or Overdue", ex.Message);
        }

        [Fact]
        public void IsOverdueShouldReturnTrueIfLoanIsOverdue()
        {
            var book = Substitute.For<IBook>();
            var borrower = Substitute.For<IMember>();

            var loan = new Loan(book, borrower, DateTime.Today, DateTime.Today.AddDays(1));

            loan.State = LoanState.CURRENT;

            Assert.False(loan.IsOverDue);

            loan.State = LoanState.OVERDUE;

            Assert.True(loan.IsOverDue);
        }

        [Fact]
        public void CheckOverDueSetsLoanStateToOverdue()
        {
            var book = Substitute.For<IBook>();
            var borrower = Substitute.For<IMember>();

            // Set DueDate to the past so we can check it is < Today.
            var dueDate = DateTime.Today.AddMonths(-1).AddDays(1);

            var loan = new Loan(book, borrower, DateTime.Today.AddMonths(-1), dueDate);

            // Set loan state to current so it clears next test (Must be Current or Overdue).
            loan.State = LoanState.CURRENT;

            Assert.True(dueDate < DateTime.Today);

            // Check if it is overdue as of Today.
            var result = loan.CheckOverDue(DateTime.Today);

            // Make sure the check set the loan state to overdue.
            Assert.Equal(LoanState.OVERDUE, loan.State);
            Assert.True(result);
        }

        [Theory]
        [InlineData(LoanState.PENDING)]
        [InlineData(LoanState.COMPLETE)]
        public void CheckOverDueThrowsExceptionIfNotCurrentOrOverdue(LoanState state)
        {
            var book = Substitute.For<IBook>();
            var borrower = Substitute.For<IMember>();

            var loan = new Loan(book, borrower, DateTime.Today, DateTime.Today.AddDays(1));

            loan.State = state;

            var ex = Assert.Throws<InvalidOperationException>(() => loan.CheckOverDue(DateTime.Today));

            Assert.Equal("Cannot check Over Due if Loan is not Current or Overdue", ex.Message);
        }

        [Fact]
        public void CanGetBorrower()
        {
            var book = Substitute.For<IBook>();
            var borrower = Substitute.For<IMember>();

            var loan = new Loan(book, borrower, DateTime.Today, DateTime.Today.AddDays(1));

            Assert.Equal(borrower, loan.Borrower);
        }

        [Fact]
        public void CanGetBook()
        {
            var book = Substitute.For<IBook>();
            var borrower = Substitute.For<IMember>();

            var loan = new Loan(book, borrower, DateTime.Today, DateTime.Today.AddDays(1));

            Assert.Equal(book, loan.Book);
        }

        [Fact]
        public void CanGetID()
        {
            var book = Substitute.For<IBook>();
            var borrower = Substitute.For<IMember>();

            var loan = new Loan(book, borrower, DateTime.Today, DateTime.Today.AddDays(1));

            Assert.Equal(0, loan.ID);
        }

        [Fact]
        public void ReturnLoanReadable()
        {
            var book = Substitute.For<Book>("Jim Tulip", "Adventures in Programming", "call number", 1);
            var member = Substitute.For<Member>("Jim", "Tulip", "Phone", "Email", 1);

            var loan = new Loan(book, member, DateTime.Today, DateTime.Today.AddDays(1));

            var readable =
                $"Loan ID:\t\t{loan.ID}\nAuthor:\t\t{loan.Book.Author}\nTitle:\t\t{loan.Book.Title}\nBorrower:\t{loan.Borrower.ToString()}\nBorrow Date:\t{loan.BorrowDate.ToShortDateString()}\nDue Date:\t{loan.DueDate.ToShortDateString()}";
            Assert.Equal(readable, loan.ToString());
        }
    }
}
