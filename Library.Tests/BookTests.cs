using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Entities;
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
    }
}
