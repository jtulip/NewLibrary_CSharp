using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    public class ControlTests : IDisposable
    {
        public ControlTests()
        {
            _display = Substitute.For<IDisplay>();
            _reader = Substitute.For<ICardReader>();
            _scanner = Substitute.For<IScanner>();
            _printer = Substitute.For<IPrinter>();
            _bookDao = Substitute.For<IBookDAO>();
            _loanDao = Substitute.For<ILoanDAO>();
            _memberDao = Substitute.For<IMemberDAO>();
        }

        private IDisplay _display;
        private ICardReader _reader;
        private IScanner _scanner;
        private IPrinter _printer;
        private IBookDAO _bookDao;
        private ILoanDAO _loanDao;
        private IMemberDAO _memberDao;

        [Fact]
        public void CanCreateControl()
        {
            // Must be done on an STA thread for WPF
            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new Action(() =>
                {
                    var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

                    Assert.NotNull(ctrl);
                }));
        }

        public void Dispose()
        {
            _display = null;
            _reader = null;
            _scanner = null;
            _printer = null;
            _bookDao = null;
            _loanDao = null;
            _memberDao = null;
        }
    }
}
