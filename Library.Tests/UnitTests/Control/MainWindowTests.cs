using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Controllers;
using Library.Controls;
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

        [WpfFact]
        public void WhenMainWindowCreatedMainControllerGetsMadeDisplay()
        {
            var mainWindow = new MainWindow();

            Assert.True(mainWindow.Display is MainMenuControl);
        }

        [WpfFact]
        public void WhenMainWindowCreatedMainMenuControllerParametersStoredLocally()
        {
            var mainWindow = new MainWindow();

            Assert.NotNull(mainWindow._reader);
            Assert.NotNull(mainWindow._scanner);
            Assert.NotNull(mainWindow._printer);
            Assert.NotNull(mainWindow._bookDAO);
            Assert.NotNull(mainWindow._loanDAO);
            Assert.NotNull(mainWindow._memberDAO);

            var listener = (MainMenuController)((MainMenuControl) mainWindow.Display)._listener;

            Assert.Equal(mainWindow._reader, listener._reader);
            Assert.Equal(mainWindow._scanner, listener._scanner);
            Assert.Equal(mainWindow._printer, listener._printer);
            Assert.Equal(mainWindow._bookDAO, listener._bookDAO);
            Assert.Equal(mainWindow._loanDAO, listener._loanDAO);
            Assert.Equal(mainWindow._memberDAO, listener._memberDAO);
        }
    }
}
