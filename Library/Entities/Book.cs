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
        public Book(string author, string title, string callNumber, int bookID)
        {
            if (string.IsNullOrWhiteSpace(author)) throw new ArgumentException("Author needs to be provided.");
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title needs to be provided.");
            if (string.IsNullOrWhiteSpace(callNumber)) throw new ArgumentException("Call Number needs to be provided.");

            if (bookID <= 0) throw new ArgumentException("ID needs to be greater than 0.");

        }

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
