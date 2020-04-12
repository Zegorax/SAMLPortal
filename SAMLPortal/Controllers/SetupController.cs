using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using SAMLPortal.Misc;
using SAMLPortal.Models;
using SAMLPortal.Models.Setup;
using MySql.Data.MySqlClient;
using Microsoft.EntityFrameworkCore;

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
			FirstStepModel model = new FirstStepModel();
			model.MySQLPort = 3306;
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
			return Content("WIP");
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