using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace SAMLPortal.Models
{
    public class GlobalSettings
    {
        public string CompanyName { get; set; }
        public string AppUrl { get; set; }
        public bool IsConfigured { get; set; }
        public bool IsInMaintenance { get; set; }
        public string LdapHost { get; set; }
        public int LdapPort { get; set; }
        public string BindDn { get; set; }
        public string BindPass { get; set; }
        public string SearchBase { get; set; }
        public string UsersFilter { get; set; }
        public string AdministratorsFilter { get; set; }
        public string UidAttr { get; set; }
        public string MemberOfAttr { get; set; }
        public string DisplayNameAttr { get; set; }

        //public static void RefreshSetupSettings()
        //{
        //    SAMLPortalContext context = new SAMLPortalContext();
        //    if (!context.Setup.Any())
        //    {
        //        Setup setup = context.Setup.Single();

        //        CompanyName = setup.CompanyName;
        //        IsConfigured = setup.IsConfigured;
        //        IsInMaintenance = setup.IsInMaintenance;
        //        LdapHost = setup.LdapHost;
        //        LdapPort = setup.LdapPort;
        //        BindDn = setup.BindDn;
        //        BindPass = setup.BindPass;
        //        SearchBase = setup.SearchBase;
        //        UsersFilter = setup.UsersFilter;
        //        AdministratorsFilter = setup.AdministratorsFilter;
        //        UidAttr = setup.UidAttr;
        //        MemberOfAttr = setup.MemberOfAttr;
        //        DisplayNameAttr = setup.DisplayNameAttr;
        //    }
        //}

        //public static void StoreSettings()
        //{
        //    SAMLPortalContext context = new SAMLPortalContext();
        //    if (!context.Setup.Any())
        //    {
        //        Setup setup = context.Setup.Single();

        //        setup.CompanyName = CompanyName;
        //        setup.IsConfigured = IsConfigured;
        //        setup.IsInMaintenance =IsInMaintenance;
        //        setup.LdapHost = LdapHost;
        //        setup.LdapPort = LdapPort;
        //        setup.BindDn = BindDn;
        //        setup.BindPass = BindPass;
        //        setup.SearchBase = SearchBase;
        //        setup.UsersFilter = UsersFilter;
        //        setup.AdministratorsFilter = AdministratorsFilter;
        //        setup.UidAttr = UidAttr;
        //        setup.MemberOfAttr = MemberOfAttr;
        //        setup.DisplayNameAttr = DisplayNameAttr;

        //        context.Add(setup);
        //        context.SaveChanges();
        //        RefreshSetupSettings();
        //    }
        //}
    }
}
