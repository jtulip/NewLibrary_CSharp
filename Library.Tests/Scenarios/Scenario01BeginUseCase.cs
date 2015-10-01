using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Controllers.Borrow;
using Library.Controls.Borrow;
using Library.Daos;
using Library.Entities;
using Library.Hardware;
using Library.Interfaces.Controllers.Borrow;
using Xunit;

namespace Library.Tests.Scenarios
{
    [Trait("Category", "Scenarios")]

    public class Scenario01BeginUseCase
    {
        [WpfFact]
        public void RunScenario()
        {
           var bookDao = new BookDao(new BookHelper());
           var loanDao = new LoanDao(new LoanHelper());
           var memberDao = new MemberDao(new MemberHelper());

            var display = new MainWindow();
            var reader = new CardReader();
            var scanner = new Scanner();
            var printer = new Printer();

            var controller = new BorrowController(display, reader, scanner, printer,
                bookDao, loanDao, memberDao);

            // Test pre-conditions
            Assert.NotNull(controller);

            Assert.NotNull(bookDao);
            Assert.NotNull(loanDao);
            Assert.NotNull(memberDao);
            Assert.NotNull(display);
            Assert.NotNull(reader);
            Assert.NotNull(scanner);
            Assert.NotNull(printer);

            Assert.Equal(controller._bookDAO, bookDao);
            Assert.Equal(controller._loanDAO, loanDao);
            Assert.Equal(controller._memberDAO, memberDao);
            Assert.Equal(controller._display, display);
            Assert.Equal(controller._reader, reader);
            Assert.Equal(controller._scanner, scanner);
            Assert.Equal(controller._printer, printer);

            Assert.Equal(controller._reader.Listener, controller);
            Assert.Equal(controller._scanner.Listener, controller);

            Assert.Equal(EBorrowState.CREATED, controller._state);

            // Run use case
            controller.initialise();

            // Test post-conditions
            // Borrow book UI Displayed
            Assert.True(display.Display.IsEnabled);

            var borrowCtrl = ((BorrowControl) display.Display);
            var swipeCtrl = borrowCtrl._controlDict.Single(c => c.Value is SwipeCardControl).Value as SwipeCardControl;

            Assert.NotNull(swipeCtrl);
            Assert.True(swipeCtrl.IsEnabled);
            Assert.True(swipeCtrl.cancelButton.IsEnabled);

            Assert.True(reader.Enabled);
            Assert.True(!scanner.Enabled);
            Assert.Equal(EBorrowState.INITIALIZED, controller._state);
        }
    }
}
