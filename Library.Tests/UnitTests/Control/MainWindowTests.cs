using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Library.Tests.UnitTests.Control
{
    [Trait("Category", "BorrowControl Tests")]
    public class MainWindowTests
    {
        [WpfFact]
        public void CanCreateMainWindow()
        {
            var mainWindow = new MainWindow();

            Assert.NotNull(mainWindow);
        }
    }
}
