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
using NSubstitute;
using Xunit;

namespace Library.Tests.Scenarios
{
    [Trait("Category", "Scenarios")]
    public class Scenario02SwipeBorrowerCard: IDisposable
    {
        public Scenario02SwipeBorrowerCard()
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
        public void RunMemberExistsAndNotRestricted()
        {
            // Set up
            var controller = new BorrowController(_display, _reader, _scanner, _printer,
                                                    _bookDao, _loanDao, _memberDao);

            controller.initialise();

            // Some test data initialisation
            var borrowDate = DateTime.Today;
            var dueDate = DateTime.Today.AddDays(7);

            var member = _memberDao.AddMember("Jim", "Tulip", "Phone", "Email");

            var book = _bookDao.AddBook("Jim Tulip", "Adventures in Programming", "call number");

            var loan = _loanDao.CreateLoan(member, book, borrowDate, dueDate);

            _loanDao.CommitLoan(loan);

            // Test Pre-conditions
            Assert.True(_display.Display.IsEnabled);

            var borrowCtrl = ((BorrowControl)_display.Display);
            var swipeCtrl = borrowCtrl._controlDict.Single(c => c.Value is SwipeCardControl).Value as SwipeCardControl;
            var scanBookCtrl = borrowCtrl._controlDict.Single(c => c.Value is ScanBookControl).Value as ScanBookControl;

            Assert.NotNull(swipeCtrl);
            Assert.True(swipeCtrl.IsEnabled);
            Assert.True(swipeCtrl.cancelButton.IsEnabled);

            Assert.True(_reader.Enabled);
            Assert.Equal(controller, _reader.Listener);
            Assert.NotNull(controller._memberDAO);
            Assert.Equal(EBorrowState.INITIALIZED, controller._state);

            // Run use case
            controller.cardSwiped(member.ID);

            // Test Post-conditions
            Assert.NotNull(scanBookCtrl);
            Assert.True(scanBookCtrl.IsEnabled);
            Assert.True(scanBookCtrl.cancelButton.IsEnabled);
            Assert.True(scanBookCtrl.completeButton.IsEnabled);

            Assert.True(!_reader.Enabled);
            Assert.True(_scanner.Enabled);

            Assert.Equal(member.ID, scanBookCtrl.idLabel.Content);
            Assert.Equal($"{member.FirstName} {member.LastName}", scanBookCtrl.nameLabel.Content.ToString());
            Assert.Equal(member.Loans[0].ToString(), scanBookCtrl.existingLoanBox.Text);  // Test one existing loan is present

            Assert.Equal(member.Loans.Count, controller.scanCount);
            Assert.Equal(member, controller._borrower);
            Assert.Equal(EBorrowState.SCANNING_BOOKS, controller._state);
        }

        [WpfFact]
        public void RunMemberExistsAndRestricted()
        {
            // Set up
            var controller = new BorrowController(_display, _reader, _scanner, _printer,
                                                    _bookDao, _loanDao, _memberDao);

            controller.initialise();

            // Some test data initialisation
            var borrowDate = DateTime.Today;
            var dueDate = DateTime.Today.AddDays(7);

            var member = _memberDao.AddMember("Jim", "Tulip", "Phone", "Email");

            var book = _bookDao.AddBook("Jim Tulip", "Adventures in Programming", "call number");

            var loan = _loanDao.CreateLoan(member, book, borrowDate, dueDate);

            _loanDao.CommitLoan(loan);
            
            // Make the loan overdue to put member on restricted status
            _loanDao.UpdateOverDueStatus(DateTime.Today.AddMonths(1));

            // Test Pre-conditions
            Assert.True(_display.Display.IsEnabled);

            var borrowCtrl = ((BorrowControl)_display.Display);
            var swipeCtrl = borrowCtrl._controlDict.Single(c => c.Value is SwipeCardControl).Value as SwipeCardControl;
            var restrictedCtrl = borrowCtrl._controlDict.Single(c => c.Value is RestrictedControl).Value as RestrictedControl;

            Assert.NotNull(swipeCtrl);
            Assert.True(swipeCtrl.IsEnabled);
            Assert.True(swipeCtrl.cancelButton.IsEnabled);

            Assert.True(_reader.Enabled);
            Assert.Equal(controller, _reader.Listener);
            Assert.NotNull(controller._memberDAO);
            Assert.Equal(EBorrowState.INITIALIZED, controller._state);

            // Run use case
            controller.cardSwiped(member.ID);

            // Test Post-conditions
            Assert.NotNull(restrictedCtrl);
            Assert.True(restrictedCtrl.IsEnabled);
            Assert.True(restrictedCtrl.cancelButton.IsEnabled);


            Assert.True(!_reader.Enabled);
            Assert.True(!_scanner.Enabled);

            Assert.Equal(member.ID, restrictedCtrl.idLabel.Content);
            Assert.Equal($"{member.FirstName} {member.LastName}", restrictedCtrl.nameLabel.Content.ToString());
            Assert.Equal(member.Loans[0].ToString(), restrictedCtrl.existingLoanBox.Text);  // Test one existing loan is present

            Assert.Equal("Borrower has overdue loans", restrictedCtrl.overDueLoanLabel.Content);
            Assert.Equal("Member has been restricted from borrowing", restrictedCtrl.errorMessage.Content);


            Assert.Equal(member, controller._borrower);
            Assert.Equal(EBorrowState.BORROWING_RESTRICTED, controller._state);
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
