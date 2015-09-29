using System;
using Library.Entities;
using Library.Interfaces.Entities;
using NSubstitute;
using Xunit;

namespace Library.Tests.UnitTests
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
            var book = new Book("author", "title", "call number", 1) {State = BookState.LOST};

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

        [Fact]
        public void ReturningBookSetsStateToDamaged()
        {
            var book = new Book("author", "title", "call number", 1);

            var loan = Substitute.For<ILoan>();
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

            var loan = Substitute.For<ILoan>();
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

            var loan = Substitute.For<ILoan>();
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
        public void LoseSetsBookStateToLost()
        {
            var book = new Book("author", "title", "call number", 1);

            book.State = BookState.ON_LOAN;

            book.Lose();

            Assert.Equal(BookState.LOST, book.State);
        }

        [Fact]
        public void LoseThrowsExceptionIfBookIsNotOnLoan()
        {
            var book = new Book("author", "title", "call number", 1);

            book.State = BookState.AVAILABLE;

            var ex = Assert.Throws<InvalidOperationException>(() => book.Lose());

            Assert.Equal("Book must be on loan to be marked as lost", ex.Message);
        }

        [Fact]
        public void RepairSetsBookStateToAvailable()
        {
            var book = new Book("author", "title", "call number", 1);

            // Must be set to Damaged to be repaired.
            book.State = BookState.DAMAGED;

            book.Repair();

            Assert.Equal(BookState.AVAILABLE, book.State);
        }

        [Fact]
        public void RepairThrowsExceptionIfBookIsNotDamaged()
        {
            var book = new Book("author", "title", "call number", 1);

            book.State = BookState.ON_LOAN;

            var ex = Assert.Throws<InvalidOperationException>(() => book.Repair());

            Assert.Equal("Book is not damaged so cannot be repaired", ex.Message);
        }

        [Fact]
        public void DisposeSetsBookStateToDisposed()
        {
            var book = new Book("author", "title", "call number", 1);

            book.State = BookState.AVAILABLE;

            book.Dispose();

            Assert.Equal(BookState.DISPOSED, book.State);
        }

        [Fact]
        public void DisposeThrowsException()
        {
            var book = new Book("author", "title", "call number", 1);

            // Set book to Available and should be able to dispose.
            book.State = BookState.AVAILABLE;

            book.Dispose();
            Assert.Equal(BookState.DISPOSED, book.State);

            // Set book to Damaged and should be able to dispose.
            book.State = BookState.DAMAGED;

            book.Dispose();
            Assert.Equal(BookState.DISPOSED, book.State);

            // Set book to Lost and should be able to dispose.
            book.State = BookState.LOST;

            book.Dispose();
            Assert.Equal(BookState.DISPOSED, book.State);

            // Should throw for others.
            book.State = BookState.ON_LOAN;
            var ex1 = Assert.Throws<InvalidOperationException>(() => book.Dispose());

            book.State = BookState.DISPOSED;
            var ex2 = Assert.Throws<InvalidOperationException>(() => book.Dispose());

            Assert.Equal(ex1.Message, ex2.Message);
            Assert.Equal("Book cannot be disposed in its current state", ex2.Message);
        }

        [Fact]
        public void CanGetState()
        {
            var book = new Book("author", "title", "call number", 1);

            book.State = BookState.AVAILABLE;

            Assert.Equal(BookState.AVAILABLE, book.State);
        }

        [Fact]
        public void CanGetAuthor()
        {
            var author = "Author";
            var title = "Title";
            var callNumber = "Call Number";
            var id = 1;

            var book = new Book(author, title, callNumber, id);

            Assert.Equal(author, book.Author);
        }

        [Fact]
        public void CanGetTitle()
        {
            var author = "Author";
            var title = "Title";
            var callNumber = "Call Number";
            var id = 1;

            var book = new Book(author, title, callNumber, id);

            Assert.Equal(title, book.Title);
        }

        [Fact]
        public void CanGetCallNumber()
        {
            var author = "Author";
            var title = "Title";
            var callNumber = "Call Number";
            var id = 1;

            var book = new Book(author, title, callNumber, id);

            Assert.Equal(callNumber, book.CallNumber);
        }

        [Fact]
        public void CanGetID()
        {
            var author = "Author";
            var title = "Title";
            var callNumber = "Call Number";
            var id = 1;

            var book = new Book(author, title, callNumber, id);

            Assert.Equal(id, book.ID);
        }
    }
}
