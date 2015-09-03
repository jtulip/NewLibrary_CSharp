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
            var member = new Member();

            Assert.NotNull(member);
        }

        [Fact]
        public void MemberImplementsIMemberInterface()
        {
            var member = new Member() as IMember;

            Assert.NotNull(member);
        }

    }
}
