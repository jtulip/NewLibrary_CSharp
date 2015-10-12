﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Entities;
using Library.Interfaces.Entities;
using Xunit;

namespace Library.Tests.Integration.Entity
{
    [Trait("Category", "Loan Tests")]
    public class LoanIntegration
    {
        [Fact]
        public void CanCreateLoan()
        {
            var book = new Book("author", "title", "call number", 1);
            var member = new Member("first", "last", "phone", "email", 1);
            
            DateTime borrowDate = DateTime.Today;
            DateTime dueDate = DateTime.Today.AddDays(7);

            // Jim has stated that the initial spec is incorrect and it should not take an ID, but should return a default ID of 0.
            var loan = new Loan(book, member, borrowDate, dueDate);

            Assert.NotNull(loan);

            // By new rule, assert ID = 0.
            Assert.Equal(0, loan.ID);
        }


        [Fact]
        public void LoanCtorThrowsIllegalParameterException()
        {
            var book = new Book("author", "title", "call number", 1);
            var member = new Member("first", "last", "phone", "email", 1);

            DateTime borrowDate = DateTime.Today;
            DateTime dueDate = DateTime.Today.AddDays(7);

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
            var book = new Book("author", "title", "call number", 1);
            var member = new Member("first", "last", "phone", "email", 1);

            DateTime borrowDate = DateTime.Today;
            DateTime dueDate = DateTime.Today.AddDays(7);
            var loanId = 1;

            var loan = new Loan(book, member, borrowDate, dueDate);

            loan.Commit(loanId);
        }

        [Fact]
        public void CommitLoanSetsStateToCurrent()
        {
            var book = new Book("author", "title", "call number", 1);
            var member = new Member("first", "last", "phone", "email", 1);

            DateTime borrowDate = DateTime.Today;
            DateTime dueDate = DateTime.Today.AddDays(7);
            var loanId = 1;

            var loan = new Loan(book, member, borrowDate, dueDate);

            loan.Commit(loanId);

            Assert.Equal(LoanState.CURRENT, loan.State);
        }

        [Fact]
        public void CommitLoanCallsBookBorrow()
        {
            var book = new Book("author", "title", "call number", 1);
            var member = new Member("first", "last", "phone", "email", 1);

            DateTime borrowDate = DateTime.Today;
            DateTime dueDate = DateTime.Today.AddDays(7);
            var loanId = 1;

            var loan = new Loan(book, member, borrowDate, dueDate);

            // Call the commit and expect that it will call book.Borrow.
            loan.Commit(loanId);

            // When loan is committed, assert book.Borrow will be called with the loan.
            Assert.Equal(BookState.ON_LOAN, book.State);
        }

        [Fact]
        public void CommitLoanCallsBorrowerAddLoan()
        {
            var book = new Book("author", "title", "call number", 1);
            var member = new Member("first", "last", "phone", "email", 1);

            DateTime borrowDate = DateTime.Today;
            DateTime dueDate = DateTime.Today.AddDays(7);
            var loanId = 1;

            var loan = new Loan(book, member, borrowDate, dueDate);

            // Call the commit and expect that it will call book.Borrow.
            loan.Commit(loanId);

            // When loan is committed, assert borrower.AddLoan will be called with the loan.
            Assert.Equal(loan, member.Loans[0]);
        }

        [Fact]
        public void CommitLoanThrowsExceptionIfLoanStateNotPending()
        {
            var book = new Book("author", "title", "call number", 1);
            var member = new Member("first", "last", "phone", "email", 1);

            DateTime borrowDate = DateTime.Today;
            DateTime dueDate = DateTime.Today.AddDays(7);
            var loanId = 1;

            var loan = new Loan(book, member, borrowDate, dueDate);

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
            var book = new Book("author", "title", "call number", 1);
            var member = new Member("first", "last", "phone", "email", 1);

            DateTime borrowDate = DateTime.Today;
            DateTime dueDate = DateTime.Today.AddDays(7);
            var loanId = 1;

            var loan = new Loan(book, member, borrowDate, dueDate);

            loan.Commit(loanId);

            loan.Complete();

            Assert.Equal(LoanState.COMPLETE, loan.State);
        }

        [Theory]
        [InlineData(LoanState.PENDING)]
        public void CommitLoanThrowsRuntimeException(LoanState state)
        {
            var book = new Book("author", "title", "call number", 1);
            var member = new Member("first", "last", "phone", "email", 1);

            DateTime borrowDate = DateTime.Today;
            DateTime dueDate = DateTime.Today.AddDays(7);

            var loan = new Loan(book, member, borrowDate, dueDate);

            // Set the state to one passed in
            loan.State = state;

            var ex = Assert.Throws<InvalidOperationException>(() => loan.Complete());

            Assert.Equal("Cannot complete a loan if it's not Current or Overdue", ex.Message);
        }

        [Fact]
        public void IsOverdueShouldReturnTrueIfLoanIsOverdue()
        {
            var book = new Book("author", "title", "call number", 1);
            var member = new Member("first", "last", "phone", "email", 1);

            DateTime borrowDate = DateTime.Today;
            DateTime dueDate = DateTime.Today.AddDays(7);

            var loan = new Loan(book, member, borrowDate, dueDate);

            loan.State = LoanState.CURRENT;

            Assert.False(loan.IsOverDue);

            loan.State = LoanState.OVERDUE;

            Assert.True(loan.IsOverDue);
        }

        [Fact]
        public void CheckOverDueSetsLoanStateToOverdue()
        {
            var book = new Book("author", "title", "call number", 1);
            var member = new Member("first", "last", "phone", "email", 1);

            DateTime borrowDate = DateTime.Today.AddMonths(-1);

            // Set DueDate to the past so we can check it is < Today.
            var dueDate = DateTime.Today.AddMonths(-1).AddDays(1);

            var loan = new Loan(book, member, borrowDate, dueDate);

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
            var book = new Book("author", "title", "call number", 1);
            var member = new Member("first", "last", "phone", "email", 1);

            DateTime borrowDate = DateTime.Today;
            DateTime dueDate = DateTime.Today.AddDays(7);

            var loan = new Loan(book, member, borrowDate, dueDate);

            loan.State = state;

            var ex = Assert.Throws<InvalidOperationException>(() => loan.CheckOverDue(DateTime.Today));

            Assert.Equal("Cannot check Over Due if Loan is not Current or Overdue", ex.Message);
        }

        [Fact]
        public void CanGetBorrower()
        {
            var book = new Book("author", "title", "call number", 1);
            var member = new Member("first", "last", "phone", "email", 1);

            DateTime borrowDate = DateTime.Today;
            DateTime dueDate = DateTime.Today.AddDays(7);

            var loan = new Loan(book, member, borrowDate, dueDate);

            Assert.Equal(member, loan.Borrower);
        }

        [Fact]
        public void CanGetBook()
        {
            var book = new Book("author", "title", "call number", 1);
            var member = new Member("first", "last", "phone", "email", 1);

            DateTime borrowDate = DateTime.Today;
            DateTime dueDate = DateTime.Today.AddDays(7);

            var loan = new Loan(book, member, borrowDate, dueDate);

            Assert.Equal(book, loan.Book);
        }

        [Fact]
        public void CanGetID()
        {
            var book = new Book("author", "title", "call number", 1);
            var member = new Member("first", "last", "phone", "email", 1);

            DateTime borrowDate = DateTime.Today;
            DateTime dueDate = DateTime.Today.AddDays(7);

            var loan = new Loan(book, member, borrowDate, dueDate);

            Assert.Equal(0, loan.ID);
        }

        [Fact]
        public void WhenLoanIsCreatedShouldBePending()
        {
            var book = new Book("author", "title", "call number", 1);
            var member = new Member("first", "last", "phone", "email", 1);

            DateTime borrowDate = DateTime.Today;
            DateTime dueDate = DateTime.Today.AddDays(7);

            var loan = new Loan(book, member, borrowDate, dueDate);

            Assert.Equal(LoanState.PENDING, loan.State);
        }

        [Fact]
        public void WhenLoanIsPendingAndCommittedShouldBeCurrent()
        {
            var book = new Book("author", "title", "call number", 1);
            var member = new Member("first", "last", "phone", "email", 1);

            DateTime borrowDate = DateTime.Today;
            DateTime dueDate = DateTime.Today.AddDays(7);

            var loanId = 1;

            var loan = new Loan(book, member, borrowDate, dueDate);

            Assert.Equal(LoanState.PENDING, loan.State);

            loan.Commit(loanId);

            Assert.Equal(LoanState.CURRENT, loan.State);
        }

        [Fact]
        public void WhenLoanIsCurrentAndNotOverdueThenShouldStayCurrent()
        {
            var book = new Book("author", "title", "call number", 1);
            var member = new Member("first", "last", "phone", "email", 1);

            DateTime borrowDate = DateTime.Today;
            DateTime dueDate = DateTime.Today.AddDays(7);

            var loanId = 1;

            var loan = new Loan(book, member, borrowDate, dueDate);

            loan.Commit(loanId);

            Assert.Equal(LoanState.CURRENT, loan.State);

            loan.CheckOverDue(DateTime.Today);

            Assert.Equal(LoanState.CURRENT, loan.State);
        }

        [Fact]
        public void WhenLoanIsCurrentAndOverdueShouldBeOverdue()
        {
            var book = new Book("author", "title", "call number", 1);
            var member = new Member("first", "last", "phone", "email", 1);

            DateTime borrowDate = DateTime.Today;
            DateTime dueDate = DateTime.Today.AddDays(7);

            var loanId = 1;

            var loan = new Loan(book, member, borrowDate, dueDate);

            loan.Commit(loanId);

            Assert.Equal(LoanState.CURRENT, loan.State);

            loan.CheckOverDue(DateTime.Today.AddDays(14));

            Assert.Equal(LoanState.OVERDUE, loan.State);
        }

        [Fact]
        public void WhenLoanIsOverdueAndCheckedShouldStayOverdue()
        {
            var book = new Book("author", "title", "call number", 1);
            var member = new Member("first", "last", "phone", "email", 1);

            DateTime borrowDate = DateTime.Today;
            DateTime dueDate = DateTime.Today.AddDays(7);

            var loanId = 1;

            var loan = new Loan(book, member, borrowDate, dueDate);

            loan.Commit(loanId);

            loan.CheckOverDue(DateTime.Today.AddDays(14));

            Assert.Equal(LoanState.OVERDUE, loan.State);

            loan.CheckOverDue(DateTime.Today.AddDays(21));

            Assert.Equal(LoanState.OVERDUE, loan.State);
        }

        [Fact]
        public void WhenLoanIsCurrentAndCompletedShouldBeComplete()
        {
            var book = new Book("author", "title", "call number", 1);
            var member = new Member("first", "last", "phone", "email", 1);

            DateTime borrowDate = DateTime.Today;
            DateTime dueDate = DateTime.Today.AddDays(7);

            var loanId = 1;

            var loan = new Loan(book, member, borrowDate, dueDate);

            loan.Commit(loanId);

            Assert.Equal(LoanState.CURRENT, loan.State);

            loan.Complete();

            Assert.Equal(LoanState.COMPLETE, loan.State);
        }

        [Fact]
        public void WhenLoanIsOverdueAndCompletedShouldBeComplete()
        {
            var book = new Book("author", "title", "call number", 1);
            var member = new Member("first", "last", "phone", "email", 1);

            DateTime borrowDate = DateTime.Today;
            DateTime dueDate = DateTime.Today.AddDays(7);

            var loanId = 1;

            var loan = new Loan(book, member, borrowDate, dueDate);

            loan.Commit(loanId);

            Assert.Equal(LoanState.CURRENT, loan.State);

            loan.CheckOverDue(DateTime.Today.AddDays(14));

            Assert.Equal(LoanState.OVERDUE, loan.State);

            loan.Complete();

            Assert.Equal(LoanState.COMPLETE, loan.State);
        }

        [Fact]
        public void ReturnLoanReadable()
        {
            var book = new Book("Jim Tulip", "Adventures in Programming", "call number", 1);
            var member = new Member("Jim", "Tulip", "Phone", "Email", 1);

            var loan = new Loan(book, member, DateTime.Today, DateTime.Today.AddDays(1));

            var readable =
                $"Loan ID:\t\t{loan.ID}\nAuthor:\t\t{loan.Book.Author}\nTitle:\t\t{loan.Book.Title}\nBorrower:\t{loan.Borrower.ToString()}\nBorrow Date:\t{loan.BorrowDate.ToShortDateString()}\nDue Date:\t{loan.DueDate.ToShortDateString()}";
            Assert.Equal(readable, loan.ToString());
        }
    }
}
