﻿using Library.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Entities
{
    public class MemberHelper : IMemberHelper
    {
        public IMember MakeMember(string firstName, string lastName, string contactPhone, string emailAddress, int id)
        {
            return new Member(firstName, lastName, contactPhone, emailAddress, id);
        }
    }
}
