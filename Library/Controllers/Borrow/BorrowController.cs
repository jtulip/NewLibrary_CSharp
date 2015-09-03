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
        private IDisplay _display;
        private UserControl _previousDisplay;
        private ABorrowControl _ui;
        private ICardReader _reader;
        private ICardReaderListener _previousReaderListener;
        private IScanner _scanner;
        private IScannerListener _previousScannerListener;
        private IPrinter _printer;

        private IBookDAO _bookDAO;
        private ILoanDAO _loanDAO;
        private IMemberDAO _memberDAO;

        private IMember _borrower;
        private int scanCount = 0;
        private EBorrowState _state;

        private List<IBook> _bookList;
        private List<ILoan> _loanList;


        public BorrowController(IDisplay display, ICardReader reader, IScanner scanner, IPrinter printer,
                                    IBookDAO bookDAO, ILoanDAO loanDAO, IMemberDAO memberDAO)
        {
            _display = display;
            _reader = reader;
            _scanner = scanner;
            _printer = printer;


            _ui = new BorrowControl(this);

            _state = EBorrowState.CREATED;
        }


        public void initialise()
        {
            Console.WriteLine("BorrowController Initialising");
            _previousDisplay = _display.Display;
            Console.WriteLine("BorrowController Initialising, previous display = " + _previousDisplay);
            _display.Display = _ui;
            //setState(EBorrowState.INITIALIZED);
        }


        public void cancelled()
        {
            close();
            //setState(EBorrowState.CANCELLED);
        }

        public void cardSwiped(int memberID)
        {
            throw new ApplicationException("Not implemented yet");
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
            throw new ApplicationException("Not implemented yet");
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
