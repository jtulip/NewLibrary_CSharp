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

    }
}
