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
    }
}
