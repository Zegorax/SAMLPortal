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

namespace SAMLPortal.Controllers
{
	[Route("Setup")]
	[ServiceFilter(typeof(SetupAsyncActionFilter))]
	public class SetupController : Controller
	{
		[HttpGet]
		[Route("0")]
		[Route("")]
		public async Task<IActionResult> Index()
		{
			return View();
		}

		[HttpGet]
		[Route("1")]
		public async Task<IActionResult> Step2()
		{
			return Content("Setup2");
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