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
    /// Interaction logic for RestrictedControl.xaml
    /// </summary>
    public partial class RestrictedControl : ABorrowControl
    {
        private IBorrowListener _listener;

        public RestrictedControl(IBorrowListener listener)
        {
            _listener = listener;
            InitializeComponent();
        }

        public override void DisplayMemberDetails(int memberID, string memberName, string memberPhone)
        {
            idLabel.Content = memberID;
            nameLabel.Content = memberName;
            contactLabel.Content = memberPhone;
        }


        public override void DisplayExistingLoan(string loanDetails)
        {
            existingLoanBox.Text = loanDetails;
            existingLoanBox.ScrollToLine(existingLoanBox.LineCount - 1);
        }


        public override void DisplayOverDueMessage()
        {
            overDueLoanLabel.Content = "Borrower has overdue loans";
        }


        public override void DisplayAtLoanLimitMessage()
        {
            overDueLoanLabel.Content = "Borrower has reached maximum number of borrowed items";
        }


        public override void DisplayOutstandingFineMessage(float amountOwing)
        {
            outstandingFineLabel.Content =
                String.Format("Borrower has outstanding fines. Amount owing: ${0:0.00}", amountOwing);
        }


        public override void DisplayOverFineLimitMessage(float amountOwing)
        {
            outstandingFineLabel.Content =
                String.Format("Borrower has exceeded fine limit. Amount owing: ${0:0.00}", amountOwing);
        }


        public override void DisplayScannedBookDetails(string bookDetails)
        {
            throw new NotImplementedException();
        }


        public override void DisplayPendingLoan(string loanDetails)
        {
            throw new NotImplementedException();
        }


        public override void DisplayConfirmingLoan(string loanDetails)
        {
            throw new NotImplementedException();
        }


        public override void DisplayErrorMessage(string errorMesg)
        {
            errorMessage.Content = errorMesg;
        }


        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            _listener.cancelled();
        }

        private void confirmButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
