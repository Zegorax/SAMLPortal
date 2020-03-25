using System;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace SAMLPortal.Models
{
    public class GlobalSettings
    {
        public static string CompanyName { get; private set; }
        public static bool IsConfigured { get; private set; }
        public static bool IsInMaintenance { get; private set; }
        public static string LdapHost { get; private set; }
        public static string BindDn { get; private set; }
        public static string BindPass { get; private set; }
        public static string SearchBase { get; private set; }
        public static string UsersFilter { get; private set; }
        public static string AdministratorsFilter { get; private set; }
        public static string UidAttr { get; private set; }
        public static string MemberOfAttr { get; private set; }
        public static string DisplayNameAttr { get; private set; }
        public static string TemporaryPassword { get; set; }

        public GlobalSettings()
        {
        }

        public static void RefreshSetupSettings()
        {
            SAMLPortalContext context = new SAMLPortalContext();
            if (!context.Setup.Any())
            {
                Setup setup = context.Setup.Single();

                CompanyName = setup.CompanyName;
                IsConfigured = setup.IsConfigured;
                IsInMaintenance = setup.IsInMaintenance;
                LdapHost = setup.LdapHost;
                BindDn = setup.BindDn;
                BindPass = setup.BindPass;
                SearchBase = setup.SearchBase;
                UsersFilter = setup.UsersFilter;
                AdministratorsFilter = setup.AdministratorsFilter;
                UidAttr = setup.UidAttr;
                MemberOfAttr = setup.MemberOfAttr;
                DisplayNameAttr = setup.DisplayNameAttr;
            }
        }
    }
}
