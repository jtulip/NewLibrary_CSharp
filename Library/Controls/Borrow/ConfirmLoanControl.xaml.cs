using Library.Interfaces.Controllers.Borrow;
using Library.Interfaces.Controls.Borrow;
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
    /// Interaction logic for ConfirmLoanControl.xaml
    /// </summary>
    public partial class ConfirmLoanControl : ABorrowControl
    {
        private IBorrowListener _listener;

        public ConfirmLoanControl(IBorrowListener listener)
        {
            _listener = listener;
            InitializeComponent();
        }


        private void confirmButton_Click(object sender, RoutedEventArgs e)
        {
            _listener.loansConfirmed();
        }

        private void rejectButton_Click(object sender, RoutedEventArgs e)
        {
            _listener.loansRejected();
        }


        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            _listener.cancelled();
        }


        public override void DisplayAtLoanLimitMessage()
        {
            throw new NotImplementedException();
        }

        public override void DisplayConfirmingLoan(string loanDetails)
        {
            confirmLoanBox.Text = loanDetails;
            confirmLoanBox.ScrollToLine(confirmLoanBox.LineCount - 1);
        }

        public override void DisplayErrorMessage(string errorMesg)
        {
            throw new NotImplementedException();
        }

        public override void DisplayExistingLoan(string loanDetails)
        {
            throw new NotImplementedException();
        }

        public override void DisplayMemberDetails(int memberID, string memberName, string memberPhone)
        {
            throw new NotImplementedException();
        }

        public override void DisplayOutstandingFineMessage(float amountOwing)
        {
            throw new NotImplementedException();
        }

        public override void DisplayOverDueMessage()
        {
            throw new NotImplementedException();
        }

        public override void DisplayOverFineLimitMessage(float amountOwing)
        {
            throw new NotImplementedException();
        }

        public override void DisplayPendingLoan(string loanDetails)
        {
            throw new NotImplementedException();
        }

        public override void DisplayScannedBookDetails(string bookDetails)
        {
            throw new NotImplementedException();
        }
    }
}
