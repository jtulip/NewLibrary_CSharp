using System;
using System.Collections.Generic;
using System.Linq;
using Library.Daos;
using Library.Entities;
using Library.Interfaces.Daos;
using Library.Interfaces.Entities;
using NSubstitute;
using Xunit;

namespace Library.Tests.UnitTests.Dao
{
    [Trait("Category", "Book Tests")]
    public class BookDaoTests
    {
        [Fact]
        public void CanCreateBookDao()
        {
            var helper = Substitute.For<IBookHelper>();

            var bookDao = new BookDao(helper);

            Assert.NotNull(bookDao);
        }

        [Fact]
        public void CreateBookDaoThrowsExceptionOnNullParameter()
        {
            var ex = Assert.Throws<ArgumentException>(() => { var bookDao = new BookDao(null); });

            Assert.Equal("Helper must be provided when creating BookDao", ex.Message);
        }

        [Fact]
        public void BookDaoImplementsIBookDAOInterface()
        {
            var helper = Substitute.For<IBookHelper>();

            var bookDao = new BookDao(helper);

            Assert.IsAssignableFrom<IBookDAO>(bookDao);

            var typedMember = bookDao as IBookDAO;

            Assert.NotNull(typedMember);
        }

        [Fact]
        public void CanAddBook()
        {
            var helper = Substitute.For<IBookHelper>();

            var bookDao = new BookDao(helper);

            var author = "author";
            var title = "title";
            var callNo = "callNo";

            // Uses Helper to create a new book with a unique book ID.
            // Adds the book to a collection of books and returns new book.
            Assert.Equal(0, bookDao.BookList.Count);

            // Tell the mock what to return when it is called.
            helper.MakeBook(author, title, callNo, Arg.Any<int>()).Returns(new Book(author, title, callNo, 1));

            var result = bookDao.AddBook(author, title, callNo);

            // Assert that the mock's MakeBook method was called.
            helper.Received().MakeBook(author, title, callNo, Arg.Any<int>()); 

            Assert.NotNull(result);
            Assert.Equal(author, result.Author);
            Assert.Equal(title, result.Title);
            Assert.Equal(callNo, result.CallNumber);

            Assert.Equal(1, bookDao.BookList.Count);

            var book = bookDao.BookList[0];

            Assert.Equal(book, result);
        }

        [Fact]
        public void AddBookAssignsUniqueID()
        {
            var helper = Substitute.For<IBookHelper>();

            var bookDao = new BookDao(helper);

            var author = "author";
            var title = "title";
            var callNo = "callNo";

            // Make sure the id increments as books are added.
            for (var id = 1; id < 10; id++)
            {
                helper.MakeBook(author, title, callNo, id).Returns(new Book(author, title, callNo, id));

                var result = bookDao.AddBook(author, title, callNo);

                // Assert that the mock's MakeBook method was called.
                helper.Received().MakeBook(author, title, callNo, id);

                // Make sure the id of the book is new.
                Assert.Equal(id, result.ID);
            }
        }

        [Fact]
        public void CanGetBookById()
        {
            var helper = Substitute.For<IBookHelper>();

            var bookDao = new BookDao(helper);

            var author = "author";
            var title = "title";
            var callNo = "callNo";

            bookDao.BookList = new List<IBook>
            {
                new Book("one", "two", "three", 1),
                new Book(author, title, callNo, 2),
                new Book("one", "two", "three", 3),
            };

            var book = bookDao.GetBookByID(2);

            Assert.NotNull(book);

            Assert.Equal(2, book.ID);
            Assert.Equal(author, book.Author);
            Assert.Equal(title, book.Title);
            Assert.Equal(callNo, book.CallNumber);
        }

        [Fact]
        public void GetBookByIdReturnsNullIfNotFound()
        {
            var helper = Substitute.For<IBookHelper>();

            var bookDao = new BookDao(helper);


            bookDao.BookList = new List<IBook>
            {
                new Book("one", "two", "three", 1),
                new Book("one", "two", "three", 3),
            };

            var book = bookDao.GetBookByID(2);

            Assert.Null(book);
        }

        [Fact]
        public void CanGetBookByAuthor()
        {
            var helper = Substitute.For<IBookHelper>();

            var bookDao = new BookDao(helper);

            var author = "author";
            var title = "title";
            var callNo = "callNo";

            bookDao.BookList = new List<IBook>
            {
                new Book("one", "two", "three", 1),
                new Book(author, title, callNo, 2),
                new Book("one", "two", "three", 3),
            };

            var book = bookDao.FindBooksByAuthor(author).Single();

            Assert.NotNull(book);

            Assert.Equal(2, book.ID);
            Assert.Equal(author, book.Author);
            Assert.Equal(title, book.Title);
            Assert.Equal(callNo, book.CallNumber);
        }

        [Fact]
        public void GetBookByAuthorReturnsEmptyList()
        {
            var helper = Substitute.For<IBookHelper>();

            var bookDao = new BookDao(helper);

            var author = "author";
            var title = "title";
            var callNo = "callNo";

            bookDao.BookList = new List<IBook>
            {
                new Book("one", "two", "three", 1),
                new Book(author, title, callNo, 2),
                new Book("one", "two", "three", 3),
            };

            var list = bookDao.FindBooksByAuthor("Dahl");

            Assert.NotNull(list);
            Assert.Empty(list);
        }

        [Fact]
        public void CanGetBookByTitle()
        {
            var helper = Substitute.For<IBookHelper>();

            var bookDao = new BookDao(helper);

            var author = "author";
            var title = "title";
            var callNo = "callNo";

            bookDao.BookList = new List<IBook>
            {
                new Book("one", "two", "three", 1),
                new Book(author, title, callNo, 2),
                new Book("one", "two", "three", 3),
            };

            var book = bookDao.FindBooksByTitle(title).Single();

            Assert.NotNull(book);

            Assert.Equal(2, book.ID);
            Assert.Equal(author, book.Author);
            Assert.Equal(title, book.Title);
            Assert.Equal(callNo, book.CallNumber);
        }

        [Fact]
        public void GetBookByTitleReturnsEmptyList()
        {
            var helper = Substitute.For<IBookHelper>();

            var bookDao = new BookDao(helper);

            var author = "author";
            var title = "title";
            var callNo = "callNo";

            bookDao.BookList = new List<IBook>
            {
                new Book("one", "two", "three", 1),
                new Book(author, title, callNo, 2),
                new Book("one", "two", "three", 3),
            };

            var list = bookDao.FindBooksByTitle("The Twits");

            Assert.NotNull(list);
            Assert.Empty(list);
        }

        [Fact]
        public void CanGetBookByAuthorAndTitle()
        {
            var helper = Substitute.For<IBookHelper>();

            var bookDao = new BookDao(helper);

            var author = "author";
            var title = "title";
            var callNo = "callNo";

            bookDao.BookList = new List<IBook>
            {
                new Book("one", "two", "three", 1),
                new Book(author, title, callNo, 2),
                new Book("one", "two", "three", 3),
                new Book(author, "two", callNo, 4),
            };

            var book = bookDao.FindBooksByAuthorTitle(author, title).Single();

            Assert.NotNull(book);

            Assert.Equal(2, book.ID);
            Assert.Equal(author, book.Author);
            Assert.Equal(title, book.Title);
            Assert.Equal(callNo, book.CallNumber);
        }

        [Fact]
        public void GetBookByAuthorAndTitleReturnsEmptyList()
        {
            var helper = Substitute.For<IBookHelper>();

            var bookDao = new BookDao(helper);

            var author = "author";
            var title = "title";
            var callNo = "callNo";

            bookDao.BookList = new List<IBook>
            {
                new Book("one", "two", "three", 1),
                new Book(author, title, callNo, 2),
                new Book("one", "two", "three", 3),
                new Book(author, "two", callNo, 4),
            };

            var list = bookDao.FindBooksByAuthorTitle("Dahl", "The Twits");

            Assert.NotNull(list);
            Assert.Empty(list);
        }
    }
}
