using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Entities;
using Xunit;

namespace Library.Tests
{
    [Trait("Category", "Loan Tests")]
    public class LoanTests
    {
        [Fact]
        public void CanCreateLoan()
        {
            var loan = new Loan();

            Assert.NotNull(loan);
        }
    }
}
