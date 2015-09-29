using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Entities;
using Library.Interfaces.Entities;
using Xunit;

namespace Library.Tests.Integration.Entity
{
    [Trait("Category", "Member Tests")]
    public class MemberIntegration
    {
        [Fact]
        public void HasOverdueLoansReturnsFalseIfNoLoanIsOverdue()
        {
            var book = new Book("author", "title", "call number", 1);

            var member = new Member("first", "last", "phone", "email", 1);

            // Add a single loan that is not overdue.
            var loan = new Loan(book, member, DateTime.Today, DateTime.Today.AddDays(7));

            member.Loans.Add(loan);

            Assert.False(member.HasOverDueLoans);
        }

        [Fact]
        public void HasOverdueLoansReturnsTrueIfAnyLoanIsOverdue()
        {
            var book = new Book("author", "title", "call number", 1);

            var member = new Member("first", "last", "phone", "email", 1);

            // Add a single loan that is not overdue.
            var loan = new Loan(book, member, DateTime.Today, DateTime.Today.AddDays(14)) { State = LoanState.CURRENT };

            member.AddLoan(loan);

            // Add a second loan that is overdue.
            var secondLoan = new Loan(book, member, DateTime.Today, DateTime.Today.AddDays(7)) { State = LoanState.CURRENT };

            member.AddLoan(secondLoan);

            // Check if overdue

            foreach (var l in member.Loans)
            {
                l.CheckOverDue(DateTime.Today.AddDays(10));
            }

            Assert.True(member.HasOverDueLoans);
        }

        [Fact]
        public void HasReachedLoanLimitReturnsTrueIfLoanCountEqualsLoanLimit()
        {
            var book = new Book("author", "title", "call number", 1);

            var member = new Member("first", "last", "phone", "email", 1);

            // Add a loan.
            var loan = new Loan(book, member, DateTime.Today, DateTime.Today.AddDays(14)) { State = LoanState.CURRENT };

            member.AddLoan(loan);

            // Test that Loan Limit is not reached.
            Assert.True(member.Loans.Count() < BookConstants.LOAN_LIMIT);
            Assert.False(member.HasReachedLoanLimit);

            // Add additional loans.
            while (member.Loans.Count() < BookConstants.LOAN_LIMIT)
            {
                member.AddLoan(new Loan(book, member, DateTime.Today, DateTime.Today.AddDays(7)));
            }

            // Test that Loan Limit has been reached.
            Assert.True(member.Loans.Count() == BookConstants.LOAN_LIMIT);
            Assert.True(member.HasReachedLoanLimit);
        }

        [Fact]
        public void CanAddLoanToMember()
        {
            var book = new Book("author", "title", "call number", 1);

            var member = new Member("first", "last", "phone", "email", 1);

            // Add a loan.
            var loan = new Loan(book, member, DateTime.Today, DateTime.Today.AddDays(14)) { State = LoanState.CURRENT };

            member.AddLoan(loan);

            Assert.Contains(loan, member.Loans);
        }

        [Fact] // InvalidOperationException is more correct for .NET.
        public void AddLoanThrowsInvalidOperationExceptionIfMemberIsNotAllowedToBorrow()
        {
            var book = new Book("author", "title", "call number", 1);

            // Set member to BORROWING_DISALLOWED
            var member = new Member("first", "last", "phone", "email", 1) { State = MemberState.BORROWING_DISALLOWED };

            var loan = new Loan(book, member, DateTime.Today, DateTime.Today.AddDays(14)) { State = LoanState.CURRENT };

            var ex = Assert.Throws<InvalidOperationException>(() => member.AddLoan(loan));

            Assert.Equal("Cannot add a loan when member is not allowed to borrow", ex.Message);
        }


        [Fact]
        public void CanGetLoans()
        {
            var book = new Book("author", "title", "call number", 1);

            var member = new Member("first", "last", "phone", "email", 1);

            var loan1 = new Loan(book, member, DateTime.Today, DateTime.Today.AddDays(14)) { State = LoanState.CURRENT };
            var loan2 = new Loan(book, member, DateTime.Today, DateTime.Today.AddDays(14)) { State = LoanState.CURRENT };
            var loan3 = new Loan(book, member, DateTime.Today, DateTime.Today.AddDays(14)) { State = LoanState.CURRENT };
            var loan4 = new Loan(book, member, DateTime.Today, DateTime.Today.AddDays(14)) { State = LoanState.CURRENT };

            member.AddLoan(loan1);
            member.AddLoan(loan2);
            member.AddLoan(loan3);

            Assert.Equal(3, member.Loans.Count());
            Assert.Contains(loan1, member.Loans);
            Assert.DoesNotContain(loan4, member.Loans);
        }
    }
}

