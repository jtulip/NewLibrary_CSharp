using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Interfaces.Daos;
using Library.Interfaces.Entities;

namespace Library.Daos
{
    public class MemberDao: IMemberDAO
    {
        public MemberDao(IMemberHelper helper)
        {
            if(helper == null) throw new ArgumentException("Helper must be provided when creating MemberDao");

            _helper = helper;

            this.MemberList = new List<IMember>();
        }

        private IMemberHelper _helper { get; }
        public List<IMember> MemberList { get; }

        public IMember AddMember(string firstName, string lastName, string contactPhone, string emailAddress)
        {
            var member = _helper.MakeMember(firstName, lastName, contactPhone, emailAddress, 1);

            this.MemberList.Add(member);

            return member;
        }

        public IMember GetMemberByID(int id)
        {
            throw new NotImplementedException();
        }
        
        public List<IMember> FindMembersByLastName(string lastName)
        {
            throw new NotImplementedException();
        }

        public List<IMember> FindMembersByEmailAddress(string emailAddress)
        {
            throw new NotImplementedException();
        }

        public List<IMember> FindMembersByNames(string firstName, string lastName)
        {
            throw new NotImplementedException();
        }
    }
}
