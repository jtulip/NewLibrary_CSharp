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
using Library.Interfaces.Daos;
using Library.Interfaces.Entities;
using Library.Interfaces.Hardware;
using NSubstitute;
using Xunit;

namespace Library.Tests.Integration.Control
{
    [Trait("Category", "BorrowControl Tests")]

    public class BorrowControlIntegration
    {
        public BorrowControlIntegration()
        {
            _bookDao = new BookDao(new BookHelper());
            _loanDao = new LoanDao(new LoanHelper());
            _memberDao = new MemberDao(new MemberHelper());

            _display = new MainWindow();
            _reader = new CardReader();
            _scanner = new Scanner();
            _printer = new Printer();
        }

        private IDisplay _display;
        private ICardReader _reader;
        private IScanner _scanner;
        private IPrinter _printer;
        private IBookDAO _bookDao;
        private ILoanDAO _loanDao;
        private IMemberDAO _memberDao;

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
        public void CanSwipeBorrowerCard()
        {
            var member = _memberDao.AddMember("Jim", "Tulip", "Phone", "Email");

            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            ctrl.initialise();

            //Test pre-conditions
            Assert.True(ctrl._reader.Enabled);
            Assert.Equal(ctrl, ctrl._reader.Listener);
            Assert.NotNull(ctrl._memberDAO);
            Assert.Equal(EBorrowState.INITIALIZED, ctrl._state);

            ctrl.cardSwiped(member.ID); // If we get to the end of the method then it hasn't thrown an exception.
        }

        [WpfFact]
        public void SwipeBorrowerCardShowErrorIfMemberHasOverdueLoans()
        {
            var borrowDate = DateTime.Today;
            var dueDate = DateTime.Today.AddDays(7);

            var member = _memberDao.AddMember("Jim", "Tulip", "Phone", "Email");

            var book = _bookDao.AddBook("Jim Tulip", "Adventures in Programming", "call number");

            var loan = _loanDao.CreateLoan(member, book, borrowDate, dueDate);

            _loanDao.CommitLoan(loan);

            _loanDao.UpdateOverDueStatus(DateTime.Today.AddMonths(1));

            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            // Set the UI to the mock so we can test calls being made
            var borrowctrl = Substitute.For<ABorrowControl>();
            ctrl._ui = borrowctrl;

            ctrl.initialise();

            //Test pre-conditions
            Assert.True(ctrl._reader.Enabled);
            Assert.Equal(ctrl, ctrl._reader.Listener);
            Assert.NotNull(ctrl._memberDAO);
            Assert.Equal(EBorrowState.INITIALIZED, ctrl._state);

            ctrl.cardSwiped(member.ID);

            borrowctrl.Received().DisplayOverDueMessage();
        }

        [WpfFact]
        public void SwipeBorrowerCardShowErrorIfMemberHasReachedLoanLimit()
        {
            var borrowDate = DateTime.Today;
            var dueDate = DateTime.Today.AddDays(7);

            var member = _memberDao.AddMember("Jim", "Tulip", "Phone", "Email");

            while (!member.HasReachedLoanLimit)
            {
                var book = _bookDao.AddBook("Jim Tulip", "Adventures in Programming", "call number");

                var loan = _loanDao.CreateLoan(member, book, borrowDate, dueDate);

                _loanDao.CommitLoan(loan);
            }

            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            // Set the UI to the mock so we can test calls being made
            var borrowctrl = Substitute.For<ABorrowControl>();
            ctrl._ui = borrowctrl;

            ctrl.initialise();

            //Test pre-conditions
            Assert.True(ctrl._reader.Enabled);
            Assert.Equal(ctrl, ctrl._reader.Listener);
            Assert.NotNull(ctrl._memberDAO);
            Assert.Equal(EBorrowState.INITIALIZED, ctrl._state);

            ctrl.cardSwiped(member.ID);

            borrowctrl.Received().DisplayAtLoanLimitMessage();
        }

        [WpfFact]
        public void SwipeBorrowerCardShowErrorIfMemberHasReachedFinesLimit()
        {
            var borrowDate = DateTime.Today;
            var dueDate = DateTime.Today.AddDays(7);

            var member = _memberDao.AddMember("Jim", "Tulip", "Phone", "Email");

            var book = _bookDao.AddBook("Jim Tulip", "Adventures in Programming", "call number");

            var loan = _loanDao.CreateLoan(member, book, borrowDate, dueDate);

            _loanDao.CommitLoan(loan);

            member.AddFine(BookConstants.FINE_LIMIT + 1.00f);

            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            // Set the UI to the mock so we can test calls being made
            var borrowctrl = Substitute.For<ABorrowControl>();
            ctrl._ui = borrowctrl;

            ctrl.initialise();

            //Test pre-conditions
            Assert.True(ctrl._reader.Enabled);
            Assert.Equal(ctrl, ctrl._reader.Listener);
            Assert.NotNull(ctrl._memberDAO);
            Assert.Equal(EBorrowState.INITIALIZED, ctrl._state);

            ctrl.cardSwiped(member.ID);

            borrowctrl.Received().DisplayOverFineLimitMessage(member.FineAmount);
        }

        [WpfFact]
        public void SwipeBorrowerCardShowsCurrentLoans()
        {
            var borrowDate = DateTime.Today;
            var dueDate = DateTime.Today.AddDays(7);

            var member = _memberDao.AddMember("Jim", "Tulip", "Phone", "Email");

            var book = _bookDao.AddBook("Jim Tulip", "Adventures in Programming", "call number");

            var loan = _loanDao.CreateLoan(member, book, borrowDate, dueDate);

            _loanDao.CommitLoan(loan);

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

            ctrl.cardSwiped(member.ID);

            foreach (var l in member.Loans)
            {
                borrowctrl.Received().DisplayExistingLoan(l.ToString());
            }
        }

        [WpfFact]
        public void SwipeBorrowerCardNotRestricted()
        {
            var borrowDate = DateTime.Today;
            var dueDate = DateTime.Today.AddDays(7);

            var member = _memberDao.AddMember("Jim", "Tulip", "Phone", "Email");

            var book = _bookDao.AddBook("Jim Tulip", "Adventures in Programming", "call number");

            var loan = _loanDao.CreateLoan(member, book, borrowDate, dueDate);

            _loanDao.CommitLoan(loan);

            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            // Set the UI to the mock so we can test calls being made
            var borrowctrl = Substitute.For<ABorrowControl>();
            ctrl._ui = borrowctrl;

            ctrl.initialise();

            //Test pre-conditions
            Assert.True(ctrl._reader.Enabled);
            Assert.Equal(ctrl, ctrl._reader.Listener);
            Assert.NotNull(ctrl._memberDAO);
            Assert.Equal(EBorrowState.INITIALIZED, ctrl._state);

            ctrl.cardSwiped(member.ID);

            borrowctrl.Received().DisplayMemberDetails(member.ID, $"{member.FirstName} {member.LastName}", member.ContactPhone);

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
            var borrowDate = DateTime.Today;
            var dueDate = DateTime.Today.AddDays(7);

            var member = _memberDao.AddMember("Jim", "Tulip", "Phone", "Email");

            var book = _bookDao.AddBook("Jim Tulip", "Adventures in Programming", "call number");

            var loan = _loanDao.CreateLoan(member, book, borrowDate, dueDate);

            _loanDao.CommitLoan(loan);

            _loanDao.UpdateOverDueStatus(DateTime.Today.AddMonths(1));

            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            // Set the UI to the mock so we can test calls being made
            var borrowctrl = Substitute.For<ABorrowControl>();
            ctrl._ui = borrowctrl;

            ctrl.initialise();

            //Test pre-conditions
            Assert.True(ctrl._reader.Enabled);
            Assert.Equal(ctrl, ctrl._reader.Listener);
            Assert.NotNull(ctrl._memberDAO);
            Assert.Equal(EBorrowState.INITIALIZED, ctrl._state);

            ctrl.cardSwiped(member.ID);

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

        [WpfFact]
        public void CanScanBook()
        {
            var member = _memberDao.AddMember("Jim", "Tulip", "Phone", "Email");

            var book = _bookDao.AddBook("Jim Tulip", "Adventures in Programming", "call number");

            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            InitialiseToScanBookPreConditions(ctrl, member);

            ctrl.bookScanned(book.ID); // if we get this far we've worked.
        }

        [WpfFact]
        public void ScanBooksCardReaderDisabled()
        {
            var member = _memberDao.AddMember("Jim", "Tulip", "Phone", "Email");

            var book = _bookDao.AddBook("Jim Tulip", "Adventures in Programming", "call number");

            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            InitialiseToScanBookPreConditions(ctrl, member);

            ctrl.bookScanned(book.ID);

            Assert.True(!_reader.Enabled);
        }

        [WpfFact]
        public void ScanBooksScannerEnabled()
        {
            var member = _memberDao.AddMember("Jim", "Tulip", "Phone", "Email");

            var book = _bookDao.AddBook("Jim Tulip", "Adventures in Programming", "call number");

            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            InitialiseToScanBookPreConditions(ctrl, member);

            ctrl.bookScanned(book.ID);

            Assert.True(_scanner.Enabled);
        }

        [WpfFact]
        public void ScanBooksBookNotFound()
        {
            var member = _memberDao.AddMember("Jim", "Tulip", "Phone", "Email");

            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            // Set the UI to the mock so we can test
            var borrowctrl = Substitute.For<ABorrowControl>();
            ctrl._ui = borrowctrl;

            InitialiseToScanBookPreConditions(ctrl, member);

            ctrl.bookScanned(0);

            borrowctrl.Received().DisplayErrorMessage("Book scanned was not found");

            Assert.Equal(EBorrowState.SCANNING_BOOKS, ctrl._state);
        }

        [WpfFact]
        public void ScanBooksBookNotAvailable()
        {
            var member = _memberDao.AddMember("Jim", "Tulip", "Phone", "Email");

            var book = _bookDao.AddBook("Jim Tulip", "Adventures in Programming", "call number");
            book.Dispose();

            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            // Set the UI to the mock so we can test
            var borrowctrl = Substitute.For<ABorrowControl>();
            ctrl._ui = borrowctrl;

            InitialiseToScanBookPreConditions(ctrl, member);

            ctrl.bookScanned(book.ID);

            borrowctrl.Received().DisplayErrorMessage("Book is not available to be borrowed");

            Assert.Equal(EBorrowState.SCANNING_BOOKS, ctrl._state);
        }

        [WpfFact]
        public void ScanBooksBookAlreadyScanned()
        {
            var member = _memberDao.AddMember("Jim", "Tulip", "Phone", "Email");

            var book = _bookDao.AddBook("Jim Tulip", "Adventures in Programming", "call number");

            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            // Set the UI to the mock so we can test
            var borrowctrl = Substitute.For<ABorrowControl>();
            ctrl._ui = borrowctrl;

            InitialiseToScanBookPreConditions(ctrl, member);

            ctrl._bookList.Add(book); // Add the book to the booklist.

            ctrl.bookScanned(book.ID);

            borrowctrl.Received().DisplayErrorMessage("Book has already been scanned");

            Assert.Equal(EBorrowState.SCANNING_BOOKS, ctrl._state);
        }

        [WpfFact]
        public void ScanBooksBookScanCountLessThanLoanLimit()
        {
            var member = _memberDao.AddMember("Jim", "Tulip", "Phone", "Email");

            var book = _bookDao.AddBook("Jim Tulip", "Adventures in Programming", "call number");

            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            // Set the UI to the mock so we can test
            var borrowctrl = Substitute.For<ABorrowControl>();
            ctrl._ui = borrowctrl;

            InitialiseToScanBookPreConditions(ctrl, member);

            ctrl.bookScanned(book.ID);

            borrowctrl.Received().DisplayScannedBookDetails(book.ToString());

            Assert.Equal(1, ctrl.scanCount);
            Assert.NotNull(ctrl._loanList);
            Assert.NotEmpty(ctrl._loanList);
            Assert.Equal(1, ctrl._loanList.Count);

            borrowctrl.Received().DisplayPendingLoan(ctrl._loanList[0].ToString());

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
            var member = _memberDao.AddMember("Jim", "Tulip", "Phone", "Email");

            var book = _bookDao.AddBook("Jim Tulip", "Adventures in Programming", "call number");

            var borrowDate = DateTime.Today;
            var dueDate = DateTime.Today.AddDays(7);

            var ctrl = new BorrowController(_display, _reader, _scanner, _printer, _bookDao, _loanDao, _memberDao);

            // Set the UI to the mock so we can test
            var borrowctrl = Substitute.For<ABorrowControl>();
            ctrl._ui = borrowctrl;

            InitialiseToScanBookPreConditions(ctrl, member);

            ctrl.scanCount = BookConstants.LOAN_LIMIT - 1;

            ctrl.bookScanned(book.ID);

            borrowctrl.DisplayScannedBookDetails(book.ToString());

            Assert.Equal(BookConstants.LOAN_LIMIT, ctrl.scanCount);
            Assert.NotNull(ctrl._loanList);
            Assert.NotEmpty(ctrl._loanList);
            Assert.Equal(1, ctrl._loanList.Count);

            borrowctrl.DisplayPendingLoan(ctrl._loanList[0].ToString());

            Assert.NotNull(ctrl._bookList);
            Assert.NotEmpty(ctrl._bookList);
            Assert.Equal(1, ctrl._bookList.Count);

            Assert.Equal(book, ctrl._bookList[0]);

            Assert.True(!_scanner.Enabled);

            Assert.Equal(EBorrowState.CONFIRMING_LOANS, ctrl._state);
        }



        private void InitialiseToScanBookPreConditions(BorrowController ctrl, IMember member)
        {
            ctrl.initialise();

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
