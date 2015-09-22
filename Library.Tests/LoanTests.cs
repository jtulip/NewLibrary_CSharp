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
    }
}
