using Library.Interfaces.Controls.Borrow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Library.Interfaces.Controllers.Borrow;

namespace Library.Controls.Borrow
{
    public abstract class ABorrowControl : UserControl, IBorrowUI
    {
        public virtual EBorrowState State
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public abstract void DisplayAtLoanLimitMessage();


        public abstract void DisplayConfirmingLoan(string loanDetails);


        public abstract void DisplayErrorMessage(string errorMesg);


        public abstract void DisplayExistingLoan(string loanDetails);


        public abstract void DisplayMemberDetails(int memberID, string memberName, string memberPhone);


        public abstract void DisplayOutstandingFineMessage(float amountOwing);


        public abstract void DisplayOverDueMessage();


        public abstract void DisplayOverFineLimitMessage(float amountOwing);


        public abstract void DisplayPendingLoan(string loanDetails);


        public abstract void DisplayScannedBookDetails(string bookDetails);


    }
}
