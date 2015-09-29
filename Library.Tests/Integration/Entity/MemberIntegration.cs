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

    }
}

