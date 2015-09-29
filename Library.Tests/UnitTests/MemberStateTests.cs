using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Entities;
using Library.Interfaces.Entities;
using NSubstitute;
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

        [Fact]
        public void WhenMemberCreatedAndLoanAddedAndNotLimitReachedBorrowingAllowed()
        {
            var member = new Member("firstName", "lastName", "contactPhone", "emailAddress", 1);

            var loan = Substitute.For<ILoan>();

            member.AddLoan(loan);

            Assert.False(member.HasReachedLoanLimit);

            Assert.Equal(MemberState.BORROWING_ALLOWED, member.State);
        }

        [Fact]
        public void WhenMemberAddLoanAndLimitReachedBorrowingDisallowed()
        {
            var member = new Member("firstName", "lastName", "contactPhone", "emailAddress", 1);

            while (!member.HasReachedLoanLimit)
            {
                var loan = Substitute.For<ILoan>();

                member.AddLoan(loan);
            }

            Assert.True(member.HasReachedLoanLimit);

            Assert.Equal(MemberState.BORROWING_DISALLOWED, member.State);
        }

        [Fact]
        public void WhenBorrowingAllowedAndFinedLessThanMaxBorrowingAllowed()
        {
            var member = new Member("firstName", "lastName", "contactPhone", "emailAddress", 1);

            var loan = Substitute.For<ILoan>();

            var fineAmount = 5.00f;

            member.AddLoan(loan);

            Assert.False(member.HasReachedLoanLimit);

            Assert.Equal(MemberState.BORROWING_ALLOWED, member.State);

            member.AddFine(fineAmount);

            Assert.False(member.HasReachedFineLimit);

            Assert.True(fineAmount < BookConstants.FINE_LIMIT);

            Assert.Equal(MemberState.BORROWING_ALLOWED, member.State);
        }

        [Fact]
        public void WhenBorrowingAllowedAndLoanPaidBorrowingAllowed()
        {
            var member = new Member("firstName", "lastName", "contactPhone", "emailAddress", 1);

            var loan = Substitute.For<ILoan>();

            member.AddLoan(loan);

            Assert.False(member.HasReachedLoanLimit);

            Assert.Equal(MemberState.BORROWING_ALLOWED, member.State);

            member.AddFine(5.00f);

            Assert.Equal(MemberState.BORROWING_ALLOWED, member.State);

            member.PayFine(5.00f);

            Assert.Equal(MemberState.BORROWING_ALLOWED, member.State);
        }
    }
}
