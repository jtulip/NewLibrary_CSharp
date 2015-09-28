using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Entities;
using Library.Interfaces.Entities;
using Xunit;

namespace Library.Tests.UnitTests
{
    [Trait("Category", "Book Tests")]
    public class BookStateTests
    {
        public void WhenBookIsCreatedShouldBeAvailable()
        {
            var book = new Book("author", "title", "call number", 1);

            Assert.Equal(BookState.AVAILABLE, book.State);
        }
    }
}
