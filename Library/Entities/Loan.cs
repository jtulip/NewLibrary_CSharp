﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Interfaces.Entities;

namespace Library.Entities
{
    public class Loan: ILoan
    {
        public Loan(IBook book, IMember member, DateTime borrowDate, DateTime dueDate)
        {
            if(book == null) throw new ArgumentException("Book needs to be provided");
            if(member == null) throw new ArgumentException("Member needs to be provided");
            if(borrowDate == DateTime.MinValue) throw new ArgumentException("Borrow date needs to be provided");
            if(dueDate == DateTime.MinValue) throw new ArgumentException("Due date needs to be provided");
            if(dueDate < borrowDate) throw new ArgumentException("Due date cannot be before Borrow date");

            this.Book = book;
            this.Borrower = member;
            this.DueDate = dueDate;
            this.BorrowDate = borrowDate;

            this.ID = 0;
        }

        public void Commit(int loanID)
        {
            if(this.State != LoanState.PENDING) throw new InvalidOperationException("Loan cannot be committed unless state is Pending");

            this.State = LoanState.CURRENT;

            this.Book.Borrow(this);
            this.Borrower.AddLoan(this);

            this.ID = loanID;
        }

        public void Complete()
        {
            if(this.State != LoanState.CURRENT && this.State != LoanState.OVERDUE) throw new InvalidOperationException("Cannot complete a loan if it's not Current or Overdue");

            this.State = LoanState.COMPLETE;
        }

        public bool IsOverDue => this.State == LoanState.OVERDUE;  // Return true if LoanState is Overdue.

        public bool CheckOverDue(DateTime currentDate)
        {
            if(this.State != LoanState.CURRENT && this.State != LoanState.OVERDUE) throw new InvalidOperationException("Cannot check Over Due if Loan is not Current or Overdue");

            if (this.DueDate >= currentDate) return false;

            this.State = LoanState.OVERDUE;

            return true;
        }

        public IMember Borrower { get; }
        public IBook Book { get; }
        public int ID { get; private set; }
        public LoanState State { get; internal set; }
        public DateTime DueDate { get; private set; }
        public DateTime BorrowDate { get; private set; }

        public override string ToString()
        {
            return $"Loan ID:\t\t{this.ID}\nAuthor:\t\t{this.Book.Author}\nTitle:\t\t{this.Book.Title}\nBorrower:\t{this.Borrower.ToString()}\nBorrow Date:\t{this.BorrowDate.ToShortDateString()}\nDue Date:\t{this.DueDate.ToShortDateString()}";
        }
    }
}
