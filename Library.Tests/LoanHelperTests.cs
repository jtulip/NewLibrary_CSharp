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
        public void CanMakeLoan()
        {
            var helper = new LoanHelper();

            Assert.NotNull(helper);
        }
    }
}
