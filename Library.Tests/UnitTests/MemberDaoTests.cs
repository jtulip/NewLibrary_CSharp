using System;
using System.Collections.Generic;
using System.Linq;
using Library.Daos;
using Library.Entities;
using Library.Interfaces.Daos;
using Library.Interfaces.Entities;
using NSubstitute;
using Xunit;

namespace Library.Tests.UnitTests
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

        [Fact]
        public void AddMemberAssignsUniqueID()
        {
            var helper = Substitute.For<IMemberHelper>();

            var memberDao = new MemberDao(helper);

            var firstName = "first";
            var lastName = "last";
            var contactPhone = "phone";
            var emailAddress = "email address";

            // Make sure the id increments as Members are added.
            for (var id = 1; id < 10; id++)
            {
                helper.MakeMember(firstName, lastName, contactPhone, emailAddress, id).Returns(new Member(firstName, lastName, contactPhone, emailAddress, id));

                var result = memberDao.AddMember(firstName, lastName, contactPhone, emailAddress);

                // Assert that the mock's MakeMember method was called.
                helper.Received().MakeMember(firstName, lastName, contactPhone, emailAddress, id);

                // Make sure the id of the Member is new.
                Assert.Equal(id, result.ID);
            }
        }

        [Fact]
        public void CanGetMemberById()
        {
            var helper = Substitute.For<IMemberHelper>();

            var memberDao = new MemberDao(helper);

            var firstName = "first";
            var lastName = "last";
            var contactPhone = "phone";
            var emailAddress = "email address";

            memberDao.MemberList = new List<IMember>
            {
                new Member("one", "two", "three", "four", 1),
                new Member(firstName, lastName, contactPhone, emailAddress, 2),
                new Member("one", "two", "three", "four", 3),
            };

            var member = memberDao.GetMemberByID(2);

            Assert.NotNull(member);

            Assert.Equal(2, member.ID);
            Assert.Equal(firstName, member.FirstName);
            Assert.Equal(lastName, member.LastName);
            Assert.Equal(contactPhone, member.ContactPhone);
            Assert.Equal(emailAddress, member.EmailAddress);
        }

        [Fact]
        public void GetMemberByIdReturnsNullIfNotFound()
        {
            var helper = Substitute.For<IMemberHelper>();

            var memberDao = new MemberDao(helper);

            memberDao.MemberList = new List<IMember>
            {
                new Member("one", "two", "three", "four", 1),
                new Member("one", "two", "three", "four", 3),
            };

            var member = memberDao.GetMemberByID(2);

            Assert.Null(member);
        }

        [Fact]
        public void CanGetMemberByLastName()
        {
            var helper = Substitute.For<IMemberHelper>();

            var memberDao = new MemberDao(helper);

            var firstName = "first";
            var lastName = "last";
            var contactPhone = "phone";
            var emailAddress = "email address";

            memberDao.MemberList = new List<IMember>
            {
                new Member("one", "two", "three", "four", 1),
                new Member(firstName, lastName, contactPhone, emailAddress, 2),
                new Member("one", "two", "three", "four", 3),
            };

            var member = memberDao.FindMembersByLastName(lastName).Single();

            Assert.NotNull(member);

            Assert.Equal(2, member.ID);
            Assert.Equal(firstName, member.FirstName);
            Assert.Equal(lastName, member.LastName);
            Assert.Equal(contactPhone, member.ContactPhone);
            Assert.Equal(emailAddress, member.EmailAddress);
        }

        [Fact]
        public void GetMemberByLastNameReturnsNullIfNotFound()
        {
            var helper = Substitute.For<IMemberHelper>();

            var memberDao = new MemberDao(helper);

            var firstName = "first";
            var lastName = "last";
            var contactPhone = "phone";
            var emailAddress = "email address";

            memberDao.MemberList = new List<IMember>
            {
                new Member("one", "two", "three", "four", 1),
                new Member("one", "two", "three", "four", 3),
            };

            var list = memberDao.FindMembersByLastName(lastName);

            Assert.NotNull(list); // Shouldn't be null, should be an empty collection.
            Assert.Empty(list);
        }

        [Fact]
        public void CanGetMemberByEmailAddress()
        {
            var helper = Substitute.For<IMemberHelper>();

            var memberDao = new MemberDao(helper);

            var firstName = "first";
            var lastName = "last";
            var contactPhone = "phone";
            var emailAddress = "email address";

            memberDao.MemberList = new List<IMember>
            {
                new Member("one", "two", "three", "four", 1),
                new Member(firstName, lastName, contactPhone, emailAddress, 2),
                new Member("one", "two", "three", "four", 3),
            };

            var member = memberDao.FindMembersByEmailAddress(emailAddress).Single();

            Assert.NotNull(member);

            Assert.Equal(2, member.ID);
            Assert.Equal(firstName, member.FirstName);
            Assert.Equal(lastName, member.LastName);
            Assert.Equal(contactPhone, member.ContactPhone);
            Assert.Equal(emailAddress, member.EmailAddress);
        }

        [Fact]
        public void GetMemberByEmailAddressReturnsNullIfNotFound()
        {
            var helper = Substitute.For<IMemberHelper>();

            var memberDao = new MemberDao(helper);

            var firstName = "first";
            var lastName = "last";
            var contactPhone = "phone";
            var emailAddress = "email address";

            memberDao.MemberList = new List<IMember>
            {
                new Member("one", "two", "three", "four", 1),
                new Member("one", "two", "three", "four", 3),
            };

            var list = memberDao.FindMembersByEmailAddress(emailAddress);

            Assert.NotNull(list); // Shouldn't be null, should be an empty collection.
            Assert.Empty(list);
        }

        [Fact]
        public void CanGetMemberByNames()
        {
            var helper = Substitute.For<IMemberHelper>();

            var memberDao = new MemberDao(helper);

            var firstName = "first";
            var lastName = "last";
            var contactPhone = "phone";
            var emailAddress = "email address";

            memberDao.MemberList = new List<IMember>
            {
                new Member("one", "two", "three", "four", 1),
                new Member(firstName, lastName, contactPhone, emailAddress, 2),
                new Member("one", "two", "three", "four", 3),
                new Member(firstName, "Foo", contactPhone, emailAddress, 4),
            };

            var member = memberDao.FindMembersByNames(firstName, lastName).Single();

            Assert.NotNull(member);

            Assert.Equal(2, member.ID);
            Assert.Equal(firstName, member.FirstName);
            Assert.Equal(lastName, member.LastName);
            Assert.Equal(contactPhone, member.ContactPhone);
            Assert.Equal(emailAddress, member.EmailAddress);
        }

        [Fact]
        public void GetMemberByNamesReturnsNullIfNotFound()
        {
            var helper = Substitute.For<IMemberHelper>();

            var memberDao = new MemberDao(helper);

            var firstName = "first";
            var lastName = "last";
            var contactPhone = "phone";
            var emailAddress = "email address";

            memberDao.MemberList = new List<IMember>
            {
                new Member("one", "two", "three", "four", 1),
                new Member("one", "two", "three", "four", 3),
            };

            var list = memberDao.FindMembersByNames(firstName, lastName);

            Assert.NotNull(list); // Shouldn't be null, should be an empty collection.
            Assert.Empty(list);
        }
    }
}
