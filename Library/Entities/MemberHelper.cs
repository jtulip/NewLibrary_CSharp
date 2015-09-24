using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Interfaces.Entities;

namespace Library.Entities
{
    public class MemberHelper: IMemberHelper
    {
        public IMember MakeMember(string firstName, string lastName, string contactPhone, string emailAddress, int id)
        {
            throw new NotImplementedException();
        }
    }
}
