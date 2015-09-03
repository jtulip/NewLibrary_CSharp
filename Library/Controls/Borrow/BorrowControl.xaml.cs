using Library.Interfaces.Controllers.Borrow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Library.Controls.Borrow
{
    /// <summary>
    /// Interaction logic for BorrowControl.xaml
    /// </summary>
    public partial class BorrowControl : ABorrowControl
    {
        private IBorrowListener _listener;
        private Dictionary<EBorrowState, ABorrowControl> _controlDict;
        private ABorrowControl _currentControl;

        public BorrowControl(IBorrowListener listener)
        {
            _listener = listener;
            _controlDict = new Dictionary<EBorrowState, ABorrowControl>();

            addControl(new SwipeCardControl(listener),   EBorrowState.INITIALIZED);
            addControl(new ScanBookControl(listener),    EBorrowState.SCANNING_BOOKS);
            addControl(new RestrictedControl(listener),  EBorrowState.BORROWING_RESTRICTED);
            addControl(new ConfirmLoanControl(listener), EBorrowState.CONFIRMING_LOANS);
            //addControl(null, EBorrowState.CANCELLED);
            //addControl(null, EBorrowState.COMPLETED);

            InitializeComponent();
            State = EBorrowState.INITIALIZED;
        }

        private void addControl(ABorrowControl control, EBorrowState state)
        {
            _controlDict.Add(state, control);
        }

        private EBorrowState _state;
        public override EBorrowState State
        {
            get
            {
                return _state;
            }

            set
            {
                Panel.Children.Remove(_currentControl);
                if (_controlDict.Keys.Contains(value))
                {
                    _currentControl = _controlDict[value];
                }
                Panel.Children.Add(_currentControl);
                _state = value;
            }
        }

        public override void DisplayAtLoanLimitMessage()
        {
            _currentControl.DisplayAtLoanLimitMessage();
        }

        public override void DisplayConfirmingLoan(string loanDetails)
        {
            _currentControl.DisplayConfirmingLoan(loanDetails);
        }

        public override void DisplayErrorMessage(string errorMesg)
        {
            _currentControl.DisplayErrorMessage(errorMesg);
        }

        public override void DisplayExistingLoan(string loanDetails)
        {
            _currentControl.DisplayExistingLoan(loanDetails);
        }

        public override void DisplayMemberDetails(int memberID, string memberName, string memberPhone)
        {
            _currentControl.DisplayMemberDetails(memberID, memberName, memberPhone);
        }

        public override void DisplayOutstandingFineMessage(float amountOwing)
        {
            _currentControl.DisplayOutstandingFineMessage(amountOwing);
        }

        public override void DisplayOverDueMessage()
        {
            _currentControl.DisplayOverDueMessage();
        }

        public override void DisplayOverFineLimitMessage(float amountOwing)
        {
            _currentControl.DisplayOverFineLimitMessage(amountOwing);
        }

        public override void DisplayPendingLoan(string loanDetails)
        {
            _currentControl.DisplayPendingLoan(loanDetails);
        }

        public override void DisplayScannedBookDetails(string bookDetails)
        {
            _currentControl.DisplayScannedBookDetails(bookDetails);
        }
    }
}
