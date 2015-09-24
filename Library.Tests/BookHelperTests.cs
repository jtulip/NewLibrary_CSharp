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
    public class BookHelperTests
    {
        [Fact]
        public void CanCreateBookHelper()
        {
            var helper = new BookHelper();

            Assert.NotNull(helper);
        }

        [Fact]
        public void BookHelperImplementsIBookHelperInterface()
        {
            var helper = new BookHelper();

            Assert.IsAssignableFrom<IBookHelper>(helper);

            var typedMember = helper as IBookHelper;

            Assert.NotNull(typedMember);
        }
    }
}
