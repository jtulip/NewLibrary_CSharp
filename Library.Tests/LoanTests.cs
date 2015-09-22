using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Entities;
using Library.Interfaces.Entities;
using NSubstitute;
using Xunit;

namespace Library.Tests
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
            int loanID = 1;

            var loan = new Loan(book, member, borrowDate, dueDate, loanID);

            Assert.NotNull(loan);
        }

        [Fact]
        public void LoanCtorThrowsIllegalParameterException()
        {
            var book = Substitute.For<IBook>();
            var member = Substitute.For<IMember>();
            DateTime borrowDate = DateTime.Today;
            DateTime dueDate = DateTime.Today;
            int loanID = 1;

            var ex = Assert.Throws<ArgumentException>(() =>
            {
                var loan = new Loan(null, member, borrowDate, dueDate, loanID);
            });

            Assert.Equal("Book needs to be provided", ex.Message);

            ex = Assert.Throws<ArgumentException>(() =>
            {
                new Loan(book, null, borrowDate, dueDate, loanID);
            });

            Assert.Equal("Member needs to be provided", ex.Message);

            ex = Assert.Throws<ArgumentException>(() =>
            {
                // DateTime can't be null in .NET
                new Loan(book, member, DateTime.MinValue, dueDate, loanID);
            });

            Assert.Equal("Borrow date needs to be provided", ex.Message);

            ex = Assert.Throws<ArgumentException>(() =>
            {
                // DateTime can't be null in .NET
                new Loan(book, member, borrowDate, DateTime.MinValue, loanID);
            });

            Assert.Equal("Due date needs to be provided", ex.Message);

            ex = Assert.Throws<ArgumentException>(() =>
            {
                // DateTime can't be null in .NET
                new Loan(book, member, borrowDate, borrowDate.AddDays(-1), loanID);
            });

            Assert.Equal("Due date cannot be before Borrow date", ex.Message);

            ex = Assert.Throws<ArgumentException>(() =>
            {
                // DateTime can't be null in .NET
                new Loan(book, member, borrowDate, dueDate, 0);
            });

            Assert.Equal("ID must be greater than 0", ex.Message);
        }

        [Fact]
        public void CanCommitLoan()
        {
            var book = Substitute.For<IBook>();
            var member = Substitute.For<IMember>();
            var loanId = 1;

            var loan = new Loan(book, member, DateTime.Today, DateTime.Today.AddDays(1), loanId);

            loan.Commit(loanId);
        }

        [Fact]
        public void CommitLoanSetsStateToCurrent()
        {
            var book = Substitute.For<IBook>();
            var member = Substitute.For<IMember>();
            var loanId = 1;

            var loan = new Loan(book, member, DateTime.Today, DateTime.Today.AddDays(1), loanId);

            loan.Commit(loanId);

            Assert.Equal(LoanState.CURRENT, loan.State);
        }
    }
}
