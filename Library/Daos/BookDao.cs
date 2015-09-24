using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Interfaces.Daos;
using Library.Interfaces.Entities;

namespace Library.Daos
{
    public class BookDao: IBookDAO
    {
        public BookDao(IBookHelper helper)
        {
            if(helper == null) throw new ArgumentException("Helper must be provided when creating BookDao");

            this.BookList = new List<IBook>();

            _helper = helper;
        }

        private IBookHelper _helper { get; set; }

        public IBook AddBook(string author, string title, string callNo)
        {
            var newId = this.BookList.Count == 0 ? 1 : this.BookList.Max(b => b.ID) + 1;  // If there is a book then set it to max(book id) + 1, otherwise start at 1.

            var book = _helper.MakeBook(author, title, callNo, newId);

            this.BookList.Add(book);

            return book;
        }

        public IBook GetBookByID(int id)
        {
            return this.BookList.SingleOrDefault(b => b.ID == id);
        }

        public List<IBook> BookList { get; internal set; }
        public List<IBook> FindBooksByAuthor(string author)
        {
            return this.BookList.Where(b => b.Author == author).ToList();
        }

        public List<IBook> FindBooksByTitle(string title)
        {
            throw new NotImplementedException();
        }

        public List<IBook> FindBooksByAuthorTitle(string author, string title)
        {
            throw new NotImplementedException();
        }
    }
}
