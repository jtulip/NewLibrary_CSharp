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
        private ILoan _loan;

        public Book(string author, string title, string callNumber, int bookID)
        {
            if (string.IsNullOrWhiteSpace(author)) throw new ArgumentException("Author needs to be provided.");
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title needs to be provided.");
            if (string.IsNullOrWhiteSpace(callNumber)) throw new ArgumentException("Call Number needs to be provided.");

            if (bookID <= 0) throw new ArgumentException("ID needs to be greater than 0.");

        }

        public void Borrow(ILoan loan)
        {
            if(this.State != BookState.AVAILABLE) throw new InvalidOperationException("Cannot borrow a book that is not available");

            this.Loan = loan;
        }

        public ILoan Loan
        {
            get
            {
                // Return null if book not ON_LOAN.
                return this.State != BookState.ON_LOAN ? null : _loan;
            }
            private set { _loan = value; }
        }

        public void ReturnBook(bool damaged)
        {
            if(this.State != BookState.ON_LOAN) throw new InvalidOperationException("Book is currently not on loan");

            this.State = damaged ? BookState.DAMAGED : BookState.AVAILABLE;

            this.Loan = null;
        }

        public void Lose()
        {
            this.State = BookState.LOST;
        }

        public void Repair()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public BookState State { get; internal set; }
        public string Author { get; }
        public string Title { get; }
        public string CallNumber { get; }
        public int ID { get; }
    }
}
