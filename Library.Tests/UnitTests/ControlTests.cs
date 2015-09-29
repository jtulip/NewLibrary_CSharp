using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Library.Controllers.Borrow;
using Library.Controls.Borrow;
using Library.Interfaces.Controllers.Borrow;
using Library.Interfaces.Daos;
using Library.Interfaces.Hardware;
using NSubstitute;
using Xunit;

namespace Library.Tests.UnitTests
{
    [Trait("Category", "Control Tests")]
    public class ControlTests
    {
        [Fact]
        public void CanCreateControl()
        {
            // Must be done on an STA thread for WPF
            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new Action(() =>
                {
                    var display = Substitute.For<IDisplay>();
                    var reader = Substitute.For<ICardReader>();
                    var scanner = Substitute.For<IScanner>();
                    var printer = Substitute.For<IPrinter>();
                    var bookDao = Substitute.For<IBookDAO>();
                    var loanDao = Substitute.For<ILoanDAO>();
                    var memberDao = Substitute.For<IMemberDAO>();

                    var ctrl = new BorrowController(display, reader, scanner, printer, bookDao, loanDao, memberDao);

                    Assert.NotNull(ctrl);
                }));
        }

    }
}
