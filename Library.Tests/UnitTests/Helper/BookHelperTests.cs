using Library.Entities;
using Library.Interfaces.Entities;
using Xunit;

namespace Library.Tests.UnitTests.Helper
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

        [Fact]
        public void CanMakeBook()
        {
            var author = "author";
            var title = "title";
            var callNumber = "callNumber";
            var id = 5;

            var helper = new BookHelper();

            var book = helper.MakeBook(author, title, callNumber, id);

            Assert.NotNull(book);
            Assert.Equal(id, book.ID);
            Assert.Equal(author, book.Author);
            Assert.Equal(title, book.Title);
            Assert.Equal(callNumber, book.CallNumber);
        }
    }
}
