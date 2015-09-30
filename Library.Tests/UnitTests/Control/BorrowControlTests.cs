using System;
using System.Collections.Generic;
using Library.Controllers.Borrow;
using Library.Controls.Borrow;
using Library.Entities;
using Library.Interfaces.Controllers.Borrow;
using Library.Interfaces.Controls.Borrow;
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
        public void CreateControlSetsStateToCreated()
        {
            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            Assert.Equal(EBorrowState.CREATED, ctrl._state);
        }

        [WpfFact]
        public void CanInitialiseBorrowerController()
        {
            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            ctrl.initialise(); // If test does not fail to this point it hasn't thrown, so Initialise method has worked.
        }

        [WpfFact]
        public void InitialiseControllerEnablesCardReader()
        {
            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            ctrl.initialise();

            Assert.True(ctrl._reader.Enabled);
        }

        [WpfFact]
        public void InitialiseControllerDisablesScanner()
        {
            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            ctrl.initialise();

            Assert.True(!ctrl._scanner.Enabled);
        }

        [WpfFact]
        public void InitialiseControllerSetsStateToInitialized()
        {
            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            ctrl.initialise();

            Assert.Equal(EBorrowState.INITIALIZED, ctrl._state);
        }

        [WpfFact]
        public void CanSwipeBorrowerCard()
        {
            var member = Substitute.For<IMember>();
            member.HasOverDueLoans.Returns(false);
            member.HasReachedLoanLimit.Returns(false);
            member.HasReachedFineLimit.Returns(false);
            member.FineAmount.Returns(0.00f);
            member.ID.Returns(1);
            member.FirstName.Returns("Jim");
            member.LastName.Returns("Tulip");
            member.ContactPhone.Returns("Phone");
            member.Loans.Returns(new List<ILoan>());

            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            ctrl.initialise();

            //Test pre-conditions
            Assert.True(ctrl._reader.Enabled);
            Assert.Equal(ctrl, ctrl._reader.Listener);
            Assert.NotNull(ctrl._memberDAO);
            Assert.Equal(EBorrowState.INITIALIZED, ctrl._state);

            _memberDao.GetMemberByID(member.ID).Returns(member);

            ctrl.cardSwiped(member.ID); // If we get to the end of the method then it hasn't thrown an exception.
        }

        [WpfFact]
        public void SwipeBorrowerCardMemberDoesntExist()
        {
            var memberId = 1;

            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            // Set the UI to the mock so we can test
            var borrowctrl = Substitute.For<ABorrowControl>();
            ctrl._ui = borrowctrl;

            ctrl.initialise();

            //Test pre-conditions
            Assert.True(ctrl._reader.Enabled);
            Assert.Equal(ctrl, ctrl._reader.Listener);
            Assert.NotNull(ctrl._memberDAO);
            Assert.Equal(EBorrowState.INITIALIZED, ctrl._state);

            _memberDao.GetMemberByID(memberId).Returns((Member) null);

            ctrl.cardSwiped(memberId);

            _memberDao.Received().GetMemberByID(memberId);

            // Test using mocks that it received a Borrower not found error.
            borrowctrl.Received().DisplayErrorMessage("Borrower was not found in database");
        }

        [WpfFact]
        public void SwipeBorrowerCardShowErrorIfMemberHasOverdueLoans()        
        {
            var memberId = 1;
            var member = Substitute.For<IMember>();
            member.HasOverDueLoans.Returns(true);
            member.HasReachedLoanLimit.Returns(false);
            member.HasReachedFineLimit.Returns(false);
            member.Loans.Returns(new List<ILoan>());

            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            // Set the UI to the mock so we can test
            var borrowctrl = Substitute.For<ABorrowControl>();
            ctrl._ui = borrowctrl;

            ctrl.initialise();

            //Test pre-conditions
            Assert.True(ctrl._reader.Enabled);
            Assert.Equal(ctrl, ctrl._reader.Listener);
            Assert.NotNull(ctrl._memberDAO);
            Assert.Equal(EBorrowState.INITIALIZED, ctrl._state);

            _memberDao.GetMemberByID(memberId).Returns(member);

            ctrl.cardSwiped(memberId);

            _memberDao.Received().GetMemberByID(memberId);

            borrowctrl.Received().DisplayOverDueMessage();
        }

        [WpfFact]
        public void SwipeBorrowerCardShowErrorIfMemberHasReachedLoanLimit()
        {
            var memberId = 1;
            var member = Substitute.For<IMember>();
            member.HasOverDueLoans.Returns(false);
            member.HasReachedLoanLimit.Returns(true);
            member.HasReachedFineLimit.Returns(false);
            member.Loans.Returns(new List<ILoan>());

            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            // Set the UI to the mock so we can test
            var borrowctrl = Substitute.For<ABorrowControl>();
            ctrl._ui = borrowctrl;

            ctrl.initialise();

            //Test pre-conditions
            Assert.True(ctrl._reader.Enabled);
            Assert.Equal(ctrl, ctrl._reader.Listener);
            Assert.NotNull(ctrl._memberDAO);
            Assert.Equal(EBorrowState.INITIALIZED, ctrl._state);

            _memberDao.GetMemberByID(memberId).Returns(member);

            ctrl.cardSwiped(memberId);

            _memberDao.Received().GetMemberByID(memberId);

            borrowctrl.Received().DisplayAtLoanLimitMessage();
        }

        [WpfFact]
        public void SwipeBorrowerCardShowErrorIfMemberHasReachedFinesLimit()
        {
            var memberId = 1;
            var member = Substitute.For<IMember>();
            member.HasOverDueLoans.Returns(false);
            member.HasReachedLoanLimit.Returns(false);
            member.HasReachedFineLimit.Returns(true);
            member.FineAmount.Returns(100.00f);
            member.Loans.Returns(new List<ILoan>());

            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            // Set the UI to the mock so we can test
            var borrowctrl = Substitute.For<ABorrowControl>();
            ctrl._ui = borrowctrl;

            ctrl.initialise();

            //Test pre-conditions
            Assert.True(ctrl._reader.Enabled);
            Assert.Equal(ctrl, ctrl._reader.Listener);
            Assert.NotNull(ctrl._memberDAO);
            Assert.Equal(EBorrowState.INITIALIZED, ctrl._state);

            _memberDao.GetMemberByID(memberId).Returns(member);

            ctrl.cardSwiped(memberId);

            _memberDao.Received().GetMemberByID(memberId);

            borrowctrl.Received().DisplayOverFineLimitMessage(member.FineAmount);
        }

        [WpfFact]
        public void SwipeBorrowerCardShowsCurrentLoans()
        {
            var loan = Substitute.For<ILoan>();

            var member = Substitute.For<IMember>();
            member.HasOverDueLoans.Returns(false);
            member.HasReachedLoanLimit.Returns(false);
            member.HasReachedFineLimit.Returns(false);
            member.FineAmount.Returns(0.00f);
            member.ID.Returns(1);
            member.FirstName.Returns("Jim");
            member.LastName.Returns("Tulip");
            member.ContactPhone.Returns("Phone");
            member.Loans.Returns(new List<ILoan>() { loan, loan, loan });

            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            // Set the UI to the mock so we can test
            var borrowctrl = Substitute.For<ABorrowControl>();
            ctrl._ui = borrowctrl;

            ctrl.initialise();

            //Test pre-conditions
            Assert.True(ctrl._reader.Enabled);
            Assert.Equal(ctrl, ctrl._reader.Listener);
            Assert.NotNull(ctrl._memberDAO);
            Assert.Equal(EBorrowState.INITIALIZED, ctrl._state);

            _memberDao.GetMemberByID(member.ID).Returns(member);

            ctrl.cardSwiped(member.ID);

            _memberDao.Received().GetMemberByID(member.ID);

            foreach (var l in member.Loans)
            {
                borrowctrl.Received().DisplayExistingLoan(l.ToString());
            }
        }



        [WpfFact]
        public void SwipeBorrowerCardNotRestricted()
        {
            var member = Substitute.For<IMember>();
            member.HasOverDueLoans.Returns(false);
            member.HasReachedLoanLimit.Returns(false);
            member.HasReachedFineLimit.Returns(false);
            member.FineAmount.Returns(0.00f);
            member.ID.Returns(1);
            member.FirstName.Returns("Jim");
            member.LastName.Returns("Tulip");
            member.ContactPhone.Returns("Phone");
            member.Loans.Returns(new List<ILoan>());

            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            // Set the UI to the mock so we can test
            var borrowctrl = Substitute.For<ABorrowControl>();
            ctrl._ui = borrowctrl;

            ctrl.initialise();

            //Test pre-conditions
            Assert.True(ctrl._reader.Enabled);
            Assert.Equal(ctrl, ctrl._reader.Listener);
            Assert.NotNull(ctrl._memberDAO);
            Assert.Equal(EBorrowState.INITIALIZED, ctrl._state);

            _memberDao.GetMemberByID(member.ID).Returns(member);

            ctrl.cardSwiped(member.ID);

            _memberDao.Received().GetMemberByID(member.ID);

            _reader.Received().Enabled = false;
            _scanner.Received().Enabled = true;

            borrowctrl.Received().DisplayMemberDetails(member.ID, $"{member.FirstName} {member.LastName}" , member.ContactPhone);

            foreach (var l in member.Loans)
            {
                borrowctrl.Received().DisplayExistingLoan(l.ToString());
            }

            Assert.Equal(member, ctrl._borrower);
            Assert.True(!ctrl._reader.Enabled);
            Assert.True(ctrl._scanner.Enabled);
            Assert.Equal(EBorrowState.SCANNING_BOOKS, ctrl._state);
        }

        [WpfFact]
        public void SwipeBorrowerCardRestricted()
        {
            var member = Substitute.For<IMember>();
            member.HasOverDueLoans.Returns(true);
            member.HasReachedLoanLimit.Returns(false);
            member.HasReachedFineLimit.Returns(false);
            member.FineAmount.Returns(0.00f);
            member.ID.Returns(1);
            member.FirstName.Returns("Jim");
            member.LastName.Returns("Tulip");
            member.ContactPhone.Returns("Phone");
            member.Loans.Returns(new List<ILoan>());

            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            // Set the UI to the mock so we can test
            var borrowctrl = Substitute.For<ABorrowControl>();
            ctrl._ui = borrowctrl;

            ctrl.initialise();

            //Test pre-conditions
            Assert.True(ctrl._reader.Enabled);
            Assert.Equal(ctrl, ctrl._reader.Listener);
            Assert.NotNull(ctrl._memberDAO);
            Assert.Equal(EBorrowState.INITIALIZED, ctrl._state);

            _memberDao.GetMemberByID(member.ID).Returns(member);

            ctrl.cardSwiped(member.ID);

            _memberDao.Received().GetMemberByID(member.ID);

            _reader.Received().Enabled = false;
            _scanner.Received().Enabled = false;

            borrowctrl.Received().DisplayMemberDetails(member.ID, $"{member.FirstName} {member.LastName}", member.ContactPhone);

            borrowctrl.Received().DisplayErrorMessage("Member has been restricted from borrowing");

            foreach (var l in member.Loans)
            {
                borrowctrl.Received().DisplayExistingLoan(l.ToString());
            }

            Assert.Equal(member, ctrl._borrower);
            Assert.True(!ctrl._reader.Enabled);
            Assert.True(!ctrl._scanner.Enabled);
            Assert.Equal(EBorrowState.BORROWING_RESTRICTED, ctrl._state);
        }


        //[WpfFact]
        //public void BBUC_OP1_BeginUseCase()
        //{
        //    // Must be done on an STA thread for WPF
        //    var mockThis = Substitute.For<IBorrowListener>();

        //    var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

        //    _reader.Received().Listener = ctrl;
        //    _scanner.Received().Listener = ctrl;

        //    Assert.Equal(ctrl, _reader.Listener);
        //    Assert.Equal(ctrl, _scanner.Listener);

        //    Assert.Equal(EBorrowState.CREATED, ctrl._state);

        //    //ctrl._ui = new BorrowControl(mockThis);

        //    ctrl.initialise();

        //    //var borrowControl = (BorrowControl)ctrl._ui;

        //    //Assert.Equal(mockThis, borrowControl._listener);
        //    //Assert.Equal(4, borrowControl._controlDict.Count);

        //    //var swipeCardPanel = borrowControl._controlDict.Values.Single(c => c is SwipeCardControl);

        //    //Assert.Equal(swipeCardPanel, borrowControl._currentControl);
        //    //Assert.True(((SwipeCardControl)swipeCardPanel).IsEnabled);
        //    //Assert.True(((SwipeCardControl)swipeCardPanel).cancelButton.IsEnabled);

        //    Assert.True(_reader.Enabled);
        //    Assert.False(_scanner.Enabled);

        //    Assert.Equal(EBorrowState.INITIALIZED, ctrl._state);
        //}

        //[WpfFact]
        //public void BBUC_OP2_SwipeBorrowerCard()
        //{
        //    var mockThis = Substitute.For<IBorrowListener>();

        //    var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

        //    _reader.Received().Listener = ctrl;
        //    _scanner.Received().Listener = ctrl;

        //    Assert.Equal(ctrl, _reader.Listener);
        //    Assert.Equal(ctrl, _scanner.Listener);

        //    Assert.Equal(EBorrowState.CREATED, ctrl._state);

        //    ctrl._ui = new BorrowControl(mockThis);

        //    ctrl.initialise();

        //    //var borrowControl = (BorrowControl)ctrl._ui;

        //    //Assert.Equal(mockThis, borrowControl._listener);
        //    //Assert.Equal(4, borrowControl._controlDict.Count);

        //    //var swipeCardPanel = borrowControl._controlDict.Values.Single(c => c is SwipeCardControl);
        //    //var scanBookPanel = borrowControl._controlDict.Values.Single(c => c is ScanBookControl);

        //    //Assert.Equal(swipeCardPanel, borrowControl._currentControl);
        //    //Assert.True(((SwipeCardControl)swipeCardPanel).IsEnabled);
        //    //Assert.True(((SwipeCardControl)swipeCardPanel).cancelButton.IsEnabled);

        //    Assert.True(_reader.Enabled);
        //    Assert.False(_scanner.Enabled);

        //    Assert.Equal(EBorrowState.INITIALIZED, ctrl._state);

        //    var member = Substitute.For<IMember>();

        //    _memberDao.GetMemberByID(1).Returns(member);

        //    ctrl.cardSwiped(1);

        //    //_memberDao.Received().GetMemberByID(1);

        //    //Assert.False(_reader.Enabled);
        //    //Assert.True(_scanner.Enabled);

        //    ////Assert.False(((SwipeCardControl)swipeCardPanel).IsEnabled);
        //    ////Assert.True(((ScanBookControl)scanBookPanel).IsEnabled);
        //    ////Assert.True(((ScanBookControl)scanBookPanel).cancelButton.IsEnabled);
        //    ////Assert.True(((ScanBookControl)scanBookPanel).completeButton.IsEnabled);
        //    ////Assert.Equal(3, ((ScanBookControl)scanBookPanel).);
        //    //Assert.Equal(EBorrowState.SCANNING_BOOKS, ctrl._state);
        //}


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
