﻿using System;
using System.Collections.Generic;
using System.Linq;
using Library.Daos;
using Library.Entities;
using Library.Interfaces.Daos;
using Library.Interfaces.Entities;
using NSubstitute;
using Xunit;

namespace Library.Tests.UnitTests.Dao
{
    [Trait("Category", "Loan Tests")]
    public class LoanDaoTests
    {
        [Fact]
        public void CanCreateLoanDao()
        {
            var helper = Substitute.For<ILoanHelper>();

            var loanDao = new LoanDao(helper);

            Assert.NotNull(loanDao);
        }

        [Fact]
        public void CreateLoanDaoThrowsExceptionOnNullParameter()
        {
            var ex = Assert.Throws<ArgumentException>(() => { var loanDao = new LoanDao(null); });

            Assert.Equal("Helper must be provided when creating LoanDao", ex.Message);
        }

        [Fact]
        public void LoanDaoImplementsILoanDAOInterface()
        {
            var helper = Substitute.For<ILoanHelper>();

            var loanDao = new LoanDao(helper);

            Assert.IsAssignableFrom<ILoanDAO>(loanDao);

            var typedMember = loanDao as ILoanDAO;

            Assert.NotNull(typedMember);
        }

        [Fact]
        public void CanCreateLoan()
        {
            var helper = Substitute.For<ILoanHelper>();
            var book = Substitute.For<IBook>();
            var member = Substitute.For<IMember>();

            var loanDao = new LoanDao(helper);

            var borrowDate = DateTime.Today;
            var dueDate = DateTime.Today.AddDays(7);

            // Adds the member to a collection of members and returns new member.
            Assert.Equal(0, loanDao.LoanList.Count);

            // Tell the mock what to return when it is called.
            helper.MakeLoan(book, member, borrowDate, dueDate).Returns(new Loan(book, member, borrowDate, dueDate));

            var result = loanDao.CreateLoan(member, book, borrowDate, dueDate);

            // Assert that the mock's MakeLoan method was called.
            helper.Received().MakeLoan(book, member, borrowDate, dueDate);

            Assert.NotNull(result);
            Assert.Equal(book, result.Book);
            Assert.Equal(member, result.Borrower);

            // We don't want to store until we commit the loan.
        }

        [Fact]
        public void CreateLoanThrowsNullWhenBookOrMemberIsNull()
        {
            var helper = Substitute.For<ILoanHelper>();
            var book = Substitute.For<IBook>();
            var member = Substitute.For<IMember>();

            var loanDao = new LoanDao(helper);

            var borrowDate = DateTime.Today;
            var dueDate = DateTime.Today.AddDays(7);

            var ex1 = Assert.Throws<ArgumentException>(() =>
            {
                var result = loanDao.CreateLoan(null, book, borrowDate, dueDate);
            });

            Assert.Equal("A Member must be provided to create a loan", ex1.Message);

            var ex2 = Assert.Throws<ArgumentException>(() =>
            {
                var result = loanDao.CreateLoan(member, null, borrowDate, dueDate);
            });

            Assert.Equal("A Book must be provided to create a loan", ex2.Message);
        }

        [Fact]
        public void CanCommitLoan()
        {
            var helper = Substitute.For<ILoanHelper>();
            var book = Substitute.For<IBook>();
            var member = Substitute.For<IMember>();

            var loanDao = new LoanDao(helper);

            var borrowDate = DateTime.Today;
            var dueDate = DateTime.Today.AddDays(7);

            // Adds the member to a collection of members and returns new member.
            Assert.Equal(0, loanDao.LoanList.Count);

            // Tell the mock what to return when it is called.
            helper.MakeLoan(book, member, borrowDate, dueDate).Returns(Substitute.For<Loan>(book, member, borrowDate, dueDate));

            var loan = loanDao.CreateLoan(member, book, borrowDate, dueDate);

            // Assert that the mock's MakeLoan method was called.
            helper.Received().MakeLoan(book, member, borrowDate, dueDate);

            loanDao.CommitLoan(loan);

            Assert.NotEqual(0, loan.ID);

            Assert.Equal(1, loanDao.LoanList.Count);

            var stored = loanDao.LoanList[0];

            Assert.Equal(loan, stored);
        }

        [Fact]
        public void CanGetLoanById()
        {
            var helper = Substitute.For<ILoanHelper>();

            var loanDao = new LoanDao(helper);

            var book = Substitute.For<IBook>();
            var member = Substitute.For<IMember>();

            var borrowDate = DateTime.Today;
            var dueDate = DateTime.Today.AddDays(7);

            helper.MakeLoan(book, member, borrowDate, dueDate)
                .Returns(Substitute.For<Loan>(book, member, borrowDate, dueDate));

            // Commit one we can test.
            var loan = loanDao.CreateLoan(member, book, borrowDate, dueDate);
            loanDao.CommitLoan(loan);

            helper.Received().MakeLoan(book, member, borrowDate, dueDate);

            var max = loanDao.LoanList.Max(l => l.ID);

            var result = loanDao.GetLoanByID(max);

            Assert.NotNull(loan);

            Assert.Equal(loan.ID, result.ID);
            Assert.Equal(loan, result);
        }

        [Fact]
        public void GetMemberByIdReturnsNullIfNotFound()
        {
            var helper = Substitute.For<ILoanHelper>();

            var loanDao = new LoanDao(helper);

            loanDao.LoanList = new List<ILoan>
            {
                Substitute.For<ILoan>(),
                Substitute.For<ILoan>(),
            };

            var loan = loanDao.GetLoanByID(2);

            Assert.Null(loan);
        }

        [Fact]
        public void CanGetLoansByBookTitle()
        {
            var helper = Substitute.For<ILoanHelper>();

            var loanDao = new LoanDao(helper);

            var book = Substitute.For <Book>("author", "testing", "call", 1);
            var member = Substitute.For<IMember>();

            var borrowDate = DateTime.Today;
            var dueDate = DateTime.Today.AddDays(7);

            var storedLoan = Substitute.For<Loan>(book, member, borrowDate, dueDate);

            loanDao.LoanList = new List<ILoan>
            {
                Substitute.For<Loan>(Substitute.For<Book>("author", "title", "call", 1), Substitute.For<IMember>(), borrowDate, dueDate),
                Substitute.For<Loan>(Substitute.For<Book>("author", "title", "call", 1), Substitute.For<IMember>(), borrowDate, dueDate),
                storedLoan
            };

            var loans = loanDao.FindLoansByBookTitle("testing");

            Assert.NotNull(loans);
            Assert.Equal(storedLoan, loans[0]);
        }

        [Fact]
        public void CanGetLoansByBorrower()
        {
            var helper = Substitute.For<ILoanHelper>();

            var loanDao = new LoanDao(helper);

            var borrowDate = DateTime.Today;
            var dueDate = DateTime.Today.AddDays(7);

            var borrower = Substitute.For<Member>("Borris", "Natasha", "phone", "email", 1);

            var storedLoan = new Loan(Substitute.For<IBook>(), borrower, borrowDate, dueDate);

            loanDao.LoanList = new List<ILoan>
            {
                Substitute.For<ILoan>(),
                Substitute.For<ILoan>(),
                storedLoan
            };

            var loans = loanDao.FindLoansByBorrower(borrower);

            Assert.NotNull(loans);
            Assert.Equal(storedLoan, loans[0]);
        }

        [Fact]
        public void CanUpdateOverdueStatus()
        {
            var helper = Substitute.For<ILoanHelper>();
            var book = Substitute.For<IBook>();
            var member = Substitute.For<IMember>();

            var loanDao = new LoanDao(helper);

            var borrowDate = DateTime.Today.AddMonths(-1);
            var dueDate = DateTime.Today.AddMonths(-1).AddDays(7);

            // Adds the member to a collection of members and returns new member.
            Assert.Equal(0, loanDao.LoanList.Count);

            // Tell the mock what to return when it is called.
            helper.MakeLoan(book, member, borrowDate, dueDate).Returns(Substitute.For<Loan>(book, member, borrowDate, dueDate));

            var loan = loanDao.CreateLoan(member, book, borrowDate, dueDate);

            // Assert that the mock's MakeLoan method was called.
            helper.Received().MakeLoan(book, member, borrowDate, dueDate);

            loanDao.CommitLoan(loan);

            // Store two more for iteration
            loanDao.CommitLoan(Substitute.For<ILoan>());
            loanDao.CommitLoan(Substitute.For<ILoan>());

            loanDao.UpdateOverDueStatus(DateTime.Today);

            Assert.Contains(loan, loanDao.LoanList);

            Assert.Equal(LoanState.OVERDUE, loan.State);
        }

        [Fact]
        public void CanGetOverdueLoans()
        {
            var helper = Substitute.For<ILoanHelper>();

            var book = Substitute.For<IBook>();
            var member = Substitute.For<IMember>();

            var loanDao = new LoanDao(helper);

            var borrowDate = DateTime.Today.AddMonths(-1);
            var dueDate = DateTime.Today.AddMonths(-1).AddDays(7);

            // Adds the member to a collection of members and returns new member.
            Assert.Equal(0, loanDao.LoanList.Count);

            // Tell the mock what to return when it is called.
            helper.MakeLoan(book, member, borrowDate, dueDate).Returns(Substitute.For<Loan>(book, member, borrowDate, dueDate));

            var loan = loanDao.CreateLoan(member, book, borrowDate, dueDate);
            loanDao.CommitLoan(loan);

            // Assert that the mock's MakeLoan method was called.
            helper.Received().MakeLoan(book, member, borrowDate, dueDate);


            // Store two more for testing
            loanDao.CommitLoan(Substitute.For<ILoan>());
            loanDao.CommitLoan(Substitute.For<ILoan>());

            loanDao.UpdateOverDueStatus(DateTime.Today);

            var loans = loanDao.FindOverDueLoans();

            Assert.NotNull(loans);
            Assert.Equal(1, loans.Count);
            Assert.Equal(loan, loans[0]);
        }
    }
}
