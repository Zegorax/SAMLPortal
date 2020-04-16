using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SAMLPortal.Misc;
using SAMLPortal.Models;
using SAMLPortal.Models.Setup;
using MySql.Data.MySqlClient;
using Microsoft.EntityFrameworkCore;
using CountryData.Standard;
using Novell.Directory.Ldap;

namespace SAMLPortal.Controllers
{
	[Route("Setup")]
	[ServiceFilter(typeof(SetupAsyncActionFilter))]
	public class SetupController : Controller
	{
		[HttpGet]
		[Route("0")]
		[Route("")]
		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		[Route("1")]
		public IActionResult StepOne()
		{
			var mysqlHost = Environment.GetEnvironmentVariable("SP_MYSQL_HOST");
			var mysqlPortString = Environment.GetEnvironmentVariable("SP_MYSQL_PORT");
			var mysqlDb = Environment.GetEnvironmentVariable("SP_MYSQL_DB");
			var mysqlUser = Environment.GetEnvironmentVariable("SP_MYSQL_USER");

			var mysqlPort = 3306;
			if (mysqlPortString != null)
			{
				try
				{
					mysqlPort = Convert.ToInt32(mysqlPortString);
				}
				catch (Exception ex)
				{
					// Not important if exception fires
				}
			}

			StepOneModel model = new StepOneModel();
			model.MySQLHost = mysqlHost != null ? mysqlHost : "";
			model.MySQLPort = mysqlPort != 3306 ? mysqlPort : 3306;
			model.MySQLDatabaseName = mysqlDb != null ? mysqlDb : "";
			model.MySQLUser = mysqlUser != null ? mysqlUser : "";

			return View(model);
		}

		[HttpPost]
		[Route("1")]
		public IActionResult StepOne(StepOneModel model)
		{
			if (ModelState.IsValid && model != null)
			{
				try
				{
					string connectionString = "server=" + model.MySQLHost
											+ ";port=" + model.MySQLPort.ToString()
											+ ";database=" + model.MySQLDatabaseName
											+ ";user=" + model.MySQLUser
											+ ";password=" + model.MySQLPassword;

					MySqlConnection connection = new MySqlConnection(connectionString);
					connection.Open();

					// If everything goes well
					var fileName = GlobalSettings.Get("CONFIG_FILE");
					Helpers.ReplaceEnvVariableInFile(fileName, "SP_MYSQL_HOST", model.MySQLHost);
					Helpers.ReplaceEnvVariableInFile(fileName, "SP_MYSQL_PORT", model.MySQLPort.ToString());
					Helpers.ReplaceEnvVariableInFile(fileName, "SP_MYSQL_DB", model.MySQLDatabaseName);
					Helpers.ReplaceEnvVariableInFile(fileName, "SP_MYSQL_USER", model.MySQLUser);
					Helpers.ReplaceEnvVariableInFile(fileName, "SP_MYSQL_PASS", model.MySQLPassword);

					GlobalSettings.InitSettingsFromEnvironment();

					SAMLPortalContext context = new SAMLPortalContext();
					context.Database.Migrate();

					// MySQL Step is complete
					Helpers.ReplaceEnvVariableInFile(fileName, "SP_CONFIG_SETUPASSISTANT_STEP", "2");
					GlobalSettings.Store("CONFIG_SETUPASSISTANT_STEP", "2");
					return Redirect("2");
				}
				catch (Exception ex) when (ex is MySqlException)
				{
					if (ex is MySqlException)
					{
						ModelState.AddModelError(string.Empty, ex.Message);
					}
					else
					{
						ModelState.AddModelError(string.Empty, "An unknown error occurred. Please try again.");
						throw;
					}
				}
			}

			return View(model);
		}

		[HttpGet]
		[Route("2")]
		public IActionResult StepTwo()
		{
			if (!CheckStep(2))
			{
				return Redirect("1");
			}

			var companyName = GlobalSettings.Get("CONFIG_CompanyName");
			var appHost = GlobalSettings.Get("CONFIG_URL");

			StepTwoModel model = new StepTwoModel();
			model.CompanyName = companyName != null ? companyName : "";
			model.AppHost = appHost != null ? appHost : Request.Host.ToString();

			var countries = new CountryHelper().GetCountryData();
			model.CountryList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(countries, "CountryShortCode", "CountryName");

			return View(model);
		}

		[HttpPost]
		[Route("2")]
		public IActionResult StepTwo(StepTwoModel model)
		{
			if (!CheckStep(2))
			{
				return Redirect("1");
			}

			var countries = new CountryHelper().GetCountryData();

			if (ModelState.IsValid)
			{
				var country = countries.Where(c => c.CountryShortCode == model.CountryCode);
				if (country.Any())
				{
					GlobalSettings.Store("CONFIG_CompanyName", model.CompanyName);
					GlobalSettings.Store("CONFIG_CompanySubject", model.CompanyName);
					GlobalSettings.Store("CONFIG_CompanyCountryCode", country.First().CountryShortCode);
					GlobalSettings.Store("CONFIG_URL", model.AppHost);

					Helpers.ReplaceEnvVariableInFile(GlobalSettings.Get("CONFIG_FILE"), "SP_CONFIG_SETUPASSISTANT_STEP", "3");
					GlobalSettings.Store("CONFIG_SETUPASSISTANT_STEP", "3");

					return Redirect("3");
				}
				else
				{
					ModelState.AddModelError(string.Empty, "Unknown country");
				}
			}

			model.CountryList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(countries, "CountryShortCode", "CountryName");

			return View(model);

		}

		[HttpGet]
		[Route("3")]
		public IActionResult StepThree()
		{
			if (!CheckStep(3))
			{
				return Redirect("2");
			}

			var ldapHost = GlobalSettings.Get("LDAP_Host");
			var ldapPort = GlobalSettings.GetInt("LDAP_Port");
			var ldapSSL = false;
			var ldapBindDn = GlobalSettings.Get("LDAP_BindDN");
			var ldapBindPassword = GlobalSettings.Get("LDAP_BindPass");

			if (GlobalSettings.Get("LDAP_SSL") != null)
			{
				ldapSSL = bool.Parse(GlobalSettings.Get("LDAP_SSL"));
			}

			StepThreeModel model = new StepThreeModel();
			model.Host = ldapHost != null ? ldapHost : "";
			model.Port = ldapPort != null ? ((int)ldapPort) : 389;
			model.SSL = ldapSSL;
			model.BindDN = ldapBindDn != null ? ldapBindDn : "";
			model.BindPassword = ldapBindPassword != null ? ldapBindPassword : "";

			return View(model);
		}

		[HttpPost]
		[Route("3")]
		public IActionResult StepThree(StepThreeModel model)
		{
			if (!CheckStep(3))
			{
				return Redirect("2");
			}

			if (ModelState.IsValid && model != null)
			{
				LdapConnection connection = new LdapConnection();
				connection.SecureSocketLayer = model.SSL;

				try
				{
					connection.Connect(model.Host, model.Port);
					connection.Bind(model.BindDN, model.BindPassword);

					//If everything goes well
					GlobalSettings.Store("LDAP_Host", model.Host);
					GlobalSettings.Store("LDAP_Port", model.Port.ToString());
					GlobalSettings.Store("LDAP_SSL", model.SSL.ToString());
					GlobalSettings.Store("LDAP_BindDN", model.BindDN);
					GlobalSettings.Store("LDAP_BindPass", model.BindPassword);

					Helpers.ReplaceEnvVariableInFile(GlobalSettings.Get("CONFIG_FILE"), "SP_CONFIG_SETUPASSISTANT_STEP", "4");
					GlobalSettings.Store("CONFIG_SETUPASSISTANT_STEP", "4");
					return Redirect("4");

				}
				catch (Exception ex)
				{
					if (ex is LdapException || ex is AggregateException)
					{
						ModelState.AddModelError(string.Empty, ex.Message);
					}
					else
					{
						ModelState.AddModelError(string.Empty, "An unknown error occurred. Please try again.");
						throw;
					}
				}
			}

			return View(model);
		}

		[HttpGet]
		[Route("4")]
		public IActionResult StepFour()
		{
			if (!CheckStep(4))
			{
				return Redirect("3");
			}

			var ldapSearchBase = GlobalSettings.Get("LDAP_SearchBase");
			var ldapAdminFilter = GlobalSettings.Get("LDAP_AdminFilter");
			var ldapUserFilter = GlobalSettings.Get("LDAP_UsersFilter");

			StepFourModel model = new StepFourModel();
			model.SearchBase = ldapSearchBase != null ? ldapSearchBase : "";
			model.AdminFilter = ldapAdminFilter != null ? ldapAdminFilter : "uid={0}";
			model.UserFilter = ldapUserFilter != null ? ldapUserFilter : "uid={0}";

			return View(model);
		}

		[HttpPost]
		[Route("4")]
		public IActionResult StepFour(StepFourModel model)
		{
			if (!CheckStep(4))
			{
				return Redirect("3");
			}

			if (ModelState.IsValid && model != null)
			{
				GlobalSettings.Store("LDAP_SearchBase", model.SearchBase);
				GlobalSettings.Store("LDAP_AdminFilter", model.AdminFilter);
				GlobalSettings.Store("LDAP_UsersFilter", model.UserFilter);

				Helpers.ReplaceEnvVariableInFile(GlobalSettings.Get("CONFIG_FILE"), "SP_CONFIG_SETUPASSISTANT_STEP", "5");
				GlobalSettings.Store("CONFIG_SETUPASSISTANT_STEP", "5");
			}

			return View(model);
		}

		[HttpGet]
		[Route("5")]
		public IActionResult StepFive()
		{
			if (!CheckStep(5))
			{
				return Redirect("4");
			}

			var ldapDisplayName = GlobalSettings.Get("LDAP_Attr_DisplayName");
			var ldapUID = GlobalSettings.Get("LDAP_Attr_UID");
			var ldapMemberOf = GlobalSettings.Get("LDAP_Attr_MemberOf");
			var ldapMail = GlobalSettings.Get("LDAP_Attr_Mail");

			StepFiveModel model = new StepFiveModel();
			model.DisplayName = ldapDisplayName != null ? ldapDisplayName : "cn";
			model.UID = ldapUID != null ? ldapUID : "uid";
			model.MemberOf = ldapMemberOf != null ? ldapMemberOf : "memberOf";
			model.Mail = ldapMail != null ? ldapMail : "mail";

			return View(model);
		}

		[HttpPost]
		[Route("5")]
		public IActionResult StepFive(StepFiveModel model)
		{
			if (!CheckStep(5))
			{
				return Redirect("4");
			}

			if (ModelState.IsValid && model != null)
			{
				GlobalSettings.Store("LDAP_Attr_DisplayName", model.DisplayName);
				GlobalSettings.Store("LDAP_Attr_UID", model.UID);
				GlobalSettings.Store("LDAP_Attr_MemberOf", model.MemberOf);
				GlobalSettings.Store("LDAP_Attr_Mail", model.Mail);

				Helpers.ReplaceEnvVariableInFile(GlobalSettings.Get("CONFIG_FILE"), "SP_CONFIG_SETUPASSISTANT_STEP", "6");
				GlobalSettings.Store("CONFIG_SETUPASSISTANT_STEP", "6");

				return Redirect("/");
			}

			return View(model);
		}

		/// <summary>
		/// Returns false if the user purposely miss a step
		/// </summary>
		/// <param name="step"></param>
		/// <returns></returns>
		private static bool CheckStep(int step)
		{
			if (step - GlobalSettings.GetInt("CONFIG_SETUPASSISTANT_STEP") <= 1)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}

	public class SetupAsyncActionFilter : IAsyncActionFilter
	{
		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			if (GlobalSettings.GetInt("CONFIG_SETUPASSISTANT_STEP") > 5)
			{
				if (context != null)
				{
					// Setup has been completed and cannot be accessed anymore
					context.HttpContext.Response.Redirect("/");
				}
			}
			else
			{
				await next();
			}
		}
	}
}