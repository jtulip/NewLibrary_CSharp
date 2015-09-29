﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Daos;
using Library.Entities;
using Library.Interfaces.Entities;
using Xunit;

namespace Library.Tests.Integration
{
    [Trait("Category", "Book Tests")]
    public class BookIntegration
    {
        [Fact]
        public void CanCreateBook()
        {
            IBookHelper helper = new BookHelper();

            var dao = new BookDao(helper);

            var author = "author";
            var title = "title";
            var callNumber = "call number";

            var book = dao.AddBook(author, title, callNumber);

            Assert.NotNull(book);

            Assert.NotEmpty(dao.BookList);
            Assert.Equal(1, dao.BookList.Count);
            Assert.Equal(book, dao.BookList[0]);

            Assert.Equal(author, book.Author);
            Assert.Equal(title, book.Title);
            Assert.Equal(callNumber, book.CallNumber);

            Assert.NotEqual(0, book.ID);
        }
    }
}