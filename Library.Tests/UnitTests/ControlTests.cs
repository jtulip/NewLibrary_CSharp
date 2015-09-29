using System;
using System.CodeDom;
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

        [Fact]
        public void BBUC_OP1_BeginUseCase()
        {
            // Must be done on an STA thread for WPF
            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new Action(() =>
                {
                    var mockThis = Substitute.For<IBorrowListener>();

                    var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

                    _reader.Received().Listener = ctrl;
                    _scanner.Received().Listener = ctrl;

                    Assert.Equal(ctrl, _reader.Listener);
                    Assert.Equal(ctrl, _scanner.Listener);

                    Assert.Equal(EBorrowState.CREATED, ctrl._state);

                    ctrl._ui = new BorrowControl(mockThis);

                    ctrl.initialise();

                    Assert.Equal(mockThis, ((BorrowControl)ctrl._ui)._listener);
                    Assert.Equal(4, ((BorrowControl)ctrl._ui)._controlDict.Count);

                    var swipeCardPanel = ((BorrowControl) ctrl._ui)._controlDict.Values.Single(c => c is SwipeCardControl);

                    Assert.True(((SwipeCardControl) swipeCardPanel).IsVisible);
                    Assert.True(((SwipeCardControl)swipeCardPanel).cancelButton.IsVisible);

                    Assert.True(_reader.Enabled);
                    Assert.False(_scanner.Enabled);

                    Assert.Equal(EBorrowState.INITIALIZED, ctrl._state);
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
