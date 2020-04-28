using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SAMLPortal.Misc;
using SAMLPortal.Models;

namespace SAMLPortal.Controllers
{
	[Authorize]
	[Route("")]
	public class HomeController : Controller
	{
		[Authorize(Roles = UserRoles.User)]
		[Route("")]
		public IActionResult Index()
		{
			var userRoles = this.User.FindAll("membership").Select(r => r.Value).ToList();
			var allowedApps = new SAMLPortalContext().App.Where(app => userRoles.Contains(app.Role)).ToList();
			ViewBag.allowedApps = allowedApps;
			return View();
		}

		//[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		[Route("Error")]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}