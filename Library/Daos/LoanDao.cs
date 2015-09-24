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

            _helper = helper;

            this.LoanList = new List<ILoan>();
        }

        public List<ILoan> LoanList { get; }
        private ILoanHelper _helper { get; }

        public ILoan CreateLoan(IMember borrower, IBook book, DateTime borrowDate, DateTime dueDate)
        {
            if(borrower == null) throw new ArgumentException("A Member must be provided to create a loan");
            if(book == null) throw new ArgumentException("A Book must be provided to create a loan");

            var loan = _helper.MakeLoan(book, borrower, borrowDate, dueDate);

            return loan;
        }

        public void CommitLoan(ILoan loan)
        {
            var newId = this.LoanList.Count == 0 ? 1 : this.LoanList.Max(l => l.ID) + 1;

            loan.Commit(newId);

            this.LoanList.Add(loan);
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
