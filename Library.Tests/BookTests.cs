using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Entities;
using Library.Interfaces.Entities;
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

    }
}
