using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Novell.Directory.Ldap;
using SAMLPortal.Models;

namespace SAMLPortal.Controllers.API
{
	[Route("api/setup")]
	[ApiController]
	//[ServiceFilter(typeof(SetupAsyncActionFilter))]
	//[Produces("application/json")]
	public class SetupController : Controller
	{
		public SetupController()
		{
		}

		// GET api/setup
		[HttpPost]
		[Route("getResultsFromFilters")]
		public ActionResult<IEnumerable<string>> GetResultsFromFilters([FromBody]string testUsername)
		{
			if (GlobalSettings.GetInt("CONFIG_SETUPASSISTANT_STEP") >= 5)
			{
				try
				{
					LdapConnection connection = new LdapConnection();
					connection.SecureSocketLayer = bool.Parse(GlobalSettings.Get("LDAP_SSL"));

					connection.Connect(GlobalSettings.Get("LDAP_Host"), (int)GlobalSettings.GetInt("LDAP_Port"));
					connection.Bind(GlobalSettings.Get("LDAP_BindDN"), GlobalSettings.Get("LDAP_BindPass"));

					var userSearchFilter = string.Format(GlobalSettings.Get("LDAP_UsersFilter"), testUsername);
					var searchResults = connection.Search(GlobalSettings.Get("LDAP_SearchBase"), LdapConnection.ScopeSub, userSearchFilter, new string[] { }, false);

					List<string> results = new List<string>();

					while (searchResults.HasMore())
					{
						results.Add(searchResults.Next().Dn);
						return Ok(Json(results));
					}
				}
				catch (LdapException)
				{
					return Ok(Json(new { Error = "An error occurred." }));
				}
				catch (ArgumentNullException ex)
				{
					return Ok(Json(new { Error = "An error occured.", Message = ex.Message }));
				}
			}
			else
			{
				return Ok(Json(new { Error = "You need to complete setup step 5 or later first." }));
			}

			return Ok(Json(new { Error = "No results found." }));
		}
	}
}