using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Interfaces.Entities;

namespace Library.Entities
{
    public class BookHelper: IBookHelper
    {
        public IBook MakeBook(string author, string title, string callNumber, int id)
        {
            throw new NotImplementedException();
        }
    }
}
