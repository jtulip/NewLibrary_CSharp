using System;
using System.Collections.Generic;
using System.Linq;
using Library.Interfaces.Entities;

namespace Library.Entities
{
    public class Member: IMember
    {
        public Member(string firstName, string lastName, string contactPhone, string emailAddress, int id)
        {
            if (string.IsNullOrWhiteSpace(firstName)) throw new ArgumentException("First Name needs to be provided.");
            if (string.IsNullOrWhiteSpace(lastName)) throw new ArgumentException("Last Name needs to be provided.");
            if (string.IsNullOrWhiteSpace(contactPhone)) throw new ArgumentException("Contact Phone needs to be provided.");
            if (string.IsNullOrWhiteSpace(emailAddress)) throw new ArgumentException("Email Address needs to be provided.");

            if (id <= 0) throw new ArgumentException("ID needs to be greater than 0.");

            this.ID = id;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.ContactPhone = contactPhone;
            this.EmailAddress = emailAddress;

            // Instantiate loan collection
            this.Loans = new List<ILoan>();
        }

        public bool HasOverDueLoans => this.Loans.Any(l => l.IsOverDue); // Return true if any loan is overdue.
        public bool HasReachedLoanLimit => this.Loans.Count() == BookConstants.LOAN_LIMIT;
        public bool HasFinesPayable => this.FineAmount > 0;
        public bool HasReachedFineLimit => this.FineAmount >= BookConstants.FINE_LIMIT;
        public float FineAmount { get; private set; }
        public void AddFine(float fine)
        {
            if(fine < 0) throw new ArgumentException("Fine must not be negative value");

            this.FineAmount += fine;
        }

        public void PayFine(float payment)
        {
            if(payment < 0) throw new ArgumentException("Payment must not be negative value");
            if(payment > this.FineAmount) throw new ArgumentException($"Payment must not exceed fines of {this.FineAmount}");

            this.FineAmount -= payment;
        }

        public void AddLoan(ILoan loan)
        {
            if(loan == null) throw new ArgumentException("Loan cannot be null");

            if (this.State == MemberState.BORROWING_DISALLOWED)
                throw new InvalidOperationException("Cannot add a loan when member is not allowed to borrow");

            this.Loans.Add(loan);
        }

        public List<ILoan> Loans { get; private set; }
        public void RemoveLoan(ILoan loan)
        {
            this.Loans.Remove(loan);
        }

        public string FirstName { get; }
        public string LastName { get; }
        public string ContactPhone { get; }
        public string EmailAddress { get; }
        public int ID { get; }
        public MemberState State { get; internal set; }
    }
}