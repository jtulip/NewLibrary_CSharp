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

            this.ID = 0;
        }

        public void Commit(int loanID)
        {
            if(this.State != LoanState.PENDING) throw new InvalidOperationException("Loan cannot be committed unless state is Pending");

            this.State = LoanState.CURRENT;

            this.Book.Borrow(this);
            this.Borrower.AddLoan(this);
        }

        public void Complete()
        {
            if(this.State == LoanState.CURRENT || this.State == LoanState.OVERDUE) throw new InvalidOperationException("Cannot complete a loan if it's Current or Overdue");

            this.State = LoanState.COMPLETE;
        }

        public bool IsOverDue { get; }
        public bool CheckOverDue(DateTime currentDate)
        {
            throw new NotImplementedException();
        }

        public IMember Borrower { get; }
        public IBook Book { get; }
        public int ID { get; }
        public LoanState State { get; internal set; }
    }
}
