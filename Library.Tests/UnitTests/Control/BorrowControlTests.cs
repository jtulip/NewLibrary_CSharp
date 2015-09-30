using System;
using Library.Controllers.Borrow;
using Library.Controls.Borrow;
using Library.Interfaces.Controllers.Borrow;
using Library.Interfaces.Daos;
using Library.Interfaces.Entities;
using Library.Interfaces.Hardware;
using NSubstitute;
using Xunit;

namespace Library.Tests.UnitTests.Control
{
    [Trait("Category", "BorrowControl Tests")]
    public class BorrowControlTests : IDisposable
    {
        public BorrowControlTests()
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
        public void CreateControlThrowsExceptionOnNullArguments()
        {
            var ex = Assert.Throws<ArgumentException>(
                () =>
                {
                    var ctrl = new BorrowController(null, _reader, _scanner, _printer, _bookDao, _loanDao,
                        _memberDao);
                });

            Assert.Equal("Display object was not provided to begin the application", ex.Message);

            ex = Assert.Throws<ArgumentException>(
                () =>
                {
                    var ctrl = new BorrowController(_display, null, _scanner, _printer, _bookDao, _loanDao,
                        _memberDao);
                });

            Assert.Equal("Reader object was not provided to begin the application", ex.Message);

            ex = Assert.Throws<ArgumentException>(
                () =>
                {
                    var ctrl = new BorrowController(_display, _reader, null, _printer, _bookDao, _loanDao,
                        _memberDao);
                });

            Assert.Equal("Scanner object was not provided to begin the application", ex.Message);

            ex = Assert.Throws<ArgumentException>(
                () =>
                {
                    var ctrl = new BorrowController(_display, _reader, _scanner, null, _bookDao, _loanDao,
                        _memberDao);
                });

            Assert.Equal("Printer object was not provided to begin the application", ex.Message);

            ex = Assert.Throws<ArgumentException>(
                () =>
                {
                    var ctrl = new BorrowController(_display, _reader, _scanner, _printer, null, _loanDao,
                        _memberDao);
                });

            Assert.Equal("BookDAO object was not provided to begin the application", ex.Message);

            ex = Assert.Throws<ArgumentException>(
                () =>
                {
                    var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, null,
                        _memberDao);
                });

            Assert.Equal("LoanDAO object was not provided to begin the application", ex.Message);

            ex = Assert.Throws<ArgumentException>(
                () =>
                {
                    var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao,
                        null);
                });

            Assert.Equal("MemberDAO object was not provided to begin the application", ex.Message);
        }

        [WpfFact]
        public void CreateControlAssignsArgumentsToLocalProperties()
        {
            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            Assert.Equal(_display, ctrl._display);
            Assert.Equal(_reader, ctrl._reader);
            Assert.Equal(_scanner, ctrl._scanner);
            Assert.Equal(_printer, ctrl._printer);
            Assert.Equal(_bookDao, ctrl._bookDAO);
            Assert.Equal(_loanDao, ctrl._loanDAO);
            Assert.Equal(_memberDao, ctrl._memberDAO);
        }

        [WpfFact]
        public void CreateControlBookControlAddedAsListenerToCardReaderAndScanner()
        {
            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            _reader.Received().Listener = ctrl;
            _scanner.Received().Listener = ctrl;

            Assert.Equal(ctrl, _reader.Listener);
            Assert.Equal(ctrl, _scanner.Listener);
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
