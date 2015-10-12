﻿using System;
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
        public List<IMember> MemberList { get; internal set; }

        public IMember AddMember(string firstName, string lastName, string contactPhone, string emailAddress)
        {
            var newId = this.MemberList.Count == 0 ? 1 : this.MemberList.Max(m => m.ID) + 1;  // If there is a member then set it to max(member id) + 1, otherwise start at 1.

            var member = _helper.MakeMember(firstName, lastName, contactPhone, emailAddress, newId);

            this.MemberList.Add(member);

            return member;
        }

        public IMember GetMemberByID(int id)
        {
            return this.MemberList.SingleOrDefault(m => m.ID == id);
        }
        
        public List<IMember> FindMembersByLastName(string lastName)
        {
            return this.MemberList.Where(m => m.LastName == lastName).ToList();
        }

        public List<IMember> FindMembersByEmailAddress(string emailAddress)
        {
            return this.MemberList.Where(m => m.EmailAddress == emailAddress).ToList();
        }

        public List<IMember> FindMembersByNames(string firstName, string lastName)
        {
            return this.MemberList.Where(m => m.FirstName == firstName && m.LastName == lastName).ToList();
        }
    }
}
