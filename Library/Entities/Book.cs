using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Interfaces.Entities;

namespace Library.Entities
{
    public class Book: IBook
    {
        public void Borrow(ILoan loan)
        {
            throw new NotImplementedException();
        }

        public ILoan Loan { get; }
        public void ReturnBook(bool damaged)
        {
            throw new NotImplementedException();
        }

        public void Lose()
        {
            throw new NotImplementedException();
        }

        public void Repair()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public BookState State { get; }
        public string Author { get; }
        public string Title { get; }
        public string CallNumber { get; }
        public int ID { get; }
    }
}
