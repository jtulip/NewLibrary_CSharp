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
        public void WhenBorrowingAllowedAndLoanAddedAndNotLimitReachedBorrowingAllowed()
        {
            var member = new Member("firstName", "lastName", "contactPhone", "emailAddress", 1);

            var loan = Substitute.For<ILoan>();

            member.AddLoan(loan);

            Assert.False(member.HasReachedLoanLimit);

            Assert.Equal(MemberState.BORROWING_ALLOWED, member.State);
        }

        [Fact]
        public void WhenBorrowingAllowedAndLoanAddedAndLimitReachedBorrowingDisallowed()
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
        public void WhenBorrowingAllowedAndFinePaidBorrowingAllowed()
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

        [Fact]
        public void WhenBorrowingAllowedAndFinedMoreThanMaxBorrowingDisallowed()
        {
            var member = new Member("firstName", "lastName", "contactPhone", "emailAddress", 1);

            var loan = Substitute.For<ILoan>();

            var fineAmount = BookConstants.FINE_LIMIT + 1.00f;

            member.AddLoan(loan);

            Assert.False(member.HasReachedLoanLimit);

            Assert.Equal(MemberState.BORROWING_ALLOWED, member.State);

            member.AddFine(fineAmount);

            Assert.True(member.HasReachedFineLimit);

            Assert.True(fineAmount > BookConstants.FINE_LIMIT);

            Assert.Equal(MemberState.BORROWING_DISALLOWED, member.State);
        }

        [Fact]
        public void WhenBorrowingAllowedAndRemovingLoanBorrowingAllowed()
        {
            var member = new Member("firstName", "lastName", "contactPhone", "emailAddress", 1);

            var loan = Substitute.For<ILoan>();

            member.AddLoan(loan);

            Assert.False(member.HasReachedLoanLimit);

            Assert.Equal(MemberState.BORROWING_ALLOWED, member.State);

            member.RemoveLoan(loan);

            Assert.Equal(MemberState.BORROWING_ALLOWED, member.State);
        }

        [Fact]
        public void WhenBorrowingAllowedAndNoOverdueLoansBorrowingAllowed()
        {
            var member = new Member("firstName", "lastName", "contactPhone", "emailAddress", 1);

            var loan = Substitute.For<ILoan>();

            loan.CheckOverDue(DateTime.Today).Returns(false);

            member.AddLoan(loan);

            Assert.Equal(MemberState.BORROWING_ALLOWED, member.State);

            foreach (var l in member.Loans) l.CheckOverDue(DateTime.Today);

            Assert.False(member.HasOverDueLoans);

            loan.Received().CheckOverDue(DateTime.Today);

            Assert.Equal(MemberState.BORROWING_ALLOWED, member.State);
        }

        [Fact]
        public void WhenBorrowingAllowedAndHasOverdueLoansBorrowingDisallowed()
        {
            var member = new Member("firstName", "lastName", "contactPhone", "emailAddress", 1);

            var loan = Substitute.For<ILoan>();

            loan.CheckOverDue(DateTime.Today).Returns(true);
            loan.IsOverDue.Returns(true);

            member.AddLoan(loan);

            Assert.Equal(MemberState.BORROWING_ALLOWED, member.State);

            foreach (var l in member.Loans) l.CheckOverDue(DateTime.Today);

            Assert.True(member.HasOverDueLoans);

            loan.Received().CheckOverDue(DateTime.Today);

            Assert.Equal(MemberState.BORROWING_DISALLOWED, member.State);
        }

        [Fact]
        public void WhenBorrowingDisallowedAndFineAddedBorrowingDisallowed()
        {
            var member = new Member("firstName", "lastName", "contactPhone", "emailAddress", 1);

            var loan = Substitute.For<ILoan>();

            var fineAmount = BookConstants.FINE_LIMIT + 1.00f;

            member.AddLoan(loan);

            member.AddFine(fineAmount);

            // Borrowing state disallowed.
            Assert.Equal(MemberState.BORROWING_DISALLOWED, member.State);

            member.AddFine(1.00f);

            Assert.Equal(MemberState.BORROWING_DISALLOWED, member.State);
        }

        [Fact]
        public void WhenBorrowingDisallowedAndFinePaidButStillOverFineLimitBorrowingDisallowed()
        {
            var member = new Member("firstName", "lastName", "contactPhone", "emailAddress", 1);

            var loan = Substitute.For<ILoan>();

            var fineAmount = BookConstants.FINE_LIMIT + 5.00f;

            member.AddLoan(loan);

            member.AddFine(fineAmount);

            // Borrowing state disallowed.
            Assert.Equal(MemberState.BORROWING_DISALLOWED, member.State);

            member.PayFine(fineAmount - BookConstants.FINE_LIMIT);

            Assert.Equal(MemberState.BORROWING_DISALLOWED, member.State);
        }
    }
}
