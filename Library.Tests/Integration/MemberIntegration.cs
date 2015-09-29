using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Daos;
using Library.Entities;
using Library.Interfaces.Daos;
using Library.Interfaces.Entities;
using Xunit;

namespace Library.Tests.Integration
{
    [Trait("Category", "Member Tests")]
    public class MemberIntegration
    {
        [Fact]
        public void CanCreateMember()
        {
            IMemberHelper helper = new MemberHelper();
            IMemberDAO dao = new MemberDao(helper);

            var firstName = "first";
            var lastName = "last";
            var contactPhone = "contactPhone";
            var emailAddress = "emailAddress";

            var member = dao.AddMember(firstName, lastName, contactPhone, emailAddress);

            Assert.NotNull(member);

            Assert.NotEmpty(dao.MemberList);
            Assert.Equal(1, dao.MemberList.Count);

            Assert.Equal(member, dao.MemberList[0]);

            Assert.Equal(firstName, member.FirstName);
            Assert.Equal(lastName, member.LastName);
            Assert.Equal(contactPhone, member.ContactPhone);
            Assert.Equal(emailAddress, member.EmailAddress);

            Assert.NotEqual(0, member.ID);
        }

        [Fact]
        public void CreateMemberFailsOnNullHelper()
        {
            IMemberHelper helper = null;
            Assert.Throws<ArgumentException>(() => { IMemberDAO dao = new MemberDao(helper); });
        }

        [Fact]
        public void CreateMemberCreatesAUniqueId()
        {
            IMemberHelper helper = new MemberHelper();
            IMemberDAO dao = new MemberDao(helper);

            var firstName = "first";
            var lastName = "last";
            var contactPhone = "contactPhone";
            var emailAddress = "emailAddress";

            var member = dao.AddMember(firstName, lastName, contactPhone, emailAddress);

            Assert.NotEqual(0, member.ID);
        }

        [Fact]
        public void CanGetMemberById()
        {
            IMemberHelper helper = new MemberHelper();
            IMemberDAO dao = new MemberDao(helper);

            var firstName = "first";
            var lastName = "last";
            var contactPhone = "contactPhone";
            var emailAddress = "emailAddress";

            var member = dao.AddMember(firstName, lastName, contactPhone, emailAddress);

            for (int i = 0; i < 10; i++)
            {
                dao.AddMember("Test", "Test", "test phone", "test email");
            }

            var result = dao.GetMemberByID(member.ID);

            Assert.Equal(member, result);
        }

        [Fact]
        public void GetMemberByIdReturnsNullIfNotFound()
        {
            IMemberHelper helper = new MemberHelper();
            IMemberDAO dao = new MemberDao(helper);

            var firstName = "first";
            var lastName = "last";
            var contactPhone = "contactPhone";
            var emailAddress = "emailAddress";

            var member = dao.AddMember(firstName, lastName, contactPhone, emailAddress);

            for (int i = 0; i < 10; i++)
            {
                dao.AddMember("Test", "Test", "test phone", "test email");
            }

            var result = dao.GetMemberByID(1000);

            Assert.Null(result);
        }

        [Fact]
        public void CanGetMemberByLastName()
        {
            IMemberHelper helper = new MemberHelper();
            IMemberDAO dao = new MemberDao(helper);

            var firstName = "first";
            var lastName = "last";
            var contactPhone = "contactPhone";
            var emailAddress = "emailAddress";

            var member = dao.AddMember(firstName, lastName, contactPhone, emailAddress);

            for (int i = 0; i < 10; i++)
            {
                dao.AddMember("Test", "Test", "test phone", "test email");
            }

            var result = dao.FindMembersByLastName(member.LastName);

            var singleResult = result.Single(); // Test there should only be one result and get it.

            Assert.Equal(member, singleResult);
        }
    }
}
