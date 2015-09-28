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
    [Trait("Category", "Book Tests")]
    public class BookStateTests
    {
        [Fact]
        public void WhenBookIsCreatedShouldBeAvailable()
        {
            var book = new Book("author", "title", "call number", 1);

            Assert.Equal(BookState.AVAILABLE, book.State);
        }

        [Fact]
        public void WhenBookIsDisposedShouldBeDisposed()
        {
            var book = new Book("author", "title", "call number", 1);

            book.Dispose();

            Assert.Equal(BookState.DISPOSED, book.State);
        }

        [Fact]
        public void WhenBookIsBorrowedShouldBeOnLoan()
        {
            var book = new Book("author", "title", "call number", 1);

            var loan = Substitute.For<ILoan>();

            book.Borrow(loan);

            Assert.Equal(BookState.ON_LOAN, book.State);
        }

        [Fact]
        public void WhenBookIsOnLoanAndLostThenShouldBeLost()
        {
            var book = new Book("author", "title", "call number", 1);

            var loan = Substitute.For<ILoan>();

            book.Borrow(loan);

            book.Lose();

            Assert.Equal(BookState.LOST, book.State);
        }

        // This test will fail as the text states that a book has to be status ON_LOAN to return, and won't work for LOST.
        //[Fact]
        //public void WhenBookIsLostAndReturnedUndamagedShouldBeAvailable()
        //{
        //    var book = new Book("author", "title", "call number", 1);

        //    var loan = Substitute.For<ILoan>();

        //    book.Borrow(loan);

        //    book.Lose();

        //    book.ReturnBook(false);

        //    Assert.Equal(BookState.AVAILABLE, book.State);
        //}

        // This test will fail as the text states that a book has to be status ON_LOAN to return, and won't work for LOST.
        //[Fact]
        //public void WhenBookIsLostAndReturnedDamagedShouldBeDamaged()
        //{
        //    var book = new Book("author", "title", "call number", 1);

        //    var loan = Substitute.For<ILoan>();

        //    book.Borrow(loan);

        //    book.Lose();

        //    book.ReturnBook(true);

        //    Assert.Equal(BookState.DAMAGED, book.State);
        //}

        [Fact]
        public void WhenBookIsLostAndDisposedShouldBeDisposed()
        {
            var book = new Book("author", "title", "call number", 1);

            var loan = Substitute.For<ILoan>();

            book.Borrow(loan);

            book.Lose();

            book.Dispose();

            Assert.Equal(BookState.DISPOSED, book.State);
        }

        [Fact]
        public void WhenBookIsOnLoanAndReturnedUndamagedShouldBeAvailable()
        {
            var book = new Book("author", "title", "call number", 1);

            var loan = Substitute.For<ILoan>();

            book.Borrow(loan);

            book.ReturnBook(false);

            Assert.Equal(BookState.AVAILABLE, book.State);
        }

        [Fact]
        public void WhenBookIsOnLoanAndReturnedDamagedShouldBeDamaged()
        {
            var book = new Book("author", "title", "call number", 1);

            var loan = Substitute.For<ILoan>();

            book.Borrow(loan);

            book.ReturnBook(true);

            Assert.Equal(BookState.DAMAGED, book.State);
        }
    }
}
