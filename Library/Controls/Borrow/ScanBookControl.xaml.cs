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
using Library.Interfaces.Controllers.Borrow;

namespace Library.Controls.Borrow
{
    /// <summary>
    /// Interaction logic for ScanBookControl.xaml
    /// </summary>
    public partial class ScanBookControl : ABorrowControl
    {
        private IBorrowListener _listener;

        public ScanBookControl(IBorrowListener listener)
        {
            _listener = listener;
            InitializeComponent();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            _listener.cancelled();
        }

        private void completeButton_Click(object sender, RoutedEventArgs e)
        {
            _listener.scansCompleted();
        }

        public override void DisplayAtLoanLimitMessage()
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

        public override void DisplayExistingLoan(string loanDetails)
        {
            existingLoanBox.Text = loanDetails;
            existingLoanBox.ScrollToLine(existingLoanBox.LineCount - 1);
        }

        public override void DisplayMemberDetails(int memberID, string memberName, string memberPhone)
        {
            idLabel.Content = memberID;
            nameLabel.Content = memberName;
            contactLabel.Content = memberPhone;
        }

        public override void DisplayOutstandingFineMessage(float amountOwing)
        {
            outstandingFineLabel.Content =
                String.Format("Borrower has outstanding fines. Amount owing: ${0:0.00}", amountOwing);
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
            pendingLoanBox.Text = loanDetails;
            pendingLoanBox.ScrollToLine(pendingLoanBox.LineCount-1);
        }

        public override void DisplayScannedBookDetails(string bookDetails)
        {
            currentbookBox.Text = bookDetails;
        }
    }
}
