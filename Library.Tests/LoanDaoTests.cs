using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Daos;
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
    }
}
