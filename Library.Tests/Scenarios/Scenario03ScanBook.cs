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
using Library.Interfaces.Hardware;
using Xunit;

namespace Library.Tests.Scenarios
{
    [Trait("Category", "Scenarios")]
    public class Scenario03ScanBook: IDisposable
    {
            public Scenario03ScanBook()
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
            public void RunBookNotFound()
            {
            // Some test data initialisation
            var borrowDate = DateTime.Today;
            var dueDate = DateTime.Today.AddDays(7);

            var member = _memberDao.AddMember("Jim", "Tulip", "Phone", "Email");

            var existingBook = _bookDao.AddBook("Jim Tulip", "Adventures in Programming", "call number");

            var existingLoan = _loanDao.CreateLoan(member, existingBook, borrowDate, dueDate);

            _loanDao.CommitLoan(existingLoan);

            // Set up
            var controller = new BorrowController(_display, _reader, _scanner, _printer,
                                                        _bookDao, _loanDao, _memberDao);

                controller.initialise();
            controller.cardSwiped(member.ID);


            // Test Pre-conditions
            Assert.True(_display.Display.IsEnabled);

                var borrowCtrl = ((BorrowControl)_display.Display);
                var scanBookCtrl = borrowCtrl._controlDict.Single(c => c.Value is ScanBookControl).Value as ScanBookControl;

            Assert.NotNull(scanBookCtrl);
            Assert.True(scanBookCtrl.IsEnabled);
            Assert.True(scanBookCtrl.cancelButton.IsEnabled);
            Assert.True(scanBookCtrl.completeButton.IsEnabled);

            Assert.True(!_reader.Enabled);
            Assert.True(_scanner.Enabled);
            Assert.Equal(controller, _scanner.Listener);

            Assert.Equal(member.Loans.Count, controller.scanCount);
            Assert.Equal(member, controller._borrower);
            Assert.Equal(EBorrowState.SCANNING_BOOKS, controller._state);

            // Run use case
            controller.bookScanned(5);

            // Test Post-conditions
            Assert.True(scanBookCtrl.IsEnabled);
            Assert.True(scanBookCtrl.cancelButton.IsEnabled);
            Assert.True(scanBookCtrl.completeButton.IsEnabled);

            Assert.True(!_reader.Enabled);
            Assert.True(_scanner.Enabled);

            Assert.Equal("Book scanned was not found", scanBookCtrl.errorMessage.Content);

            Assert.Equal(EBorrowState.SCANNING_BOOKS, controller._state);
        }

        [WpfFact]
        public void RunBookNotAvailable()
        {
            // Some test data initialisation
            var borrowDate = DateTime.Today;
            var dueDate = DateTime.Today.AddDays(7);

            var member = _memberDao.AddMember("Jim", "Tulip", "Phone", "Email");

            var existingBook = _bookDao.AddBook("Jim Tulip", "Adventures in Programming", "call number");

            var existingLoan = _loanDao.CreateLoan(member, existingBook, borrowDate, dueDate);

            _loanDao.CommitLoan(existingLoan);

            var book = _bookDao.AddBook("Jim Tulip", "Adventures in Programming 2", "call number");
            book.Dispose();

            // Set up
            var controller = new BorrowController(_display, _reader, _scanner, _printer,
                                                        _bookDao, _loanDao, _memberDao);

            controller.initialise();
            controller.cardSwiped(member.ID);

            // Test Pre-conditions
            Assert.True(_display.Display.IsEnabled);

            var borrowCtrl = ((BorrowControl)_display.Display);
            var scanBookCtrl = borrowCtrl._controlDict.Single(c => c.Value is ScanBookControl).Value as ScanBookControl;

            Assert.NotNull(scanBookCtrl);
            Assert.True(scanBookCtrl.IsEnabled);
            Assert.True(scanBookCtrl.cancelButton.IsEnabled);
            Assert.True(scanBookCtrl.completeButton.IsEnabled);

            Assert.True(!_reader.Enabled);
            Assert.True(_scanner.Enabled);
            Assert.Equal(controller, _scanner.Listener);

            Assert.Equal(member.Loans.Count, controller.scanCount);
            Assert.Equal(member, controller._borrower);
            Assert.Equal(EBorrowState.SCANNING_BOOKS, controller._state);

            // Run use case
            controller.bookScanned(book.ID);

            // Test Post-conditions
            Assert.True(scanBookCtrl.IsEnabled);
            Assert.True(scanBookCtrl.cancelButton.IsEnabled);
            Assert.True(scanBookCtrl.completeButton.IsEnabled);

            Assert.True(!_reader.Enabled);
            Assert.True(_scanner.Enabled);

            Assert.Equal("Book is not available to be borrowed", scanBookCtrl.errorMessage.Content);

            Assert.Equal(EBorrowState.SCANNING_BOOKS, controller._state);
        }

        [WpfFact]
        public void RunBookAlreadyScanned()
        {
            // Some test data initialisation
            var borrowDate = DateTime.Today;
            var dueDate = DateTime.Today.AddDays(7);

            var member = _memberDao.AddMember("Jim", "Tulip", "Phone", "Email");

            var existingBook = _bookDao.AddBook("Jim Tulip", "Adventures in Programming", "call number");

            var existingLoan = _loanDao.CreateLoan(member, existingBook, borrowDate, dueDate);

            _loanDao.CommitLoan(existingLoan);

            var book = _bookDao.AddBook("Jim Tulip", "Adventures in Programming 2", "call number");

            // Set up
            var controller = new BorrowController(_display, _reader, _scanner, _printer,
                                                        _bookDao, _loanDao, _memberDao);

            controller.initialise();
            controller.cardSwiped(member.ID);

            controller._bookList.Add(book);

            // Test Pre-conditions
            Assert.True(_display.Display.IsEnabled);

            var borrowCtrl = ((BorrowControl)_display.Display);
            var scanBookCtrl = borrowCtrl._controlDict.Single(c => c.Value is ScanBookControl).Value as ScanBookControl;

            Assert.NotNull(scanBookCtrl);
            Assert.True(scanBookCtrl.IsEnabled);
            Assert.True(scanBookCtrl.cancelButton.IsEnabled);
            Assert.True(scanBookCtrl.completeButton.IsEnabled);

            Assert.True(!_reader.Enabled);
            Assert.True(_scanner.Enabled);
            Assert.Equal(controller, _scanner.Listener);

            Assert.Equal(member.Loans.Count, controller.scanCount);
            Assert.Equal(member, controller._borrower);
            Assert.Equal(EBorrowState.SCANNING_BOOKS, controller._state);

            // Run use case
            controller.bookScanned(book.ID);

            // Test Post-conditions
            Assert.True(scanBookCtrl.IsEnabled);
            Assert.True(scanBookCtrl.cancelButton.IsEnabled);
            Assert.True(scanBookCtrl.completeButton.IsEnabled);

            Assert.True(!_reader.Enabled);
            Assert.True(_scanner.Enabled);

            Assert.Equal("Book has already been scanned", scanBookCtrl.errorMessage.Content);

            Assert.Equal(EBorrowState.SCANNING_BOOKS, controller._state);
        }

        [WpfFact]
        public void RunScanCountLessThanLoanLimit()
        {
            // Some test data initialisation
            var borrowDate = DateTime.Today;
            var dueDate = DateTime.Today.AddDays(7);

            var member = _memberDao.AddMember("Jim", "Tulip", "Phone", "Email");

            var existingBook = _bookDao.AddBook("Jim Tulip", "Adventures in Programming", "call number");

            var existingLoan = _loanDao.CreateLoan(member, existingBook, borrowDate, dueDate);

            _loanDao.CommitLoan(existingLoan);

            var book = _bookDao.AddBook("Jim Tulip", "Adventures in Programming 2", "call number");

            // Set up
            var controller = new BorrowController(_display, _reader, _scanner, _printer,
                                                        _bookDao, _loanDao, _memberDao);

            controller.initialise();
            controller.cardSwiped(member.ID);

            var originalScanCount = controller.scanCount;

            // Test Pre-conditions
            Assert.True(_display.Display.IsEnabled);

            var borrowCtrl = ((BorrowControl)_display.Display);
            var scanBookCtrl = borrowCtrl._controlDict.Single(c => c.Value is ScanBookControl).Value as ScanBookControl;

            Assert.NotNull(scanBookCtrl);
            Assert.True(scanBookCtrl.IsEnabled);
            Assert.True(scanBookCtrl.cancelButton.IsEnabled);
            Assert.True(scanBookCtrl.completeButton.IsEnabled);

            Assert.True(!_reader.Enabled);
            Assert.True(_scanner.Enabled);
            Assert.Equal(controller, _scanner.Listener);

            Assert.Equal(member.Loans.Count, controller.scanCount);
            Assert.Equal(member, controller._borrower);
            Assert.Equal(EBorrowState.SCANNING_BOOKS, controller._state);

            // Run use case
            controller.bookScanned(book.ID);

            // Test Post-conditions
            Assert.True(scanBookCtrl.IsEnabled);
            Assert.True(scanBookCtrl.cancelButton.IsEnabled);
            Assert.True(scanBookCtrl.completeButton.IsEnabled);

            Assert.True(!_reader.Enabled);
            Assert.True(_scanner.Enabled);

            Assert.True(controller.scanCount == originalScanCount + 1);

            Assert.NotNull(controller._loanList);
            Assert.NotEmpty(controller._loanList);
            Assert.Equal(1, controller._loanList.Count);
            Assert.Equal(book, controller._loanList[0].Book);
            Assert.Equal(member, controller._loanList[0].Borrower);

            Assert.True(scanBookCtrl.pendingLoanBox.Text.Contains(controller._loanList[0].ToString()));

            Assert.NotNull(controller._bookList);
            Assert.NotEmpty(controller._bookList);
            Assert.Equal(1, controller._bookList.Count);

            Assert.Equal(book, controller._bookList[0]);

            Assert.Equal(EBorrowState.SCANNING_BOOKS, controller._state);
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
