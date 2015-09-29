using System;
using Library.Entities;
using Library.Interfaces.Entities;
using NSubstitute;
using Xunit;

namespace Library.Tests.UnitTests.Entity
{
    [Trait("Category", "Loan Tests")]
    public class LoanStateTests
    {
        [Fact]
        public void WhenLoanIsCreatedShouldBePending()
        {
            var book = Substitute.For<IBook>();
            var member = Substitute.For<IMember>();
            DateTime borrowDate = DateTime.Today;
            DateTime dueDate = DateTime.Today;

            var loan = new Loan(book, member, borrowDate, dueDate);

            Assert.Equal(LoanState.PENDING, loan.State);
        }

        [Fact]
        public void WhenLoanIsPendingAndCommittedShouldBeCurrent()
        {
            var book = Substitute.For<IBook>();
            var member = Substitute.For<IMember>();
            DateTime borrowDate = DateTime.Today;
            DateTime dueDate = DateTime.Today;

            var loan = new Loan(book, member, borrowDate, dueDate);

            Assert.Equal(LoanState.PENDING, loan.State);

            loan.Commit(1);

            Assert.Equal(LoanState.CURRENT, loan.State);
        }

        [Fact]
        public void WhenLoanIsCurrentAndNotOverdueThenShouldStayCurrent()
        {
            var book = Substitute.For<IBook>();
            var member = Substitute.For<IMember>();
            DateTime borrowDate = DateTime.Today;
            DateTime dueDate = DateTime.Today.AddDays(7);

            var loan = new Loan(book, member, borrowDate, dueDate);

            loan.Commit(1);

            Assert.Equal(LoanState.CURRENT, loan.State);

            loan.CheckOverDue(DateTime.Today);

            Assert.Equal(LoanState.CURRENT, loan.State);
        }

        [Fact]
        public void WhenLoanIsCurrentAndOverdueShouldBeOverdue()
        {
            var book = Substitute.For<IBook>();
            var member = Substitute.For<IMember>();
            DateTime borrowDate = DateTime.Today;
            DateTime dueDate = DateTime.Today.AddDays(7);

            var loan = new Loan(book, member, borrowDate, dueDate);

            loan.Commit(1);

            Assert.Equal(LoanState.CURRENT, loan.State);

            loan.CheckOverDue(DateTime.Today.AddDays(14));

            Assert.Equal(LoanState.OVERDUE, loan.State);
        }

        [Fact]
        public void WhenLoanIsOverdueAndCheckedShouldStayOverdue()
        {
            var book = Substitute.For<IBook>();
            var member = Substitute.For<IMember>();
            DateTime borrowDate = DateTime.Today;
            DateTime dueDate = DateTime.Today.AddDays(7);

            var loan = new Loan(book, member, borrowDate, dueDate);

            loan.Commit(1);

            loan.CheckOverDue(DateTime.Today.AddDays(14));

            Assert.Equal(LoanState.OVERDUE, loan.State);

            loan.CheckOverDue(DateTime.Today.AddDays(21));

            Assert.Equal(LoanState.OVERDUE, loan.State);
        }

        [Fact]
        public void WhenLoanIsCurrentAndCompletedShouldBeComplete()
        {
            var book = Substitute.For<IBook>();
            var member = Substitute.For<IMember>();
            DateTime borrowDate = DateTime.Today;
            DateTime dueDate = DateTime.Today.AddDays(7);

            var loan = new Loan(book, member, borrowDate, dueDate);

            loan.Commit(1);

            Assert.Equal(LoanState.CURRENT, loan.State);

            loan.Complete();

            Assert.Equal(LoanState.COMPLETE, loan.State);
        }

        [Fact]
        public void WhenLoanIsOverdueAndCompletedShouldBeComplete()
        {
            var book = Substitute.For<IBook>();
            var member = Substitute.For<IMember>();
            DateTime borrowDate = DateTime.Today;
            DateTime dueDate = DateTime.Today.AddDays(7);

            var loan = new Loan(book, member, borrowDate, dueDate);

            loan.Commit(1);

            Assert.Equal(LoanState.CURRENT, loan.State);

            loan.CheckOverDue(DateTime.Today.AddDays(14));

            Assert.Equal(LoanState.OVERDUE, loan.State);

            loan.Complete();

            Assert.Equal(LoanState.COMPLETE, loan.State);
        }
    }
}
