using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Interfaces.Entities
{
    public interface ILoan
    {
        void Commit(int loanID);

        void Complete();

        bool IsOverDue
        {
            get;
        }

        bool CheckOverDue(DateTime currentDate);

        IMember Borrower
        {
            get;
        }

        IBook Book
        {
            get;
        }

        int ID
        {
            get;
        }

        LoanState State
        {
            get;
        }
    }

}
