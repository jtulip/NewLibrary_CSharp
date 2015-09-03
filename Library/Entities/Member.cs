using System.Collections.Generic;
using Library.Interfaces.Entities;

namespace Library.Entities
{
    public class Member: IMember
    {
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

        public List<ILoan> Loans { get; }
        public void RemoveLoan(ILoan loan)
        {
            throw new System.NotImplementedException();
        }

        public string FirstName { get; }
        public string LastName { get; }
        public string ContactPhone { get; }
        public string EmailAddress { get; }
        public int ID { get; }
        public MemberState State { get; }
    }
}