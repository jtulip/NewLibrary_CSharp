﻿using System;
using System.Linq;
using Library.Entities;
using Library.Interfaces.Entities;
using NSubstitute;
using Xunit;

namespace Library.Tests.UnitTests.Entity
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

        [Fact]
        public void HasOverdueLoansReturnsFalseIfNoLoanIsOverdue()
        {
            var member = new Member("test", "member", "phone", "email", 1);

            // Add a single loan that is not overdue.
            var loan = Substitute.For<ILoan>();
            loan.IsOverDue.Returns(false);
            member.Loans.Add(loan);

            Assert.False(member.HasOverDueLoans);
        }

        [Fact]
        public void HasOverdueLoansReturnsTrueIfAnyLoanIsOverdue()
        {
            var member = new Member("test", "member", "phone", "email", 1);

            // Add a loan that is not overdue.
            var loan = Substitute.For<ILoan>();
            loan.IsOverDue.Returns(false);
            member.Loans.Add(loan);

            // Add a second loan that is overdue.
            var second = Substitute.For<ILoan>();
            second.IsOverDue.Returns(true);
            member.Loans.Add(second);

            Assert.True(member.HasOverDueLoans);
        }

        [Fact]
        public void HasReachedLoanLimitReturnsTrueIfLoanCountEqualsLoanLimit()
        {
            var member = new Member("test", "member", "phone", "email", 1);

            // Add a loan.
            var loan = Substitute.For<ILoan>();
            loan.IsOverDue.Returns(false);
            member.Loans.Add(loan);

            // Test that Loan Limit is not reached.
            Assert.True(member.Loans.Count() < BookConstants.LOAN_LIMIT);
            Assert.False(member.HasReachedLoanLimit);

            // Add additional loans.
            while (member.Loans.Count() < BookConstants.LOAN_LIMIT)
            {
                member.Loans.Add(Substitute.For<ILoan>());
            }

            // Test that Loan Limit has been reached.
            Assert.True(member.Loans.Count() == BookConstants.LOAN_LIMIT);
            Assert.True(member.HasReachedLoanLimit);
        }

        [Fact]
        public void CanAddFineToMember()
        {
            var member = new Member("test", "member", "phone", "email", 1);

            const float fine1 = 5.50f;
            const float fine2 = 2.00f;

            // Has no fines after being created.
            Assert.Equal(0, member.FineAmount);

            // Can add a fine.
            member.AddFine(fine1);

            Assert.Equal(fine1, member.FineAmount);

            // Make sure it will increment by additional amount.
            member.AddFine(fine2);

            Assert.Equal(fine1 + fine2, member.FineAmount);
        }

        [Fact]
        public void AddFineThrowsArgumentExceptionIfNegative()
        {
            var member = new Member("test", "member", "phone", "email", 1);

            var ex = Assert.Throws<ArgumentException>(() => member.AddFine(-5.00f));

            Assert.Equal("Fine must not be negative value", ex.Message);

        }

        [Fact]
        public void CanPayFineByMember()
        {
            const float fine = 5.50f;
            const float payment = 2.50f;

            var member = new Member("test", "member", "phone", "email", 1);

            member.AddFine(fine);

            Assert.Equal(fine, member.FineAmount);

            member.PayFine(payment);

            Assert.Equal(fine - payment, member.FineAmount);
        }

        [Fact]
        public void PayFineThrowsArgumentExceptionIfNegative()
        {
            const float fine = 5.50f;
            const float payment = -1.00f;

            var member = new Member("test", "member", "phone", "email", 1);

            member.AddFine(fine);

            Assert.Equal(fine, member.FineAmount);

            var ex = Assert.Throws<ArgumentException>(() => member.PayFine(payment));

            Assert.Equal("Payment must not be negative value", ex.Message);
        }

        [Fact]
        public void PayFineThrowsArgumentExceptionIfPaymentExceedsFines()
        {
            const float fine = 5.50f;
            const float payment = 6.00f;

            var member = new Member("test", "member", "phone", "email", 1);

            member.AddFine(fine);

            Assert.Equal(fine, member.FineAmount);

            var fineAmount = member.FineAmount;

            var ex = Assert.Throws<ArgumentException>(() => member.PayFine(payment));

            Assert.Equal($"Payment must not exceed fines of {fineAmount}", ex.Message);
        }

        [Fact]
        public void HasFinesPayableReturnsTrueIfFinesExceedZero()
        {
            var member = new Member("test", "member", "phone", "email", 1);

            Assert.False(member.HasFinesPayable);

            member.AddFine(5.00f);

            Assert.True(member.HasFinesPayable);
        }

        [Fact]
        public void HasReachedFineLimitReturnsTrueIfFinesReachFineMax()
        {
            var member = new Member("test", "member", "phone", "email", 1);

            // Add a fine.
            const float fine1 = 5.50f;
            const float fine2 = BookConstants.FINE_LIMIT - fine1;
            const float fine3 = fine2 + 1.0f;

            member.AddFine(fine1);

            // Test that Fine Limit is not reached. Assuming FINE_MAX == FINE_LIMIT.
            Assert.True(member.FineAmount < BookConstants.FINE_LIMIT);
            Assert.False(member.HasReachedFineLimit);

            // Add additional fine to reach == FINE_LIMIT.
            member.AddFine(fine2);

            // Test that Loan Limit has been reached.
            Assert.True(member.FineAmount == BookConstants.FINE_LIMIT);
            Assert.True(member.HasReachedFineLimit);

            // Add additional fine to exceed FINE_LIMIT.
            member.AddFine(fine3);
            Assert.True(member.FineAmount > BookConstants.FINE_LIMIT);
            Assert.True(member.HasReachedFineLimit);
        }

        [Fact]
        public void CanAddLoanToMember()
        {
            var member = new Member("test", "member", "phone", "email", 1);

            // Can add a loan.
            var loan = Substitute.For<ILoan>();

            member.AddLoan(loan);

            Assert.Contains(loan, member.Loans);
        }

        [Fact]
        public void AddLoanThrowsArgumentNullExceptionIfLoanIsNull()
        {
            var member = new Member("test", "member", "phone", "email", 1);

            // Null loan.
            ILoan loan = null;

            var ex = Assert.Throws<ArgumentException>(() => member.AddLoan(loan));

            Assert.Equal("Loan cannot be null", ex.Message);
        }

        [Fact] // InvalidOperationException is more correct for .NET.
        public void AddLoanThrowsInvalidOperationExceptionIfMemberIsNotAllowedToBorrow()
        {
            // Set member to BORROWING_DISALLOWED
            var member = new Member("test", "member", "phone", "email", 1) { State = MemberState.BORROWING_DISALLOWED };

            var loan = Substitute.For<ILoan>();

            var ex = Assert.Throws<InvalidOperationException>(() => member.AddLoan(loan));

            Assert.Equal("Cannot add a loan when member is not allowed to borrow", ex.Message);
        }

        [Fact]
        public void CanGetLoans()
        {
            var member = new Member("test", "member", "phone", "email", 1);

            var loan1 = Substitute.For<ILoan>();
            var loan2 = Substitute.For<ILoan>();
            var loan3 = Substitute.For<ILoan>();
            var loan4 = Substitute.For<ILoan>();

            member.AddLoan(loan1);
            member.AddLoan(loan2);
            member.AddLoan(loan3);

            Assert.Equal(3, member.Loans.Count());
            Assert.Contains(loan1, member.Loans);
            Assert.DoesNotContain(loan4, member.Loans);
        }

        [Fact]
        public void CanRemoveLoanFromMember()
        {
            var member = new Member("test", "member", "phone", "email", 1);

            var loan = Substitute.For<ILoan>();

            member.AddLoan(loan);

            // Make sure Member has Loan.
            Assert.Contains(loan, member.Loans);

            member.RemoveLoan(loan);

            // Make sure Loan has been removed from Member.
            Assert.DoesNotContain(loan, member.Loans);
        }

        [Fact]
        public void RemoveLoanThrowsArgumentNullExceptionIfLoanIsNull()
        {
            var member = new Member("test", "member", "phone", "email", 1);

            ILoan loan = null;

            var ex = Assert.Throws<ArgumentException>(() => member.RemoveLoan(loan));

            Assert.Equal("Loan cannot be null", ex.Message);
        }

        [Fact]
        public void RemoveLoanThrowsArgumentExceptionIfLoanIsNotInCollection()
        {
            var member = new Member("test", "member", "phone", "email", 1);

            var loan1 = Substitute.For<ILoan>();
            var loan2 = Substitute.For<ILoan>();

            member.AddLoan(loan1);

            var ex = Assert.Throws<ArgumentException>(() => member.RemoveLoan(loan2));

            Assert.Equal("Loan was not found in member's loans", ex.Message);
        }

        [Fact]
        public void CanGetState()
        {
            var member = new Member("test", "member", "phone", "email", 1) { State = MemberState.BORROWING_ALLOWED };

            Assert.Equal(MemberState.BORROWING_ALLOWED, member.State);

            member.State = MemberState.BORROWING_DISALLOWED;
            
            Assert.Equal(MemberState.BORROWING_DISALLOWED, member.State);
        }

        [Fact]
        public void CanGetFirstName()
        {
            var firstName = "test";
            var lastName = "member";
            var phoneNumber = "phone";
            var emailAddress = "email";
            var id = 1;

            var member = new Member(firstName, lastName, phoneNumber, emailAddress, id);

            Assert.Equal(firstName, member.FirstName);
        }

        [Fact]
        public void CanGetLastName()
        {
            var firstName = "test";
            var lastName = "member";
            var phoneNumber = "phone";
            var emailAddress = "email";
            var id = 1;

            var member = new Member(firstName, lastName, phoneNumber, emailAddress, id);

            Assert.Equal(lastName, member.LastName);
        }

        [Fact]
        public void CanGetContactPhone()
        {
            var firstName = "test";
            var lastName = "member";
            var phoneNumber = "phone";
            var emailAddress = "email";
            var id = 1;

            var member = new Member(firstName, lastName, phoneNumber, emailAddress, id);

            Assert.Equal(phoneNumber, member.ContactPhone);
        }

        [Fact]
        public void CanGetEmailAddress()
        {
            var firstName = "test";
            var lastName = "member";
            var phoneNumber = "phone";
            var emailAddress = "email";
            var id = 1;

            var member = new Member(firstName, lastName, phoneNumber, emailAddress, id);

            Assert.Equal(emailAddress, member.EmailAddress);
        }

        [Fact]
        public void CanGetID()
        {
            var firstName = "test";
            var lastName = "member";
            var phoneNumber = "phone";
            var emailAddress = "email";
            var id = 1;

            var member = new Member(firstName, lastName, phoneNumber, emailAddress, id);

            Assert.Equal(id, member.ID);
        }

        [Fact]
        public void ReturnMemberReadable()
        {
            var member = new Member("Jim", "Tulip", "Phone", "Email", 1);

            var readable = $"{member.FirstName} {member.LastName}";

            Assert.Equal(readable, member.ToString());
        }
    }
}

