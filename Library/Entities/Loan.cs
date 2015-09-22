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
        public Loan(IBook book, IMember member, DateTime borrowDate, DateTime dueDate, int loanId)
        {
            if(book == null) throw new ArgumentException("Book needs to be provided");
            if(member == null) throw new ArgumentException("Member needs to be provided");
            if(borrowDate == DateTime.MinValue) throw new ArgumentException("Borrow date needs to be provided");
            if(dueDate == DateTime.MinValue) throw new ArgumentException("Due date needs to be provided");
            if(dueDate < borrowDate) throw new ArgumentException("Due date cannot be before Borrow date");
            if(loanId <= 0) throw new ArgumentException("ID must be greater than 0");
        }

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
