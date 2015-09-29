using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Entities;
using Library.Interfaces.Entities;
using Xunit;

namespace Library.Tests.UnitTests
{
    [Trait("Category", "Member Tests")]
    public class MemberStateTests
    {
        [Fact]
        public void WhenMemberCreatedShouldBeBorrowingAllowed()
        {
            var member = new Member("firstName", "lastName", "contactPhone", "emailAddress", 1);

            Assert.Equal(MemberState.BORROWING_ALLOWED, member.State);
        }
    }
}
