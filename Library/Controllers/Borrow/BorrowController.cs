using Library.Controls.Borrow;
using Library.Interfaces.Controllers.Borrow;
using Library.Interfaces.Controls.Borrow;
using Library.Interfaces.Daos;
using Library.Interfaces.Entities;
using Library.Interfaces.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Library.Controllers.Borrow
{
    class BorrowController : IBorrowListener, ICardReaderListener, IScannerListener
    {
        internal IDisplay _display;
        internal UserControl _previousDisplay;
        internal ABorrowControl _ui;
        internal ICardReader _reader;
        internal ICardReaderListener _previousReaderListener;
        internal IScanner _scanner;
        internal IScannerListener _previousScannerListener;
        internal IPrinter _printer;

        internal IBookDAO _bookDAO;
        internal ILoanDAO _loanDAO;
        internal IMemberDAO _memberDAO;

        internal IMember _borrower;
        internal int scanCount = 0;
        internal EBorrowState _state;

        internal List<IBook> _bookList;
        internal List<ILoan> _loanList;


        public BorrowController(IDisplay display, ICardReader reader, IScanner scanner, IPrinter printer,
                                    IBookDAO bookDAO, ILoanDAO loanDAO, IMemberDAO memberDAO)
        {
            if(display == null) throw new ArgumentException("Display object was not provided to begin the application");
            if (reader == null) throw new ArgumentException("Reader object was not provided to begin the application");
            if (scanner == null) throw new ArgumentException("Scanner object was not provided to begin the application");
            if (printer == null) throw new ArgumentException("Printer object was not provided to begin the application");
            if (bookDAO == null) throw new ArgumentException("BookDAO object was not provided to begin the application");
            if (loanDAO == null) throw new ArgumentException("LoanDAO object was not provided to begin the application");
            if (memberDAO == null) throw new ArgumentException("MemberDAO object was not provided to begin the application");

            _display = display;
            _reader = reader;
            _scanner = scanner;
            _printer = printer;

            _bookDAO = bookDAO;
            _loanDAO = loanDAO;
            _memberDAO = memberDAO;
            
            _ui = new BorrowControl(this);

            _reader.Listener = this;
            _scanner.Listener = this;

            _state = EBorrowState.CREATED;
        }


        public void initialise()
        {
            Console.WriteLine("BorrowController Initialising");
            _previousDisplay = _display.Display;
            Console.WriteLine("BorrowController Initialising, previous display = " + _previousDisplay);
            _display.Display = _ui;

            _reader.Enabled = true;
            _scanner.Enabled = false;

            setState(EBorrowState.INITIALIZED);
        }


        public void cancelled()
        {
            close();
            //setState(EBorrowState.CANCELLED);
        }

        public void cardSwiped(int memberID)
        {
            //var member = _memberDAO.GetMemberByID(memberID);

            //if (member == null) return;

            //_borrower = member;

            _reader.Enabled = false;
            _scanner.Enabled = true;
            
            _state = EBorrowState.SCANNING_BOOKS;
        }

        public void bookScanned(int barcode)
        {
            throw new ApplicationException("Not implemented yet");
        }

        public void scansCompleted()
        {
            throw new ApplicationException("Not implemented yet");
        }

        public void loansConfirmed()
        {
            throw new ApplicationException("Not implemented yet");
        }

        public void loansRejected()
        {
            throw new ApplicationException("Not implemented yet");
        }


        private void setState(EBorrowState state)
        {
            _state = state;
        }


        public void close()
        {
            _display.Display  = _previousDisplay;
        }


        private string buildLoanListDisplay(List<ILoan> loanList)
        {
            StringBuilder bld = new StringBuilder();
            foreach (ILoan loan in loanList)
            {
                if (bld.Length > 0)
                {
                    bld.Append("\n\n");
                }
                bld.Append(loan.ToString());
            }
            return bld.ToString();
        }
    }
}
