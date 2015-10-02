using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
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
            var member = CreateMockIMember();

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
        public void SwipeBorrowerCardControlNotInitialized()
        {
            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            Assert.Equal(EBorrowState.CREATED, ctrl._state);

            var ex = Assert.Throws<InvalidOperationException>(() => { ctrl.cardSwiped(1); });

            Assert.Equal("Controls must be initialised before member's card is swiped", ex.Message);
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
            var member = CreateMockIMember();
            member.HasReachedLoanLimit.Returns(true);

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

            borrowctrl.Received().DisplayAtLoanLimitMessage();
        }

        [WpfFact]
        public void SwipeBorrowerCardShowErrorIfMemberHasReachedFinesLimit()
        {
            var member = CreateMockIMember();
            member.HasReachedFineLimit.Returns(true);
            member.FineAmount.Returns(100.00f);
            
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

            borrowctrl.Received().DisplayOverFineLimitMessage(member.FineAmount);
        }

        [WpfFact]
        public void SwipeBorrowerCardShowsCurrentLoans()
        {
            var loan = Substitute.For<ILoan>();

            var member = CreateMockIMember();

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
            var member = CreateMockIMember();

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

            borrowctrl.Received().DisplayMemberDetails(member.ID, member.ToString() , member.ContactPhone);

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
            var member = CreateMockIMember();
            member.HasOverDueLoans.Returns(true);
            
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

            borrowctrl.Received().DisplayMemberDetails(member.ID, member.ToString(), member.ContactPhone);

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

        [WpfFact]
        public void CanScanBook()
        {
            var member = CreateMockIMember();

            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            InitialiseToScanBookPreConditions(ctrl, member);

            // Make the bookDao return a successful read
            _bookDao.GetBookByID(0).Returns(Substitute.For<IBook>());

            ctrl.bookScanned(0); // if we get this far we've worked.
        }


        [WpfFact]
        public void ScanBookControlNotScanningBooks()
        {
            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            ctrl.initialise();

            var ex = Assert.Throws<InvalidOperationException>(() => { ctrl.bookScanned(0); });

            Assert.Equal("Control state must be set to 'Scanning Books'", ex.Message);
        }

        [WpfFact]
        public void ScanBooksCardReaderDisabled()
        {
            var member = CreateMockIMember();

            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            InitialiseToScanBookPreConditions(ctrl, member);

            // Make the bookDao return a successful read
            _bookDao.GetBookByID(0).Returns(Substitute.For<IBook>());

            ctrl.bookScanned(0);

            Assert.True(!_reader.Enabled);
        }

        [WpfFact]
        public void ScanBooksScannerEnabled()
        {
            var member = CreateMockIMember();

            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            InitialiseToScanBookPreConditions(ctrl, member);

            // Make the bookDao return a successful read
            _bookDao.GetBookByID(0).Returns(Substitute.For<IBook>());

            ctrl.bookScanned(0);

            Assert.True(_scanner.Enabled);
        }

        [WpfFact]
        public void ScanBooksBookNotFound()
        {
            var member = CreateMockIMember();

            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            // Set the UI to the mock so we can test
            var borrowctrl = Substitute.For<ABorrowControl>();
            ctrl._ui = borrowctrl;

            InitialiseToScanBookPreConditions(ctrl, member);

            _bookDao.GetBookByID(0).Returns((IBook)null);
            
            ctrl.bookScanned(0);

            _bookDao.Received().GetBookByID(0);

            borrowctrl.Received().DisplayErrorMessage("Book scanned was not found");

            Assert.Equal(EBorrowState.SCANNING_BOOKS, ctrl._state);
        }

        [WpfFact]
        public void ScanBooksBookNotAvailable()
        {
            var member = CreateMockIMember();

            var book = Substitute.For<IBook>();
            book.State.Returns(BookState.DISPOSED);

            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            // Set the UI to the mock so we can test
            var borrowctrl = Substitute.For<ABorrowControl>();
            ctrl._ui = borrowctrl;

            InitialiseToScanBookPreConditions(ctrl, member);

            _bookDao.GetBookByID(0).Returns(book);

            ctrl.bookScanned(0);

            _bookDao.Received().GetBookByID(0);

            borrowctrl.Received().DisplayErrorMessage("Book is not available to be borrowed");

            Assert.Equal(EBorrowState.SCANNING_BOOKS, ctrl._state);
        }

        [WpfFact]
        public void ScanBooksBookAlreadyScanned()
        {
            var member = CreateMockIMember();

            var book = Substitute.For<IBook>();
            book.State.Returns(BookState.AVAILABLE);

            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            // Set the UI to the mock so we can test
            var borrowctrl = Substitute.For<ABorrowControl>();
            ctrl._ui = borrowctrl;

            InitialiseToScanBookPreConditions(ctrl, member);

            _bookDao.GetBookByID(0).Returns(book);

            ctrl._bookList.Add(book); // Add the book to the booklist.

            ctrl.bookScanned(0);

            _bookDao.Received().GetBookByID(0);

            borrowctrl.Received().DisplayErrorMessage("Book has already been scanned");

            Assert.Equal(EBorrowState.SCANNING_BOOKS, ctrl._state);
        }

        [WpfFact]
        public void ScanBooksBookScanCountLessThanLoanLimit()
        {
            var member = CreateMockIMember();

            var book = Substitute.For<IBook>();
            book.State.Returns(BookState.AVAILABLE);

            var borrowDate = DateTime.Today;
            var dueDate = DateTime.Today.AddDays(7);

            var loan = Substitute.For<Loan>(book, member, borrowDate, dueDate);

            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            // Set the UI to the mock so we can test
            var borrowctrl = Substitute.For<ABorrowControl>();
            ctrl._ui = borrowctrl;

            InitialiseToScanBookPreConditions(ctrl, member);

            _bookDao.GetBookByID(0).Returns(book);
            _loanDao.CreateLoan(member, book, borrowDate, dueDate).Returns(loan);

            ctrl.bookScanned(0);

            _bookDao.Received().GetBookByID(0);
            _loanDao.Received().CreateLoan(member, book, borrowDate, dueDate);

            borrowctrl.Received().DisplayScannedBookDetails(book.ToString());

            borrowctrl.Received().DisplayPendingLoan(loan.ToString());

            Assert.Equal(1, ctrl.scanCount);
            Assert.NotNull(ctrl._loanList);
            Assert.NotEmpty(ctrl._loanList);
            Assert.Equal(1, ctrl._loanList.Count);

            Assert.Equal(loan, ctrl._loanList[0]);

            Assert.NotNull(ctrl._bookList);
            Assert.NotEmpty(ctrl._bookList);
            Assert.Equal(1, ctrl._bookList.Count);

            Assert.Equal(book, ctrl._bookList[0]);

            Assert.Equal(EBorrowState.SCANNING_BOOKS, ctrl._state);
        }

        // Assuming that it becomes equal on the scan otherwise it would allow an additional scan and error?
        [WpfFact]
        public void ScanBooksBookScanCountEqualsLoanLimit()
        {
            var member = CreateMockIMember();

            var book = Substitute.For<IBook>();
            book.State.Returns(BookState.AVAILABLE);

            var book2 = Substitute.For<IBook>();
            book2.State.Returns(BookState.AVAILABLE);

            var borrowDate = DateTime.Today;
            var dueDate = DateTime.Today.AddDays(7);

            var loan = Substitute.For<Loan>(book, member, borrowDate, dueDate);
            var loan2 = Substitute.For<Loan>(book2, member, borrowDate, dueDate);


            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            // Set the UI to the mock so we can test
            var borrowctrl = Substitute.For<ABorrowControl>();
            ctrl._ui = borrowctrl;

            InitialiseToScanBookPreConditions(ctrl, member);

            _bookDao.GetBookByID(0).Returns(book);
            _bookDao.GetBookByID(1).Returns(book2);
            _loanDao.CreateLoan(member, book, borrowDate, dueDate).Returns(loan);
            _loanDao.CreateLoan(member, book2, borrowDate, dueDate).Returns(loan2);

            ctrl.scanCount = BookConstants.LOAN_LIMIT - 2;

            ctrl.bookScanned(0);
            ctrl.bookScanned(1);

            _bookDao.Received().GetBookByID(0);
            _loanDao.Received().CreateLoan(member, book, borrowDate, dueDate);

            _bookDao.Received().GetBookByID(1);
            _loanDao.Received().CreateLoan(member, book2, borrowDate, dueDate);


            borrowctrl.Received().DisplayScannedBookDetails(book.ToString());
            borrowctrl.Received().DisplayScannedBookDetails(book2.ToString());

            borrowctrl.Received().DisplayPendingLoan(loan.ToString());
            borrowctrl.Received().DisplayPendingLoan(loan2.ToString());

            Assert.Equal(BookConstants.LOAN_LIMIT, ctrl.scanCount);
            Assert.NotNull(ctrl._loanList);
            Assert.NotEmpty(ctrl._loanList);
            Assert.Equal(2, ctrl._loanList.Count);

            Assert.Equal(loan, ctrl._loanList[0]);

            Assert.NotNull(ctrl._bookList);
            Assert.NotEmpty(ctrl._bookList);
            Assert.Equal(2, ctrl._bookList.Count);

            Assert.Equal(book, ctrl._bookList[0]);

            Assert.True(!_scanner.Enabled);

            borrowctrl.Received().DisplayConfirmingLoan(loan.ToString());
            borrowctrl.Received().DisplayConfirmingLoan(loan2.ToString());

            Assert.Equal(EBorrowState.CONFIRMING_LOANS, ctrl._state);
        }

        [WpfFact]
        public void CanCompleteScans()
        {
            var loan = Substitute.For<ILoan>();

            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            // Set the UI to the mock so we can test
            var borrowctrl = Substitute.For<ABorrowControl>();
            ctrl._ui = borrowctrl;

            ctrl.initialise();

            // Set Pre-conditions
            ctrl._state = EBorrowState.SCANNING_BOOKS;
            ctrl._loanList.Add(loan);
            ctrl._loanList.Add(loan);

            Assert.NotNull(ctrl);
            Assert.NotEmpty(ctrl._loanList);
            Assert.Equal(EBorrowState.SCANNING_BOOKS, ctrl._state);

            ctrl.scansCompleted();

            borrowctrl.Received(2).DisplayConfirmingLoan(loan.ToString());

            Assert.True(!_reader.Enabled);
            Assert.True(!_scanner.Enabled);
            Assert.Equal(EBorrowState.CONFIRMING_LOANS, ctrl._state);
        }

        [WpfFact]
        public void CompleteScansControlNotScanningBooks()
        {
            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            var ex = Assert.Throws<InvalidOperationException>(() => { ctrl.scansCompleted(); });

            Assert.Equal("Control state must be set to 'Scanning Books'", ex.Message);
        }

        [WpfFact]
        public void CanConfirmLoans()
        {
            var loan = Substitute.For<ILoan>();

            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            // Set the UI to the mock so we can test
            var borrowctrl = Substitute.For<ABorrowControl>();
            ctrl._ui = borrowctrl;

            ctrl.initialise();

            // Set Pre-conditions
            ctrl._state = EBorrowState.CONFIRMING_LOANS;
            ctrl._loanList.Add(loan);
            ctrl._loanList.Add(loan);

            Assert.NotNull(ctrl);
            Assert.NotEmpty(ctrl._loanList);
            Assert.Equal(EBorrowState.CONFIRMING_LOANS, ctrl._state);

            ctrl.loansConfirmed();

            _loanDao.Received(2).CommitLoan(loan);

            _printer.Received(2).print(loan.ToString());

            Assert.True(!_reader.Enabled);
            Assert.True(!_scanner.Enabled);
            Assert.Equal(EBorrowState.COMPLETED, ctrl._state);
        }

        [WpfFact]
        public void ConfirmLoansControlNotConfirmingLoans()
        {
            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            var ex = Assert.Throws<InvalidOperationException>(() => { ctrl.loansConfirmed(); });

            Assert.Equal("Control state must be set to 'Confirming Loans'", ex.Message);
        }

        [WpfFact]
        public void CanRejectLoans()
        {
            var loan = Substitute.For<ILoan>();


            var member = CreateMockIMember();
            member.Loans.Returns(new List<ILoan>() { loan });

            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            InitialiseToScanBookPreConditions(ctrl, member);

            // Set the UI to the mock so we can test
            var borrowctrl = Substitute.For<ABorrowControl>();
            ctrl._ui = borrowctrl;

            // Set Pre-conditions
            ctrl._state = EBorrowState.CONFIRMING_LOANS;
            ctrl._loanList.Add(loan);
            ctrl._loanList.Add(loan);

            Assert.NotNull(ctrl);
            Assert.NotEmpty(ctrl._loanList);
            Assert.Equal(EBorrowState.CONFIRMING_LOANS, ctrl._state);

            ctrl.loansRejected();

            borrowctrl.Received().DisplayMemberDetails(member.ID, member.ToString(), member.ContactPhone);
            borrowctrl.Received().DisplayExistingLoan(loan.ToString());

            borrowctrl.Received().DisplayPendingLoan("");
            borrowctrl.Received().DisplayScannedBookDetails("");

            Assert.Empty(ctrl._loanList);
            Assert.Empty(ctrl._bookList);
            Assert.Equal(1, ctrl.scanCount);
            Assert.True(!_reader.Enabled);
            Assert.True(_scanner.Enabled);
            Assert.Equal(EBorrowState.SCANNING_BOOKS, ctrl._state);
        }

        [WpfFact]
        public void RejectLoansControlNotConfirmingLoans()
        {
            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            var ex = Assert.Throws<InvalidOperationException>(() => { ctrl.loansRejected(); });

            Assert.Equal("Control state must be set to 'Confirming Loans'", ex.Message);
        }

        [WpfFact]
        public void ScanBooksBookScanCountLessThanLoanLimitClearsPreviousError()
        {
            var member = CreateMockIMember();

            var book = Substitute.For<IBook>();
            book.State.Returns(BookState.AVAILABLE);

            var borrowDate = DateTime.Today;
            var dueDate = DateTime.Today.AddDays(7);

            var loan = Substitute.For<Loan>(book, member, borrowDate, dueDate);

            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            // Set the UI to the mock so we can test
            var borrowctrl = Substitute.For<ABorrowControl>();
            ctrl._ui = borrowctrl;

            InitialiseToScanBookPreConditions(ctrl, member);

            _bookDao.GetBookByID(0).Returns(book);
            _loanDao.CreateLoan(member, book, borrowDate, dueDate).Returns(loan);

            ctrl.bookScanned(0);

            // Expect the error message to be cleared
            borrowctrl.Received().DisplayErrorMessage("");
        }

        private static IMember CreateMockIMember()
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

            return member;
        }

        private void InitialiseToScanBookPreConditions(BorrowController ctrl, IMember member)
        {
            ctrl.initialise();

            _memberDao.GetMemberByID(member.ID).Returns(member);

            ctrl.cardSwiped(member.ID);

            // Test Pre-conditions
            Assert.NotNull(ctrl);
            Assert.NotNull(ctrl._scanner);
            Assert.Equal(ctrl._scanner.Listener, ctrl);
            Assert.Equal(EBorrowState.SCANNING_BOOKS, ctrl._state);
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
