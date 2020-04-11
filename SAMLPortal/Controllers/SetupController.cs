using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SAMLPortal.Misc;
using SAMLPortal.Models;

namespace SAMLPortal.Controllers
{
	[Authorize]
	[Route("Setup")]
	public class SetupController : Controller
	{
		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> Index()
		{
			return View();
		}

		[HttpGet]
		[Route("2")]
		public async Task<IActionResult> Step2()
		{
			return Content("Setup2");
		}
	}
}