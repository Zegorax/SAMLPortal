using System;
namespace SAMLPortal.Models
{
    public class Setup
    {
        public virtual int Id { get; set; }
        public virtual string CompanyName { get; set; }
        public virtual bool IsConfigured { get; set; }
        public virtual bool IsInMaintenance { get; set; }
        public virtual string LdapHost { get; set; }
        public virtual int LdapPort { get; set; }
        public virtual string BindDn { get; set; }
        public virtual string BindPass { get; set; }
        public virtual string SearchBase { get; set; }
        public virtual string UsersFilter { get; set; }
        public virtual string AdministratorsFilter { get; set; }
        public virtual string UidAttr { get; set; }
        public virtual string MemberOfAttr { get; set; }
        public virtual string DisplayNameAttr { get; set; }

        public Setup()
        {
        }
    }
}
