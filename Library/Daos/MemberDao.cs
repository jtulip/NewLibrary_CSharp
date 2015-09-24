using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Interfaces.Entities;

namespace Library.Daos
{
    public class MemberDao
    {
        public MemberDao(IMemberHelper helper)
        {
            if(helper == null) throw new ArgumentException("Helper must be provided when creating MemberDao");
        }
    }
}
