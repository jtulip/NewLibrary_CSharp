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
    public class LoanHelperTests
    {
        [Fact]
        public void CanCreateLoanHelper()
        {
            var helper = new LoanHelper();

            Assert.NotNull(helper);
        }

        [Fact]
        public void LoanHelperImplementsILoanHelperInterface()
        {
            var helper = new LoanHelper();

            Assert.IsAssignableFrom<ILoanHelper>(helper);

            var typedMember = helper as ILoanHelper;

            Assert.NotNull(typedMember);
        }

        [Fact]
        public void CanMakeLoan()
        {
            var book = Substitute.For<IBook>();
            var borrower = Substitute.For<IMember>();
            var borrowDate = DateTime.Today;
            var dueDate = DateTime.Today.AddDays(7);

            var helper = new LoanHelper();

            var loan = helper.MakeLoan(book, borrower, borrowDate, dueDate);

            Assert.NotNull(loan);
            Assert.Equal(0, loan.ID);
            Assert.Equal(book, loan.Book);
            Assert.Equal(borrower, loan.Borrower);
        }
    }
}
