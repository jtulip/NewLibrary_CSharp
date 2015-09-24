using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Interfaces.Daos;
using Library.Interfaces.Entities;

namespace Library.Daos
{
    public class LoanDao: ILoanDAO
    {
        public LoanDao(ILoanHelper helper)
        {
            if(helper == null) throw new ArgumentException("Helper must be provided when creating LoanDao");
        }

        public List<ILoan> LoanList { get; }

        public ILoan CreateLoan(IMember borrower, IBook book, DateTime borrowDate, DateTime dueDate)
        {
            throw new NotImplementedException();
        }

        public void CommitLoan(ILoan loan)
        {
            throw new NotImplementedException();
        }

        public ILoan GetLoanByID(int id)
        {
            throw new NotImplementedException();
        }
        
        public List<ILoan> FindLoansByBorrower(IMember borrower)
        {
            throw new NotImplementedException();
        }

        public List<ILoan> FindLoansByBookTitle(string title)
        {
            throw new NotImplementedException();
        }

        public void UpdateOverDueStatus(DateTime currentDate)
        {
            throw new NotImplementedException();
        }

        public List<ILoan> FindOverDueLoans()
        {
            throw new NotImplementedException();
        }
    }
}
