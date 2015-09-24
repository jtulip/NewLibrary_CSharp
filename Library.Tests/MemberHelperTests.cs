using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Entities;
using Xunit;

namespace Library.Tests
{
    [Trait("Category", "Member Tests")]
    public class MemberHelperTests
    {
        [Fact]
        public void CanCreateMemberHelper()
        {
            var helper = new MemberHelper();

            Assert.NotNull(helper);
        }

    }
}
