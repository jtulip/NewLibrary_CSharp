using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Daos;
using Library.Entities;
using Library.Interfaces.Daos;
using Library.Interfaces.Entities;
using NSubstitute;
using Xunit;

namespace Library.Tests
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
    }
}
