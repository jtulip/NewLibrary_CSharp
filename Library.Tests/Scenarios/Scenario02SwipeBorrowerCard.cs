using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Controllers.Borrow;
using Library.Controls.Borrow;
using Library.Daos;
using Library.Entities;
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

            // Only mocking UI concerns
            _display = Substitute.For<IDisplay>();
            _reader = Substitute.For<ICardReader>();
            _scanner = Substitute.For<IScanner>();
            _printer = Substitute.For<IPrinter>();
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

            // Set the UI to the mock so we can test
            var borrowctrl = Substitute.For<ABorrowControl>();
            controller._ui = borrowctrl;


            controller.initialise();

            // Some test data initialisation
            var borrowDate = DateTime.Today;
            var dueDate = DateTime.Today.AddDays(7);

            var member = _memberDao.AddMember("Jim", "Tulip", "Phone", "Email");

            var book = _bookDao.AddBook("Jim Tulip", "Adventures in Programming", "call number");

            var loan = _loanDao.CreateLoan(member, book, borrowDate, dueDate);

            _loanDao.CommitLoan(loan);

            // Test Pre-conditions
            Assert.True(_reader.Enabled);
            Assert.Equal(controller, _reader.Listener);
            Assert.NotNull(controller._memberDAO);
            Assert.Equal(EBorrowState.INITIALIZED, controller._state);

            // Run use case
            controller.cardSwiped(member.ID);

            // Test Post-conditions
            Assert.True(!_reader.Enabled);
            Assert.True(_scanner.Enabled);

            borrowctrl.Received().DisplayMemberDetails(member.ID, $"{member.FirstName} {member.LastName}", member.ContactPhone);

            foreach (var l in member.Loans)
            {
                borrowctrl.Received().DisplayExistingLoan(l.ToString());
            }

            Assert.Equal(member, controller._borrower);
            Assert.Equal(EBorrowState.SCANNING_BOOKS, controller._state);

        }

        //[WpfFact]
        //public void RunMemberExistsAndRestricted()
        //{
        //    var controller = SetPreConditions();

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
