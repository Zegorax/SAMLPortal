using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;
using SAMLPortal.Models;

namespace SAMLPortal.Services
{
    public class LdapAuthenticationService : IAuthenticationService
    {
        private readonly LdapConnection _connection;
        private readonly GlobalSettings _config;

        public LdapAuthenticationService(IOptions<GlobalSettings> config)
        {
            _config = config.Value;
            _connection = new LdapConnection
            {
                SecureSocketLayer = false
            };
        }

        public AppUser Login(string username, string password)
        {
            _connection.Connect(_config.LdapHost, _config.LdapPort);
            _connection.Bind(_config.BindDn, _config.BindPass);

            var adminSearchFilter = string.Format(_config.AdministratorsFilter, username);
            var userSearchFilter = string.Format(_config.UsersFilter, username);

            List<string> filters = new List<string> { adminSearchFilter, userSearchFilter };

            foreach (var filter in filters)
            {
                LdapSearchQueue queue = _connection.Search(
                    _config.SearchBase,
                    LdapConnection.ScopeSub,
                    filter,
                    new string[] { _config.MemberOfAttr, _config.DisplayNameAttr, _config.UidAttr },
                    false,
                    (LdapSearchQueue)null,
                    (LdapSearchConstraints)null
                );

                LdapMessage message;

                try
                {
                    while ((message = queue.GetResponse()) != null)
                    {
                        if (message is LdapSearchResult)
                        {
                            var user = ((LdapSearchResult)message).Entry;
                            if (user != null)
                            {
                                _connection.Bind(user.Dn, password);
                                if (_connection.Bound)
                                {
                                    return new AppUser
                                    {
                                        DisplayName = user.GetAttribute(_config.DisplayNameAttr).StringValue,
                                        Username = user.GetAttribute(_config.UidAttr).StringValue,
                                        IsAdmin = filter == adminSearchFilter
                                    };
                                }
                            }
                        }
                    }

                    if (filter == userSearchFilter)
                    {
                        throw new Exception("Access denied");
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Access denied. ");
                }
            }

            _connection.Disconnect();
            return null;
        }
    }
}
