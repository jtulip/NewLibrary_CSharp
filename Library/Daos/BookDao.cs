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
            var book = _helper.MakeBook(author, title, callNo, 1);

            this.BookList.Add(book);

            return book;
        }

        public IBook GetBookByID(int id)
        {
            throw new NotImplementedException();
        }

        public List<IBook> BookList { get; }
        public List<IBook> FindBooksByAuthor(string author)
        {
            throw new NotImplementedException();
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
