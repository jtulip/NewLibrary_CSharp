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
using Library.Entities;
using Library.Interfaces.Controllers.Borrow;
using Library.Interfaces.Daos;
using Library.Interfaces.Entities;
using Library.Interfaces.Hardware;
using NSubstitute;
using NSubstitute.Core;
using Xunit;

namespace Library.Tests.UnitTests
{
    [Trait("Category", "BrowserControl Tests")]
    public class BrowserControlTests : IDisposable
    {
        public BrowserControlTests()
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

        [WpfFact]
        public void CanCreateControl()
        {
            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            Assert.NotNull(ctrl);
        }

        [WpfFact]
        public void BBUC_OP1_BeginUseCase()
        {
            // Must be done on an STA thread for WPF
            var mockThis = Substitute.For<IBorrowListener>();

            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            _reader.Received().Listener = ctrl;
            _scanner.Received().Listener = ctrl;

            Assert.Equal(ctrl, _reader.Listener);
            Assert.Equal(ctrl, _scanner.Listener);

            Assert.Equal(EBorrowState.CREATED, ctrl._state);

            //ctrl._ui = new BorrowControl(mockThis);

            ctrl.initialise();

            //var borrowControl = (BorrowControl)ctrl._ui;

            //Assert.Equal(mockThis, borrowControl._listener);
            //Assert.Equal(4, borrowControl._controlDict.Count);

            //var swipeCardPanel = borrowControl._controlDict.Values.Single(c => c is SwipeCardControl);

            //Assert.Equal(swipeCardPanel, borrowControl._currentControl);
            //Assert.True(((SwipeCardControl)swipeCardPanel).IsEnabled);
            //Assert.True(((SwipeCardControl)swipeCardPanel).cancelButton.IsEnabled);

            Assert.True(_reader.Enabled);
            Assert.False(_scanner.Enabled);

            Assert.Equal(EBorrowState.INITIALIZED, ctrl._state);
        }

        [WpfFact]
        public void BBUC_OP2_SwipeBorrowerCard()
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

            //var borrowControl = (BorrowControl)ctrl._ui;

            //Assert.Equal(mockThis, borrowControl._listener);
            //Assert.Equal(4, borrowControl._controlDict.Count);

            //var swipeCardPanel = borrowControl._controlDict.Values.Single(c => c is SwipeCardControl);
            //var scanBookPanel = borrowControl._controlDict.Values.Single(c => c is ScanBookControl);

            //Assert.Equal(swipeCardPanel, borrowControl._currentControl);
            //Assert.True(((SwipeCardControl)swipeCardPanel).IsEnabled);
            //Assert.True(((SwipeCardControl)swipeCardPanel).cancelButton.IsEnabled);

            Assert.True(_reader.Enabled);
            Assert.False(_scanner.Enabled);

            Assert.Equal(EBorrowState.INITIALIZED, ctrl._state);

            var member = Substitute.For<IMember>();

            _memberDao.GetMemberByID(1).Returns(member);

            ctrl.cardSwiped(1);

            //_memberDao.Received().GetMemberByID(1);

            //Assert.False(_reader.Enabled);
            //Assert.True(_scanner.Enabled);

            ////Assert.False(((SwipeCardControl)swipeCardPanel).IsEnabled);
            ////Assert.True(((ScanBookControl)scanBookPanel).IsEnabled);
            ////Assert.True(((ScanBookControl)scanBookPanel).cancelButton.IsEnabled);
            ////Assert.True(((ScanBookControl)scanBookPanel).completeButton.IsEnabled);
            ////Assert.Equal(3, ((ScanBookControl)scanBookPanel).);
            //Assert.Equal(EBorrowState.SCANNING_BOOKS, ctrl._state);
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
