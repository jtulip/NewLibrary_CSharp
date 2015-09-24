using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Daos;
using Library.Entities;
using Library.Interfaces.Daos;
using Library.Interfaces.Entities;
using NSubstitute;
using Xunit;

namespace Library.Tests
{
    [Trait("Category", "Member Tests")]
    public class MemberDaoTests
    {
        [Fact]
        public void CanCreateMemberDao()
        {
            var helper = Substitute.For<IMemberHelper>();

            var memberDao = new MemberDao(helper);

            Assert.NotNull(memberDao);
        }

        [Fact]
        public void CreateMemberDaoThrowsExceptionOnNullParameter()
        {
            var ex = Assert.Throws<ArgumentException>(() => { var memberDao = new MemberDao(null); });

            Assert.Equal("Helper must be provided when creating MemberDao", ex.Message);
        }

        [Fact]
        public void MemberDaoImplementsIMemberDAOInterface()
        {
            var helper = Substitute.For<IMemberHelper>();

            var memberDao = new MemberDao(helper);

            Assert.IsAssignableFrom<IMemberDAO>(memberDao);

            var typedMember = memberDao as IMemberDAO;

            Assert.NotNull(typedMember);
        }

        [Fact]
        public void CanAddMember()
        {
            var helper = Substitute.For<IMemberHelper>();

            var memberDao = new MemberDao(helper);

            var firstName = "first";
            var lastName = "last";
            var contactPhone = "phone";
            var emailAddress = "email address";

            // Uses Helper to create a new member with a unique member ID.
            // Adds the member to a collection of members and returns new member.
            Assert.Equal(0, memberDao.MemberList.Count);

            // Tell the mock what to return when it is called.
            helper.MakeMember(firstName, lastName, contactPhone, emailAddress, Arg.Any<int>()).Returns(new Member(firstName, lastName, contactPhone, emailAddress, 1));

            var result = memberDao.AddMember(firstName, lastName, contactPhone, emailAddress);

            // Assert that the mock's MakeMember method was called.
            helper.Received().MakeMember(firstName, lastName, contactPhone, emailAddress, Arg.Any<int>());

            Assert.NotNull(result);
            Assert.Equal(firstName, result.FirstName);
            Assert.Equal(lastName, result.LastName);
            Assert.Equal(contactPhone, result.ContactPhone);
            Assert.Equal(emailAddress, result.EmailAddress);

            Assert.Equal(1, memberDao.MemberList.Count);

            var member = memberDao.MemberList[0];

            Assert.Equal(member, result);
        }
    }
}
