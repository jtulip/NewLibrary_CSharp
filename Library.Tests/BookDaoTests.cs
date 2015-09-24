using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Library.Daos;
using Library.Interfaces.Entities;
using NSubstitute;
using Xunit;

namespace Library.Tests
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
    }
}
