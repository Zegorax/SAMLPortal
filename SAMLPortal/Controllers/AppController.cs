using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAMLPortal.Misc;
using SAMLPortal.Models;
using Microsoft.EntityFrameworkCore;
using ITfoxtec.Identity.Saml2.Schemas.Metadata;
using SAMLPortal.Misc;

namespace SAMLPortal.Controllers
{
	[Authorize]
	[Route("App")]
	public class AppController : Controller
	{
		// GET: App
		[AllowAnonymous] //just pour tester
						 //[Authorize(Roles = UserRoles.Administrator)]
		public ActionResult Index()
		{
			SAMLPortalContext context = new SAMLPortalContext();
			return View(context.App.ToList());
		}

		// GET: App/Details/5
		[AllowAnonymous] //just pour tester
						 //[Authorize(Roles = UserRoles.Administrator)]
		[Route("Details/{id}")]
		public ActionResult Details(int id)
		{
			SAMLPortalContext context = new SAMLPortalContext();
			App app = context.App.Find(id);
			return View(app);
		}

		// GET: App/Create
		[HttpGet]
		[AllowAnonymous] //just pour tester
						 //[Authorize(Roles = UserRoles.Administrator)]
		[Route("Create")]
		public ActionResult Create()
		{
			App emptyApp = new App();
			return View(emptyApp);
		}

		// Post: App/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		[AllowAnonymous] //just pour tester
						 //[Authorize(Roles = UserRoles.Administrator)]
		[Route("Create")]
		public async Task<IActionResult> Create(App appli)
		{
			try
			{
				SAMLPortalContext context = new SAMLPortalContext();
				if (ModelState.IsValid)
				{
					if (appli.MetadataURL != null)
					{

					}
					context.Add(appli);
					await context.SaveChangesAsync();
					return Redirect("/");
				}
			}
			catch (DbUpdateException /* ex */)
			{
				//Log the error (uncomment ex variable name and write a log.
				ModelState.AddModelError("", "Unable to save changes. " +
					"Try again, and if the problem persists " +
					"see your system administrator.");
			}
			return View(appli);
		}

		// GET: App/Edit/5
		[AllowAnonymous] //just pour tester
						 //[Authorize(Roles = UserRoles.Administrator)]
		[Route("Edit/{id}")]
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			SAMLPortalContext context = new SAMLPortalContext();
			var app = await context.App.FindAsync(id);
			if (app == null)
			{
				return NotFound();
			}
			return View(app);
		}

		// POST: App/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		[AllowAnonymous] //just pour tester
						 //[Authorize(Roles = UserRoles.Administrator)]
		[Route("Edit/{id}")]
		public async Task<IActionResult> EditPost(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}
			SAMLPortalContext context = new SAMLPortalContext();
			var appToUpdate = await context.App.FirstOrDefaultAsync(s => s.Id == id);
			if (await TryUpdateModelAsync<App>(
				appToUpdate,
				"",
				s => s.Name,
				s => s.Description,
				s => s.Enabled,
				s => s.MetadataURL,
				s => s.Issuer,
				s => s.SingleSignOnDestination,
				s => s.SingleLogoutResponseDestination))
			{
				try
				{
					await context.SaveChangesAsync();
					return Redirect("/");
				}
				catch (DbUpdateException /* ex */)
				{
					//Log the error (uncomment ex variable name and write a log.)
					ModelState.AddModelError("", "Unable to save changes. " +
						"Try again, and if the problem persists, " +
						"see your system administrator.");
				}
			}
			return View(appToUpdate);
		}

		// GET: App/Delete/5
		[AllowAnonymous] //just pour tester
						 //[Authorize(Roles = UserRoles.Administrator)]
		[Route("Delete/{id}")]
		public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
		{
			if (id == null)
			{
				return NotFound();
			}
			SAMLPortalContext context = new SAMLPortalContext();
			var app = await context.App.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
			if (app == null)
			{
				return NotFound();
			}

			if (saveChangesError.GetValueOrDefault())
			{
				ViewData["ErrorMessage"] =
					"Delete failed. Try again, and if the problem persists " +
					"see your system administrator.";
			}
			return View(app);
		}

		// POST: App/Delete/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		[AllowAnonymous] //just pour tester
						 //[Authorize(Roles = UserRoles.Administrator)]
		[Route("Delete/{id}")]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			SAMLPortalContext context = new SAMLPortalContext();
			var app = await context.App.FindAsync(id);
			if (app == null)
			{
				return Redirect("/");
			}

			try
			{
				context.App.Remove(app);
				await context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			catch (DbUpdateException /* ex */)
			{
				//Log the error (uncomment ex variable name and write a log.)
				return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
			}
		}

		// Post: App/VerifyMetadata
		[HttpPost]
		[ValidateAntiForgeryToken]
		[AllowAnonymous] //just pour tester
						 //[Authorize(Roles = UserRoles.Administrator)]
		[Route("VerifyMetadata")]
		public async Task<IActionResult> VerifyMetadata(AppWithMandatoryFields app)
		{
			App appli = new App();
			if (ModelState.IsValid)
			{
				var entityDescriptor = new EntityDescriptor();
				appli.Name = app.Name;
				appli.Description = app.Description;
				entityDescriptor.ReadSPSsoDescriptorFromUrl(new Uri(app.MetadataURL));
				if (entityDescriptor.SPSsoDescriptor != null)
				{
					appli.Issuer = entityDescriptor.EntityId;
					appli.SingleSignOnDestination = entityDescriptor.SPSsoDescriptor.AssertionConsumerServices.First().Location;
					if (entityDescriptor.SPSsoDescriptor.SingleLogoutServices.Count() > 0)
					{
						var singleLogoutService = entityDescriptor.SPSsoDescriptor.SingleLogoutServices.First();
						appli.SingleLogoutResponseDestination = singleLogoutService.ResponseLocation ?? singleLogoutService.Location;
					}
					else
					{
						appli.SingleLogoutResponseDestination = new Uri("about:blank");
					}
				}
				else
				{
					appli.MetadataURL = ""; //pas de metadataURL donc on a tout fait à la main
				}
			}
			return View("Create", appli);
		}
	}
}