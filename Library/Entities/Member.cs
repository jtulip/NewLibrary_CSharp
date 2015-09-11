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
        public bool HasFinesPayable { get; }
        public bool HasReachedFineLimit { get; }
        public float FineAmount { get; }
        public void AddFine(float fine)
        {
            throw new System.NotImplementedException();
        }

        public void PayFine(float payment)
        {
            throw new System.NotImplementedException();
        }

        public void AddLoan(ILoan loan)
        {
            throw new System.NotImplementedException();
        }

        public List<ILoan> Loans { get; internal set; }
        public void RemoveLoan(ILoan loan)
        {
            throw new System.NotImplementedException();
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ContactPhone { get; set; }
        public string EmailAddress { get; set; }
        public int ID { get; set; }
        public MemberState State { get; set; }
    }
}