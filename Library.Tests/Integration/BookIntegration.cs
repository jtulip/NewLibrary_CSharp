using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Daos;
using Library.Entities;
using Library.Interfaces.Daos;
using Library.Interfaces.Entities;
using Xunit;

namespace Library.Tests.Integration
{
    [Trait("Category", "Book Tests")]
    public class BookIntegration
    {
        [Fact]
        public void CanCreateBook()
        {
            IBookHelper helper = new BookHelper();

            var dao = new BookDao(helper);

            var author = "author";
            var title = "title";
            var callNumber = "call number";

            var book = dao.AddBook(author, title, callNumber);

            Assert.NotNull(book);

            Assert.NotEmpty(dao.BookList);
            Assert.Equal(1, dao.BookList.Count);
            Assert.Equal(book, dao.BookList[0]);

            Assert.Equal(author, book.Author);
            Assert.Equal(title, book.Title);
            Assert.Equal(callNumber, book.CallNumber);

            Assert.NotEqual(0, book.ID);
        }

        [Fact]
        public void CreateBookFailsOnNullHelper()
        {
            IBookHelper helper = null;
            Assert.Throws<ArgumentException>(() => { IBookDAO dao = new BookDao(helper); });
        }

        [Fact]
        public void CreateBookFailsOnIllegalArguments()
        {
            IBookHelper helper = new BookHelper();
            IBookDAO dao = new BookDao(helper);

            var author = "author";
            var title = "title";
            var callNumber = "call number";

            Assert.Throws<ArgumentException>(
                () => { var book = dao.AddBook(null, title, callNumber); });

            Assert.Throws<ArgumentException>(
                () => { var book = dao.AddBook(author, null, callNumber); });

            Assert.Throws<ArgumentException>(
                () => { var book = dao.AddBook(author, title, null); });
        }

        [Fact]
        public void CreateMemberCreatesAUniqueId()
        {
            IBookHelper helper = new BookHelper();
            IBookDAO dao = new BookDao(helper);

            var author = "author";
            var title = "title";
            var callNumber = "call number";

            var book = dao.AddBook(author, title, callNumber);

            Assert.NotEqual(0, book.ID);
        }

        [Fact]
        public void CanGetBookById()
        {
            IBookHelper helper = new BookHelper();
            IBookDAO dao = new BookDao(helper);

            var author = "author";
            var title = "title";
            var callNumber = "call number";

            var book = dao.AddBook(author, title, callNumber);

            for (int i = 0; i < 10; i++)
            {
                dao.AddBook("Test", "Test", "Test");
            }

            var result = dao.GetBookByID(book.ID);

            Assert.Equal(book, result);
        }

        [Fact]
        public void GetBookByIdReturnsNullIfNotFound()
        {
            IBookHelper helper = new BookHelper();
            IBookDAO dao = new BookDao(helper);

            var author = "author";
            var title = "title";
            var callNumber = "call number";

            var book = dao.AddBook(author, title, callNumber);

            for (int i = 0; i < 10; i++)
            {
                dao.AddBook("Test", "Test", "Test");
            }

            var result = dao.GetBookByID(1000);

            Assert.Null(result);
        }

        [Fact]
        public void CanGetBookByAuthor()
        {
            IBookHelper helper = new BookHelper();
            IBookDAO dao = new BookDao(helper);

            var author = "author";
            var title = "title";
            var callNumber = "call number";

            var book = dao.AddBook(author, title, callNumber);

            for (int i = 0; i < 10; i++)
            {
                dao.AddBook("Test", "Test", "Test");
            }

            var result = dao.FindBooksByAuthor(book.Author);

            var single = result.Single();

            Assert.Equal(book, single);
        }

        [Fact]
        public void GetBookByAuthorReturnsEmptyCollectionIfNotFound()
        {
            IBookHelper helper = new BookHelper();
            IBookDAO dao = new BookDao(helper);

            var author = "author";
            var title = "title";
            var callNumber = "call number";

            var book = dao.AddBook(author, title, callNumber);

            for (int i = 0; i < 10; i++)
            {
                dao.AddBook("Test", "Test", "Test");
            }

            var result = dao.FindBooksByAuthor("Jim Tulip");

            Assert.Empty(result);
        }

        [Fact]
        public void CanGetBookByTitle()
        {
            IBookHelper helper = new BookHelper();
            IBookDAO dao = new BookDao(helper);

            var author = "author";
            var title = "title";
            var callNumber = "call number";

            var book = dao.AddBook(author, title, callNumber);

            for (int i = 0; i < 10; i++)
            {
                dao.AddBook("Test", "Test", "Test");
            }

            var result = dao.FindBooksByTitle(book.Title);

            var single = result.Single();

            Assert.Equal(book, single);
        }

        [Fact]
        public void GetBookByTitleReturnsEmptyCollectionIfNotFound()
        {
            IBookHelper helper = new BookHelper();
            IBookDAO dao = new BookDao(helper);

            var author = "author";
            var title = "title";
            var callNumber = "call number";

            var book = dao.AddBook(author, title, callNumber);

            for (int i = 0; i < 10; i++)
            {
                dao.AddBook("Test", "Test", "Test");
            }

            var result = dao.FindBooksByAuthor("Adventures in Programming");

            Assert.Empty(result);
        }
    }
}
