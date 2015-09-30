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
