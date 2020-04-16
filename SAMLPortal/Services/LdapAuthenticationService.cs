using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;
using SAMLPortal.Models;

namespace SAMLPortal.Services
{
	public class LdapAuthenticationService : IAuthenticationService
	{
		private readonly LdapConnection _connection;

		public LdapAuthenticationService()
		{
			_connection = new LdapConnection
			{
				SecureSocketLayer = bool.Parse(GlobalSettings.Get("LDAP_SSL"))
			};
		}

		public AppUser Login(string username, string password)
		{
			_connection.Connect(GlobalSettings.Get("LDAP_Host"), (int)GlobalSettings.GetInt("LDAP_Port"));
			_connection.Bind(GlobalSettings.Get("LDAP_BindDN"), GlobalSettings.Get("LDAP_BindPass"));

			var adminSearchFilter = string.Format(GlobalSettings.Get("LDAP_AdminFilter"), username);
			var userSearchFilter = string.Format(GlobalSettings.Get("LDAP_UsersFilter"), username);

			List<string> filters = new List<string> { adminSearchFilter, userSearchFilter };

			foreach (var filter in filters)
			{
				var searchResults = _connection.Search(
					GlobalSettings.Get("LDAP_SearchBase"),
					LdapConnection.ScopeSub,
					filter,
					new string[] { GlobalSettings.Get("LDAP_Attr_MemberOf"), GlobalSettings.Get("LDAP_Attr_DisplayName"), GlobalSettings.Get("LDAP_Attr_UID"), GlobalSettings.Get("LDAP_Attr_Mail") },
					false
				);

				while (searchResults.HasMore())
				{
					try
					{
						var user = searchResults.Next();
						user.GetAttributeSet();
						if (user != null)
						{
							_connection.Bind(user.Dn, password);
							if (_connection.Bound)
							{
								var ldapDisplayName = user.GetAttribute(GlobalSettings.Get("LDAP_Attr_DisplayName")).StringValue;
								var ldapUsername = user.GetAttribute(GlobalSettings.Get("LDAP_Attr_UID")).StringValue;
								var ldapEmail = user.GetAttribute(GlobalSettings.Get("LDAP_Attr_Mail")).StringValue;
								var isAdmin = filter == adminSearchFilter;
								var ldapMemberships = user.GetAttribute(GlobalSettings.Get("LDAP_Attr_MemberOf")).StringValueArray;

								List<object> attributes = new List<object>() { ldapDisplayName, ldapUsername, ldapEmail, ldapMemberships };
								if (attributes.Any(a => a == null))
								{
									var nullAttributes = attributes.FindAll(a => a == null).ToArray();
									throw new Exception("The attribute " + string.Join(",", nullAttributes) + " is not present in your LDAP account. Please contact your administrator.");
								}

								return new AppUser
								{
									DisplayName = ldapDisplayName,
									Username = ldapUsername,
									Email = ldapEmail,
									IsAdmin = filter == adminSearchFilter,
									Memberships = ldapMemberships != null ? ldapMemberships : new string[] { }
								};
							}
						}

						if (filter == userSearchFilter)
						{
							throw new Exception("Invalid username or password");
						}
					}
					catch
					{
						throw new Exception("Invalid username or password");
					}
				}
			}

			_connection.Disconnect();
			return null;
		}
	}
}