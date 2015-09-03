using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Interfaces.Entities
{
    public interface ILoanHelper
    {
        ILoan MakeLoan(IBook book, IMember borrower, DateTime borrowDate, DateTime dueDate);
    }
}
