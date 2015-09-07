using System.Collections.Generic;
using Library.Interfaces.Entities;

namespace Library.Entities
{
    public class Member: IMember
    {
        public Member(string firstName, string lastName, string contactPhone, string emailAddress, int id)
        {
            this.ID = id;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.ContactPhone = contactPhone;
            this.EmailAddress = emailAddress;
        }

        public bool HasOverDueLoans { get; }
        public bool HasReachedLoanLimit { get; }
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