using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
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

            this.Author = author;
            this.Title = title;
            this.CallNumber = callNumber;
            this.ID = bookID;

            this.State = BookState.AVAILABLE;
        }

        public void Borrow(ILoan loan)
        {
            if(this.State != BookState.AVAILABLE) throw new InvalidOperationException("Cannot borrow a book that is not available");

            this.Loan = loan;

            this.State = BookState.ON_LOAN;
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
            if(this.State != BookState.ON_LOAN) throw new InvalidOperationException("Book must be on loan to be marked as lost");

            this.State = BookState.LOST;
        }

        public void Repair()
        {
            if(this.State != BookState.DAMAGED) throw new InvalidOperationException("Book is not damaged so cannot be repaired");

            this.State = BookState.AVAILABLE;
        }

        public void Dispose()
        {
            if(this.State != BookState.AVAILABLE && this.State != BookState.DAMAGED && this.State != BookState.LOST)
                throw new InvalidOperationException("Book cannot be disposed in its current state");

            this.State = BookState.DISPOSED;
        }

        public BookState State { get; internal set; }
        public string Author { get; }
        public string Title { get; }
        public string CallNumber { get; }
        public int ID { get; }

        public override string ToString()
        {
            return $"ID:\t\t{this.ID}\nCall Number:\t{this.CallNumber}\nAuthor:\t\t{this.Author}\nTitle:\t\t{this.Title}";
        }
    }
}
