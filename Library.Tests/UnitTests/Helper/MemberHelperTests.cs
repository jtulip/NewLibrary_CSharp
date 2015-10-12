using Library.Entities;
using Library.Interfaces.Entities;
using Xunit;

namespace Library.Tests.UnitTests.Helper
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

        [Fact]
        public void CanMakeMember()
        {
            var firstName = "first";
            var lastName = "last";
            var contactPhone = "phone";
            var emailAddress = "email";
            var id = 10;

            var helper = new MemberHelper();

            var member = helper.MakeMember(firstName, lastName, contactPhone, emailAddress, id);

            Assert.NotNull(member);
            Assert.Equal(id, member.ID);
            Assert.Equal(firstName, member.FirstName);
            Assert.Equal(lastName, member.LastName);
            Assert.Equal(contactPhone, member.ContactPhone);
            Assert.Equal(emailAddress, member.EmailAddress);
        }
    }
}
