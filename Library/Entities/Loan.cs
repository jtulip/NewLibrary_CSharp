using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Interfaces.Entities;

namespace Library.Entities
{
    public class Loan: ILoan
    {
        public void Commit(int loanID)
        {
            throw new NotImplementedException();
        }

        public void Complete()
        {
            throw new NotImplementedException();
        }

        public bool IsOverDue { get; }
        public bool CheckOverDue(DateTime currentDate)
        {
            throw new NotImplementedException();
        }

        public IMember Borrower { get; }
        public IBook Book { get; }
        public int ID { get; }
        public LoanState State { get; }
    }
}
