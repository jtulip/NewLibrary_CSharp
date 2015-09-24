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

        [Fact]
        public void CreateLoanDaoThrowsExceptionOnNullParameter()
        {
            var ex = Assert.Throws<ArgumentException>(() => { var loanDao = new LoanDao(null); });

            Assert.Equal("Helper must be provided when creating LoanDao", ex.Message);
        }
    }
}
