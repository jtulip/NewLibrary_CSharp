using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Daos;
using Library.Entities;
using Library.Interfaces.Daos;
using Library.Interfaces.Entities;
using Xunit;

namespace Library.Tests.Integration
{
    [Trait("Category", "Loan Tests")]
    public class LoanIntegration
    {
        [Fact]
        public void CanCreateLoan()
        {
            ILoanHelper loanHelper = new LoanHelper();
            ILoanDAO loanDao = new LoanDao(loanHelper);

            IMemberHelper memberHelper = new MemberHelper();
            IMemberDAO memberDao = new MemberDao(memberHelper);

            IBookHelper bookHelper = new BookHelper();
            IBookDAO bookDao = new BookDao(bookHelper);

            var borrowDate = DateTime.Today;
            var dueDate = DateTime.Today.AddDays(7);

            var member = memberDao.AddMember("Jim", "Tulip", "csu phone", "jim@example.com");

            var book = bookDao.AddBook("Jim Tulip", "Adventures in Programming", "call number");

            var loan = loanDao.CreateLoan(member, book, borrowDate, dueDate);

            Assert.NotNull(loan);

            Assert.Empty(loanDao.LoanList);

            Assert.Equal(book, loan.Book);
            Assert.Equal(member, loan.Borrower);

            Assert.Equal(0, loan.ID);
        }

        [Fact]
        public void CanCommitLoan()
        {
            ILoanHelper loanHelper = new LoanHelper();
            ILoanDAO loanDao = new LoanDao(loanHelper);

            IMemberHelper memberHelper = new MemberHelper();
            IMemberDAO memberDao = new MemberDao(memberHelper);

            IBookHelper bookHelper = new BookHelper();
            IBookDAO bookDao = new BookDao(bookHelper);

            var borrowDate = DateTime.Today;
            var dueDate = DateTime.Today.AddDays(7);

            var member = memberDao.AddMember("Jim", "Tulip", "csu phone", "jim@example.com");

            var book = bookDao.AddBook("Jim Tulip", "Adventures in Programming", "call number");

            var loan = loanDao.CreateLoan(member, book, borrowDate, dueDate);

            loanDao.CommitLoan(loan);

            Assert.NotNull(loan);

            Assert.Equal(1, loanDao.LoanList.Count);
            Assert.Equal(loan, loanDao.LoanList[0]);

            Assert.NotEqual(0, loan.ID);
        }

        [Fact]
        public void CanGetLoanByLoanID()
        {
            ILoanHelper loanHelper = new LoanHelper();
            ILoanDAO loanDao = new LoanDao(loanHelper);

            IMemberHelper memberHelper = new MemberHelper();
            IMemberDAO memberDao = new MemberDao(memberHelper);

            IBookHelper bookHelper = new BookHelper();
            IBookDAO bookDao = new BookDao(bookHelper);

            var borrowDate = DateTime.Today;
            var dueDate = DateTime.Today.AddDays(7);

            var member = memberDao.AddMember("Jim", "Tulip", "csu phone", "jim@example.com");

            var book = bookDao.AddBook("Jim Tulip", "Adventures in Programming", "call number");

            var loan = loanDao.CreateLoan(member, book, borrowDate, dueDate);

            loanDao.CommitLoan(loan);

            for (int i = 0; i < 10; i++)
            {
                var m = memberDao.AddMember("Test", "Test", "Test Phone", "Test Email");
                var b = bookDao.AddBook("Test", "Test", "Test");

                var l = loanDao.CreateLoan(m, b, borrowDate, dueDate);

                loanDao.CommitLoan(l);
            }

            var result = loanDao.GetLoanByID(loan.ID);

            Assert.Equal(loan, result);
        }

        [Fact]
        public void GetLoanByLoanIdReturnsNullIfNotFound()
        {
            ILoanHelper loanHelper = new LoanHelper();
            ILoanDAO loanDao = new LoanDao(loanHelper);

            IMemberHelper memberHelper = new MemberHelper();
            IMemberDAO memberDao = new MemberDao(memberHelper);

            IBookHelper bookHelper = new BookHelper();
            IBookDAO bookDao = new BookDao(bookHelper);

            var borrowDate = DateTime.Today;
            var dueDate = DateTime.Today.AddDays(7);

            var member = memberDao.AddMember("Jim", "Tulip", "csu phone", "jim@example.com");

            var book = bookDao.AddBook("Jim Tulip", "Adventures in Programming", "call number");

            var loan = loanDao.CreateLoan(member, book, borrowDate, dueDate);

            loanDao.CommitLoan(loan);

            for (int i = 0; i < 10; i++)
            {
                var m = memberDao.AddMember("Test", "Test", "Test Phone", "Test Email");
                var b = bookDao.AddBook("Test", "Test", "Test");

                var l = loanDao.CreateLoan(m, b, borrowDate, dueDate);

                loanDao.CommitLoan(l);
            }

            var result = loanDao.GetLoanByID(1000);

            Assert.Null(result);
        }

        public void CanGetLoanByBookTitle()
        {
            ILoanHelper loanHelper = new LoanHelper();
            ILoanDAO loanDao = new LoanDao(loanHelper);

            IMemberHelper memberHelper = new MemberHelper();
            IMemberDAO memberDao = new MemberDao(memberHelper);

            IBookHelper bookHelper = new BookHelper();
            IBookDAO bookDao = new BookDao(bookHelper);

            var borrowDate = DateTime.Today;
            var dueDate = DateTime.Today.AddDays(7);

            var member = memberDao.AddMember("Jim", "Tulip", "csu phone", "jim@example.com");

            var book = bookDao.AddBook("Jim Tulip", "Adventures in Programming", "call number");

            var loan = loanDao.CreateLoan(member, book, borrowDate, dueDate);

            loanDao.CommitLoan(loan);

            for (int i = 0; i < 10; i++)
            {
                var m = memberDao.AddMember("Test", "Test", "Test Phone", "Test Email");
                var b = bookDao.AddBook("Test", "Test", "Test");

                var l = loanDao.CreateLoan(m, b, borrowDate, dueDate);

                loanDao.CommitLoan(l);
            }

            var result = loanDao.FindLoansByBookTitle(book.Title);

            var single = result.Single();

            Assert.Equal(loan, single);
        }

        [Fact]
        public void GetLoanByBookTitleReturnsEmptyCollectionIfNotFound()
        {
            ILoanHelper loanHelper = new LoanHelper();
            ILoanDAO loanDao = new LoanDao(loanHelper);

            IMemberHelper memberHelper = new MemberHelper();
            IMemberDAO memberDao = new MemberDao(memberHelper);

            IBookHelper bookHelper = new BookHelper();
            IBookDAO bookDao = new BookDao(bookHelper);

            var borrowDate = DateTime.Today;
            var dueDate = DateTime.Today.AddDays(7);

            var member = memberDao.AddMember("Jim", "Tulip", "csu phone", "jim@example.com");

            var book = bookDao.AddBook("Jim Tulip", "Adventures in Programming", "call number");

            var loan = loanDao.CreateLoan(member, book, borrowDate, dueDate);

            loanDao.CommitLoan(loan);

            for (int i = 0; i < 10; i++)
            {
                var m = memberDao.AddMember("Test", "Test", "Test Phone", "Test Email");
                var b = bookDao.AddBook("Test", "Test", "Test");

                var l = loanDao.CreateLoan(m, b, borrowDate, dueDate);

                loanDao.CommitLoan(l);
            }

            var result = loanDao.FindLoansByBookTitle("No Title");

            Assert.Empty(result);
        }

        public void CanGetLoanByBorrower()
        {
            ILoanHelper loanHelper = new LoanHelper();
            ILoanDAO loanDao = new LoanDao(loanHelper);

            IMemberHelper memberHelper = new MemberHelper();
            IMemberDAO memberDao = new MemberDao(memberHelper);

            IBookHelper bookHelper = new BookHelper();
            IBookDAO bookDao = new BookDao(bookHelper);

            var borrowDate = DateTime.Today;
            var dueDate = DateTime.Today.AddDays(7);

            var member = memberDao.AddMember("Jim", "Tulip", "csu phone", "jim@example.com");

            var book = bookDao.AddBook("Jim Tulip", "Adventures in Programming", "call number");

            var loan = loanDao.CreateLoan(member, book, borrowDate, dueDate);

            loanDao.CommitLoan(loan);

            for (int i = 0; i < 10; i++)
            {
                var m = memberDao.AddMember("Test", "Test", "Test Phone", "Test Email");
                var b = bookDao.AddBook("Test", "Test", "Test");

                var l = loanDao.CreateLoan(m, b, borrowDate, dueDate);

                loanDao.CommitLoan(l);
            }

            var result = loanDao.FindLoansByBorrower(member);

            var single = result.Single();

            Assert.Equal(loan, single);
        }

        [Fact]
        public void GetLoanByBorrowerReturnsEmptyCollectionIfNotFound()
        {
            ILoanHelper loanHelper = new LoanHelper();
            ILoanDAO loanDao = new LoanDao(loanHelper);

            IMemberHelper memberHelper = new MemberHelper();
            IMemberDAO memberDao = new MemberDao(memberHelper);

            IBookHelper bookHelper = new BookHelper();
            IBookDAO bookDao = new BookDao(bookHelper);

            var borrowDate = DateTime.Today;
            var dueDate = DateTime.Today.AddDays(7);

            var member = memberDao.AddMember("Jim", "Tulip", "csu phone", "jim@example.com");

            var book = bookDao.AddBook("Jim Tulip", "Adventures in Programming", "call number");

            var loan = loanDao.CreateLoan(member, book, borrowDate, dueDate);

            loanDao.CommitLoan(loan);

            for (int i = 0; i < 10; i++)
            {
                var m = memberDao.AddMember("Test", "Test", "Test Phone", "Test Email");
                var b = bookDao.AddBook("Test", "Test", "Test");

                var l = loanDao.CreateLoan(m, b, borrowDate, dueDate);

                loanDao.CommitLoan(l);
            }

            var result = loanDao.FindLoansByBorrower(new Member("Test", "Test", "Test", "Test", 100));

            Assert.Empty(result);
        }

        [Fact]
        public void CanUpdateOverdueStatus()
        {
            ILoanHelper loanHelper = new LoanHelper();
            ILoanDAO loanDao = new LoanDao(loanHelper);

            IMemberHelper memberHelper = new MemberHelper();
            IMemberDAO memberDao = new MemberDao(memberHelper);

            IBookHelper bookHelper = new BookHelper();
            IBookDAO bookDao = new BookDao(bookHelper);

            var borrowDate = DateTime.Today;
            var dueDate = DateTime.Today.AddDays(7);

            var member = memberDao.AddMember("Jim", "Tulip", "csu phone", "jim@example.com");

            var book = bookDao.AddBook("Jim Tulip", "Adventures in Programming", "call number");

            var loan = loanDao.CreateLoan(member, book, borrowDate, dueDate);

            loanDao.CommitLoan(loan);

            Assert.Equal(LoanState.CURRENT, loan.State);

            loanDao.UpdateOverDueStatus(DateTime.Today.AddMonths(1));

            Assert.Equal(LoanState.OVERDUE, loan.State);
        }

        [Fact]
        public void CanGetOverdueLoans()
        {
            ILoanHelper loanHelper = new LoanHelper();
            ILoanDAO loanDao = new LoanDao(loanHelper);

            IMemberHelper memberHelper = new MemberHelper();
            IMemberDAO memberDao = new MemberDao(memberHelper);

            IBookHelper bookHelper = new BookHelper();
            IBookDAO bookDao = new BookDao(bookHelper);

            var borrowDate = DateTime.Today;
            var dueDate = DateTime.Today.AddDays(7);

            var member = memberDao.AddMember("Jim", "Tulip", "csu phone", "jim@example.com");

            var book = bookDao.AddBook("Jim Tulip", "Adventures in Programming", "call number");

            var loan = loanDao.CreateLoan(member, book, borrowDate, dueDate);

            loanDao.CommitLoan(loan);

            Assert.Equal(LoanState.CURRENT, loan.State);

            loanDao.UpdateOverDueStatus(DateTime.Today.AddMonths(1));

            Assert.Equal(LoanState.OVERDUE, loan.State);

            for (int i = 0; i < 10; i++)
            {
                var m = memberDao.AddMember("Test", "Test", "Test Phone", "Test Email");
                var b = bookDao.AddBook("Test", "Test", "Test");

                var l = loanDao.CreateLoan(m, b, borrowDate, dueDate);

                loanDao.CommitLoan(l);
            }

            var overdue = loanDao.FindOverDueLoans();

            Assert.Equal(1, overdue.Count);
            Assert.Equal(loan, overdue[0]);
        }
    }
}
