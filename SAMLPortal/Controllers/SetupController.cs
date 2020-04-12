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
		public IActionResult FirstStep()
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
					mysqlPort = Convert.ToInt32(mysqlPort);
				}
				catch (Exception)
				{
				}
			}

			FirstStepModel model = new FirstStepModel();
			model.MySQLHost = mysqlHost != null ? mysqlHost : "";
			model.MySQLPort = mysqlPort != 3306 ? mysqlPort : 3306;
			model.MySQLDatabaseName = mysqlDb != null ? mysqlDb : "";
			model.MySQLUser = mysqlUser != null ? mysqlUser : "";

			return View(model);
		}

		[HttpPost]
		[Route("1")]
		public IActionResult FirstStep(FirstStepModel model)
		{
			if (ModelState.IsValid)
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
				catch (MySqlException ex)
				{
					Console.WriteLine(ex.Message);
					ModelState.AddModelError(string.Empty, ex.Message);
				}
				catch (Exception)
				{
					ModelState.AddModelError(string.Empty, "An unknown error occured. Please try again.");
				}
			}

			return View(model);
		}

		[HttpGet]
		[Route("2")]
		public IActionResult SecondStep()
		{
			var companyName = GlobalSettings.Get("CONFIG_CompanyName");
			var appHost = GlobalSettings.Get("CONFIG_URL");

			SecondStepModel model = new SecondStepModel();
			model.CompanyName = companyName != null ? companyName : "";
			model.AppHost = appHost != null ? appHost : Request.Host.ToString();

			var countries = new CountryHelper().GetCountryData();
			model.CountryList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(countries, "CountryShortCode", "CountryName");

			return View(model);
		}

		[HttpPost]
		[Route("2")]
		public IActionResult SecondStep(SecondStepModel model)
		{
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
		public IActionResult ThirdStep()
		{
			var ldapHost = GlobalSettings.Get("LDAP_Host");
			var ldapPort = GlobalSettings.GetInt("LDAP_Port");
			var ldapSSL = GlobalSettings.Get("LDAP_SSL") != null ? bool.Parse(GlobalSettings.Get("LDAP_SSL")) : false;
			var ldapBindDn = GlobalSettings.Get("LDAP_BindDN");
			var ldapBindPassword = GlobalSettings.Get("LDAP_BindPass");

			ThirdStepModel model = new ThirdStepModel();
			model.Host = ldapHost != null ? ldapHost : "";
			model.Port = ldapPort != null ? ((int)ldapPort) : 389;
			model.SSL = ldapSSL;
			model.BindDN = ldapBindDn != null ? ldapBindDn : "";
			model.BindPassword = ldapBindPassword != null ? ldapBindPassword : "";

			return View(model);
		}

		[HttpPost]
		[Route("3")]
		public IActionResult ThirdStep(ThirdStepModel model)
		{
			if (ModelState.IsValid)
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
				catch (LdapException ex)
				{
					ModelState.AddModelError(string.Empty, ex.Message);
				}
				catch (AggregateException ex)
				{
					ModelState.AddModelError(string.Empty, ex.Message);
				}
				catch (Exception ex)
				{
					ModelState.AddModelError(string.Empty, "An unknown error occurred. Please try again.");
					Console.WriteLine(ex.Message);
				}
			}

			return View(model);
		}

		[HttpGet]
		[Route("4")]
		public IActionResult StepFour()
		{
			StepFourModel model = new StepFourModel();
			return View(model);
		}

		[HttpPost]
		[Route("4")]
		public IActionResult StepFour(StepFourModel model)
		{
			if (ModelState.IsValid)
			{

			}

			return View(model);
		}
	}

	public class SetupAsyncActionFilter : IAsyncActionFilter
	{
		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			if (GlobalSettings.GetInt("CONFIG_SETUPASSISTANT_STEP") > 5)
			{
				// Setup has been completed and cannot be accessed anymore
				context.HttpContext.Response.Redirect("/");
			}

			var resultContext = await next();
		}
	}
}