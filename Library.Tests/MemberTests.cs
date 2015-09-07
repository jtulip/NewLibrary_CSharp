using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Entities;
using Library.Interfaces.Entities;
using Xunit;

namespace Library.Tests
{
    [Trait("Category", "Member Tests")]
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
            var member = new Member("firstName", "lastName", "contactPhone", "emailAddress", 1);

            Assert.IsAssignableFrom<IMember>(member);

            var typedMember = member as IMember;

            Assert.NotNull(typedMember);
        }

        [Fact]
        public void MemberCtorThrowsIllegalParameterException()
        {
            // Test to make sure that first name is required.
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                var member = new Member("", "", "", "", 0);
            });

            Assert.Equal("First Name needs to be provided.", ex.Message);

            // Test to make sure that last name is required.
            ex = Assert.Throws<ArgumentException>(() =>
            {
                var member = new Member("firstName", "", "", "", 0);
            });

            Assert.Equal("Last Name needs to be provided.", ex.Message);

            // Test to make sure that contact phone is required.
            ex = Assert.Throws<ArgumentException>(() =>
            {
                var member = new Member("firstName", "lastName", "", "", 0);
            });

            Assert.Equal("Contact Phone needs to be provided.", ex.Message);

            // Test to make sure that email address is required.
            ex = Assert.Throws<ArgumentException>(() =>
            {
                var member = new Member("firstName", "lastName", "contactPhone", "", 0);
            });

            Assert.Equal("Email Address needs to be provided.", ex.Message);


            // Tests to make sure that id is not less than or equal to 0.
            ex = Assert.Throws<ArgumentException>(() =>
            {
                var member = new Member("firstName", "lastName", "contactPhone", "emailAddress", 0);
            });

            Assert.Equal("ID needs to be greater than 0.", ex.Message);

            ex = Assert.Throws<ArgumentException>(() =>
            {
                var member = new Member("firstName", "lastName", "contactPhone", "emailAddress", -5);
            });

            Assert.Equal("ID needs to be greater than 0.", ex.Message);
        }
    }
}
