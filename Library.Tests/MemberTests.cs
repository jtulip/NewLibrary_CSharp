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
    public class MemberTests
    {
        [Fact]
        public void CanCreateANewMember()
        {
            var member = new Member("firstName", "lastName", "contactPhone", "emailAddress", 1);

            Assert.NotNull(member);
        }

        [Fact]
        public void MemberImplementsIMemberInterface()
        {
            var member = new Member("firstName", "lastName", "contactPhone", "emailAddress", 1) as IMember;

            Assert.NotNull(member);
        }

    }
}
