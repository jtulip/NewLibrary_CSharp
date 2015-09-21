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
            var book = new Book();

            Assert.NotNull(book);
        }

        [Fact]
        public void BookImplementsIBookInterface()
        {
            var book = new Book();

            Assert.IsAssignableFrom<IBook>(book);

            var typedMember = book as IBook;

            Assert.NotNull(typedMember);
        }

    }
}
