using Library.Interfaces.Controllers.Borrow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Interfaces.Controls.Borrow
{
    public interface IBorrowUI
    {
        void DisplayMemberDetails(int memberID, string memberName, string memberPhone);

        void DisplayExistingLoan(string loanDetails);

        void DisplayOverDueMessage();

        void DisplayAtLoanLimitMessage();

        void DisplayOutstandingFineMessage(float amountOwing);

        void DisplayOverFineLimitMessage(float amountOwing);

        void DisplayScannedBookDetails(String bookDetails);

        void DisplayPendingLoan(string loanDetails);

        void DisplayConfirmingLoan(string loanDetails);

        void DisplayErrorMessage(string errorMesg);

        EBorrowState State { get; set; }

    }
}
