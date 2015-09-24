using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Entities;
using Library.Interfaces.Entities;
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

        [Fact]
        public void MemberHelperImplementsIMemberHelperInterface()
        {
            var helper = new MemberHelper();

            Assert.IsAssignableFrom<IMemberHelper>(helper);

            var typedMember = helper as IMemberHelper;

            Assert.NotNull(typedMember);
        }
    }
}
