﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Entities;
using Library.Interfaces.Entities;
using Xunit;

namespace Library.Tests.Integration.Entity
{
    [Trait("Category", "Book Tests")]
    public class BookIntegration
    {
        [Fact]
        public void CanBorrowBook()
        {
            var book = new Book("author", "title", "call number", 1);
            var member = new Member("first", "last", "phone", "email", 1);

            var loan = new Loan(book, member, DateTime.Today, DateTime.Today.AddDays(7));

            // Associate the book with the loan.
            book.Borrow(loan);

            // Set book state to ON_LOAN - affected by GetLoanFromBookReturnsNullIfBookIsNotON_LOAN()
            book.State = BookState.ON_LOAN;

            Assert.Equal(loan, book.Loan);
        }

        [Fact]
        public void BorrowBookThrowsRuntimeExceptionIfBookIsNotCurrentlyAvailable()
        {
            // Set book state to something other than Available.
            var book = new Book("author", "title", "call number", 1) { State = BookState.LOST };
            var member = new Member("first", "last", "phone", "email", 1);

            var loan = new Loan(book, member, DateTime.Today, DateTime.Today.AddDays(7));

            // Associate the book with the loan.
            var ex = Assert.Throws<InvalidOperationException>(() => book.Borrow(loan));

            Assert.Equal("Cannot borrow a book that is not available", ex.Message);
        }

        [Fact]
        public void CanGetLoanFromBook()
        {
            var book = new Book("author", "title", "call number", 1);
            var member = new Member("first", "last", "phone", "email", 1);

            var loan = new Loan(book, member, DateTime.Today, DateTime.Today.AddDays(7));

            // Associate the book with the loan.
            book.Borrow(loan);

            // Set book state to ON_LOAN - affected by GetLoanFromBookReturnsNullIfBookIsNotON_LOAN()
            book.State = BookState.ON_LOAN;

            // Testing the getter on the book.
            var loanRetrieved = book.Loan;

            Assert.Equal(loan, loanRetrieved);
        }

        [Fact]
        public void GetLoanFromBookReturnsNullIfBookIsNotON_LOAN()
        {
            var book = new Book("author", "title", "call number", 1);
            var member = new Member("first", "last", "phone", "email", 1);

            var loan = new Loan(book, member, DateTime.Today, DateTime.Today.AddDays(7));

            // Testing the getter on the book.
            var loanRetrieved = book.Loan;

            // Make sure the book returns null when no loan associated.
            Assert.Null(loanRetrieved);

            // Associate the book with the loan.
            book.Borrow(loan);

            // Make sure the loan comes back if it is on loan.
            book.State = BookState.ON_LOAN;

            loanRetrieved = book.Loan;

            // Make sure the loan retrieved is the same one loaned out.
            Assert.Equal(loan, loanRetrieved);

            // Set the loan state to not ON_LOAN.
            book.State = BookState.AVAILABLE;

            // Make sure null is returned if book is not ON_LOAN.
            loanRetrieved = book.Loan;

            Assert.Null(loanRetrieved);
        }

        [Fact]
        public void CanReturnBook()
        {
            var book = new Book("author", "title", "call number", 1);
            var member = new Member("first", "last", "phone", "email", 1);

            var loan = new Loan(book, member, DateTime.Today, DateTime.Today.AddDays(7));

            book.Borrow(loan);

            // Set book state to ON_LOAN - affected by GetLoanFromBookReturnsNullIfBookIsNotON_LOAN()
            book.State = BookState.ON_LOAN;

            Assert.Equal(loan, book.Loan);

            book.ReturnBook(false);

            Assert.Null(book.Loan);
        }

        [Fact]
        public void ReturningBookSetsStateToDamaged()
        {
            var book = new Book("author", "title", "call number", 1);
            var member = new Member("first", "last", "phone", "email", 1);

            var loan = new Loan(book, member, DateTime.Today, DateTime.Today.AddDays(7));

            book.Borrow(loan);

            // Set book state to ON_LOAN - affected by GetLoanFromBookReturnsNullIfBookIsNotON_LOAN()
            book.State = BookState.ON_LOAN;

            Assert.Equal(loan, book.Loan);

            // Set damaged flag to true.
            book.ReturnBook(true);

            Assert.Null(book.Loan);
            Assert.Equal(BookState.DAMAGED, book.State);
        }

        [Fact]
        public void ReturningBookSetsStateToAvailable()
        {
            var book = new Book("author", "title", "call number", 1);
            var member = new Member("first", "last", "phone", "email", 1);

            var loan = new Loan(book, member, DateTime.Today, DateTime.Today.AddDays(7));

            book.Borrow(loan);

            // Set book state to ON_LOAN - affected by GetLoanFromBookReturnsNullIfBookIsNotON_LOAN()
            book.State = BookState.ON_LOAN;

            Assert.Equal(loan, book.Loan);

            // Set damaged flag to false.
            book.ReturnBook(false);

            Assert.Null(book.Loan);
            Assert.Equal(BookState.AVAILABLE, book.State);
        }

        [Fact]
        public void ReturningBookThrowsExceptionIfNotOnLoan()
        {
            var book = new Book("author", "title", "call number", 1);
            var member = new Member("first", "last", "phone", "email", 1);

            var loan = new Loan(book, member, DateTime.Today, DateTime.Today.AddDays(7));

            book.Borrow(loan);

            // Set book state to ON_LOAN - affected by GetLoanFromBookReturnsNullIfBookIsNotON_LOAN()
            book.State = BookState.ON_LOAN;

            Assert.Equal(loan, book.Loan);

            // Set book state to LOST so we can make sure it's not ON_LOAN already.
            book.State = BookState.LOST;

            var ex = Assert.Throws<InvalidOperationException>(() => book.ReturnBook(false));

            Assert.Equal("Book is currently not on loan", ex.Message);
        }

        [Fact]
        public void WhenBookIsBorrowedShouldBeOnLoan()
        {
            var book = new Book("author", "title", "call number", 1);
            var member = new Member("first", "last", "phone", "email", 1);

            var loan = new Loan(book, member, DateTime.Today, DateTime.Today.AddDays(7));

            book.Borrow(loan);

            Assert.Equal(BookState.ON_LOAN, book.State);
        }

        [Fact]
        public void WhenBookIsOnLoanAndLostThenShouldBeLost()
        {
            var book = new Book("author", "title", "call number", 1);
            var member = new Member("first", "last", "phone", "email", 1);

            var loan = new Loan(book, member, DateTime.Today, DateTime.Today.AddDays(7));

            book.Borrow(loan);

            book.Lose();

            Assert.Equal(BookState.LOST, book.State);
        }

        [Fact]
        public void WhenBookIsLostAndDisposedShouldBeDisposed()
        {
            var book = new Book("author", "title", "call number", 1);
            var member = new Member("first", "last", "phone", "email", 1);

            var loan = new Loan(book, member, DateTime.Today, DateTime.Today.AddDays(7));

            book.Borrow(loan);

            book.Lose();

            book.Dispose();

            Assert.Equal(BookState.DISPOSED, book.State);
        }

        [Fact]
        public void WhenBookIsOnLoanAndReturnedUndamagedShouldBeAvailable()
        {
            var book = new Book("author", "title", "call number", 1);
            var member = new Member("first", "last", "phone", "email", 1);

            var loan = new Loan(book, member, DateTime.Today, DateTime.Today.AddDays(7));

            book.Borrow(loan);

            book.ReturnBook(false);

            Assert.Equal(BookState.AVAILABLE, book.State);
        }

        [Fact]
        public void WhenBookIsOnLoanAndReturnedDamagedShouldBeDamaged()
        {
            var book = new Book("author", "title", "call number", 1);
            var member = new Member("first", "last", "phone", "email", 1);

            var loan = new Loan(book, member, DateTime.Today, DateTime.Today.AddDays(7));

            book.Borrow(loan);

            book.ReturnBook(true);

            Assert.Equal(BookState.DAMAGED, book.State);
        }

        [Fact]
        public void WhenBookIsDamagedAndRepairedShouldBeAvailable()
        {
            var book = new Book("author", "title", "call number", 1);
            var member = new Member("first", "last", "phone", "email", 1);

            var loan = new Loan(book, member, DateTime.Today, DateTime.Today.AddDays(7));

            book.Borrow(loan);

            book.ReturnBook(true);

            book.Repair();

            Assert.Equal(BookState.AVAILABLE, book.State);
        }

        [Fact]
        public void WhenBookIsDamagedAndDisposedShouldBeDisposed()
        {
            var book = new Book("author", "title", "call number", 1);
            var member = new Member("first", "last", "phone", "email", 1);

            var loan = new Loan(book, member, DateTime.Today, DateTime.Today.AddDays(7));

            book.Borrow(loan);

            book.ReturnBook(true);

            book.Dispose();

            Assert.Equal(BookState.DISPOSED, book.State);
        }
    }
}
