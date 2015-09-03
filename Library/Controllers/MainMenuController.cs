using Library.Controllers.Borrow;
using Library.Controls;
using Library.Interfaces.Controllers;
using Library.Interfaces.Daos;
using Library.Interfaces.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Controllers
{
    class MainMenuController : IMainMenuListener
    {
        private IDisplay _display;
        private ICardReader _reader;
        private IScanner _scanner;
        private IPrinter _printer;

        private IBookDAO _bookDAO;
        private ILoanDAO _loanDAO;
        private IMemberDAO _memberDAO;


        public MainMenuController(IDisplay display, ICardReader reader, IScanner scanner, IPrinter printer,
                                    IBookDAO bookDAO, ILoanDAO loanDAO, IMemberDAO memberDAO)
        {
            _display = display;
            _reader = reader;
            _scanner = scanner;
            _printer = printer;

            _bookDAO = bookDAO;
            _loanDAO = loanDAO;
            _memberDAO = memberDAO;
        }

        public void initialise()
        {
            MainMenuControl mainMenuControl = new MainMenuControl(this);
            _display.Display = mainMenuControl;
        }

        public void borrowBook()
        {
            BorrowController borrowController = new BorrowController(_display, _reader, _scanner, _printer,
                                                                     _bookDAO, _loanDAO, _memberDAO);
            borrowController.initialise();
        }
    }
}
