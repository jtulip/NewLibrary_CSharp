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
using Xunit;

namespace Library.Tests.Scenarios
{
    [Trait("Category", "Scenarios")]
    public class Scenario06RejectLoans: IDisposable
    {
        public Scenario06RejectLoans()
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
        public void RunScenario()
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
            controller.bookScanned(book.ID);
            controller.scansCompleted();

            // Test Pre-conditions
            Assert.True(_display.Display.IsEnabled);

            var borrowCtrl = ((BorrowControl)_display.Display);
            var confirmingLoansCtrl = borrowCtrl._controlDict.Single(c => c.Value is ConfirmLoanControl).Value as ConfirmLoanControl;
            var scanBookCtrl = borrowCtrl._controlDict.Single(c => c.Value is ScanBookControl).Value as ScanBookControl;

            Assert.NotNull(confirmingLoansCtrl);
            Assert.True(confirmingLoansCtrl.IsEnabled);
            Assert.True(confirmingLoansCtrl.cancelButton.IsEnabled);
            Assert.True(confirmingLoansCtrl.rejectButton.IsEnabled);

            Assert.True(!_reader.Enabled);
            Assert.True(!_scanner.Enabled);

            Assert.NotNull(controller._loanList);
            Assert.NotEmpty(controller._loanList);
            Assert.Equal(1, controller._loanList.Count);
            Assert.Equal(book, controller._loanList[0].Book);
            Assert.Equal(member, controller._loanList[0].Borrower);

            var loan = controller._loanList[0];

            Assert.NotNull(controller._bookList);
            Assert.NotEmpty(controller._bookList);
            Assert.Equal(1, controller._bookList.Count);

            Assert.Equal(book, controller._bookList[0]);

            Assert.Equal(EBorrowState.CONFIRMING_LOANS, controller._state);

            // Run use case
            controller.loansRejected();

            // Test Post-conditions

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

            Assert.Equal(member.ID, scanBookCtrl.idLabel.Content);
            Assert.Equal($"{member.FirstName} {member.LastName}", scanBookCtrl.nameLabel.Content.ToString());
            Assert.Equal(member.Loans[0].ToString(), scanBookCtrl.existingLoanBox.Text);  // Test one existing loan is present

            Assert.Equal("", scanBookCtrl.pendingLoanBox.Text);
            Assert.Equal("", scanBookCtrl.currentbookBox.Text);

            Assert.NotNull(controller._bookList);
            Assert.Empty(controller._bookList);
            Assert.NotNull(controller._loanList);
            Assert.Empty(controller._loanList);
            Assert.Equal(1, controller.scanCount);
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
