﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Entities;
using Library.Interfaces.Entities;
using NSubstitute;
using Xunit;

namespace Library.Tests
{
    [Trait("Category", "Book Tests")]
    public class BookTests
    {
        [Fact]
        public void CanCreateANewBook()
        {
            var book = new Book("author", "title", "call number", 1);

            Assert.NotNull(book);
        }

        [Fact]
        public void BookImplementsIBookInterface()
        {
            var book = new Book("author", "title", "call number", 1);

            Assert.IsAssignableFrom<IBook>(book);

            var typedMember = book as IBook;

            Assert.NotNull(typedMember);
        }

        [Fact]
        public void BookCtorThrowsIllegalParameterException()
        {
            // Test to make sure that author is required.
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                var book = new Book("", "", "", 0);
            });

            Assert.Equal("Author needs to be provided.", ex.Message);

            // Test to make sure that title is required.
            ex = Assert.Throws<ArgumentException>(() =>
            {
                var book = new Book("author", "", "", 0);
            });

            Assert.Equal("Title needs to be provided.", ex.Message);

            // Test to make sure that call number is required.
            ex = Assert.Throws<ArgumentException>(() =>
            {
                var book = new Book("author", "title", "", 0);
            });

            Assert.Equal("Call Number needs to be provided.", ex.Message);

            // Tests to make sure that id is not less than or equal to 0.
            ex = Assert.Throws<ArgumentException>(() =>
            {
                var book = new Book("author", "title", "callNumber", 0);
            });

            Assert.Equal("ID needs to be greater than 0.", ex.Message);

            ex = Assert.Throws<ArgumentException>(() =>
            {
                var book = new Book("author", "title", "callNumber", -5);
            });

            Assert.Equal("ID needs to be greater than 0.", ex.Message);
        }

        [Fact]
        public void CanBorrowBook()
        {
            var book = new Book("author", "title", "call number", 1);

            var loan = Substitute.For<ILoan>();

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

            var loan = Substitute.For<ILoan>();

            // Associate the book with the loan.
            var ex = Assert.Throws<InvalidOperationException>(() => book.Borrow(loan));

            Assert.Equal("Cannot borrow a book that is not available", ex.Message);
        }

        [Fact]
        public void CanGetLoanFromBook()
        {
            var book = new Book("author", "title", "call number", 1);

            var loan = Substitute.For<ILoan>();

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

            // Testing the getter on the book.
            var loanRetrieved = book.Loan;

            // Make sure the book returns null when no loan associated.
            Assert.Null(loanRetrieved);

            var loan = Substitute.For<ILoan>();

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

            var loan = Substitute.For<ILoan>();
            book.Borrow(loan);

            // Set book state to ON_LOAN - affected by GetLoanFromBookReturnsNullIfBookIsNotON_LOAN()
            book.State = BookState.ON_LOAN;

            Assert.Equal(loan, book.Loan);

            book.ReturnBook(false);

            Assert.Null(book.Loan);
        }
    }
}