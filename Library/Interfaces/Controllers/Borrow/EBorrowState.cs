using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Interfaces.Controllers.Borrow
{
    public enum EBorrowState { CREATED, INITIALIZED, SCANNING_BOOKS, CONFIRMING_LOANS, COMPLETED, BORROWING_RESTRICTED, CANCELLED }
}
