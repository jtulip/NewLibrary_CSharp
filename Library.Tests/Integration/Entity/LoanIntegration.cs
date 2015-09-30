﻿using System;
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


        [Fact]
        public void LoanCtorThrowsIllegalParameterException()
        {
            var book = new Book("author", "title", "call number", 1);
            var member = new Member("first", "last", "phone", "email", 1);

            DateTime borrowDate = DateTime.Today;
            DateTime dueDate = DateTime.Today.AddDays(7);

            var ex = Assert.Throws<ArgumentException>(() =>
            {
                var loan = new Loan(null, member, borrowDate, dueDate);
            });

            Assert.Equal("Book needs to be provided", ex.Message);

            ex = Assert.Throws<ArgumentException>(() =>
            {
                new Loan(book, null, borrowDate, dueDate);
            });

            Assert.Equal("Member needs to be provided", ex.Message);

            ex = Assert.Throws<ArgumentException>(() =>
            {
                // DateTime can't be null in .NET
                new Loan(book, member, DateTime.MinValue, dueDate);
            });

            Assert.Equal("Borrow date needs to be provided", ex.Message);

            ex = Assert.Throws<ArgumentException>(() =>
            {
                // DateTime can't be null in .NET
                new Loan(book, member, borrowDate, DateTime.MinValue);
            });

            Assert.Equal("Due date needs to be provided", ex.Message);

            ex = Assert.Throws<ArgumentException>(() =>
            {
                // DateTime can't be null in .NET
                new Loan(book, member, borrowDate, borrowDate.AddDays(-1));
            });

            Assert.Equal("Due date cannot be before Borrow date", ex.Message);
        }


    }
}
