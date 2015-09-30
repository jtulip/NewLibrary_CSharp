using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Entities;
using Xunit;

namespace Library.Tests.Integration.Entity
{
    [Trait("Category", "Loan Tests")]
    public class LoanIntegration
    {
        [Fact]
        public void CanCreateLoan()
        {
            var book = new Book("author", "title", "call number", 1);
            var member = new Member("first", "last", "phone", "email", 1);
            
            DateTime borrowDate = DateTime.Today;
            DateTime dueDate = DateTime.Today.AddDays(7);

            // Jim has stated that the initial spec is incorrect and it should not take an ID, but should return a default ID of 0.
            var loan = new Loan(book, member, borrowDate, dueDate);

            Assert.NotNull(loan);

            // By new rule, assert ID = 0.
            Assert.Equal(0, loan.ID);
        }
    }
}
