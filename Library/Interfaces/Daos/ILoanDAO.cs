using Library.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Interfaces.Daos
{
    public interface ILoanDAO
    {
        ILoan CreateLoan(IMember borrower, IBook book, DateTime borrowDate, DateTime dueDate);

        // Next, there is an error in the commitLoan specification of the ILoanDAO interface, which incorrectly states that 
        // commitLoan returns a reference to an ILoan, when in fact it should be a void method. 
        //The correct implementation is specified in the interface file.
        void CommitLoan(ILoan loan);

        ILoan GetLoanByID(int id);

        List<ILoan> LoanList
        {
            get;
        }

        List<ILoan> FindLoansByBorrower(IMember borrower);

        List<ILoan> FindLoansByBookTitle(string title);

        void UpdateOverDueStatus(DateTime currentDate);

        List<ILoan> FindOverDueLoans();
    }

}
