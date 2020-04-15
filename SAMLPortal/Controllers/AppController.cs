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

namespace SAMLPortal.Controllers
{
	[Authorize]
	public class AppController : Controller
    {
        // GET: App
        public ActionResult Index()
        {
            return View();
        }

        // GET: App/Details/5
        public ActionResult Details(int id)
        {
            return View();
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
		public async Task<IActionResult> Create(
			[Bind("Name, Description, Enabled, MetadataURL, Issuer, SingleSignOnDestination, SingleLogoutResponseDestination, SignatureValidationCertificate")] App appli)
        {
			try
			{
				SAMLPortalContext context = new SAMLPortalContext();
				if(ModelState.IsValid)
				{
					
					context.Add(appli);
					await context.SaveChangesAsync();
					return RedirectToAction(nameof(Index));
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
		[Route("Edit")]
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
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
		[AllowAnonymous] //just pour tester
		//[Authorize(Roles = UserRoles.Administrator)]
		[Route("Edit")]
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
				s => s.SingleLogoutResponseDestination,
				s => s.SignatureValidationCertificate))
			{
				try
				{
					await context.SaveChangesAsync();
					return RedirectToAction(nameof(Index));
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
		[Route("Delete")]
		public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
			if(id == null)
			{
				return NotFound();
			}
			SAMLPortalContext context = new SAMLPortalContext();
			var app = await context.App.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
			if(app == null)
			{
				return NotFound();
			}

			if(saveChangesError.GetValueOrDefault())
			{
				ViewData["ErrorMessage"] =
					"Delete failed. Try again, and if the problem persists " +
					"see your system administrator.";
			}
            return View(app);
        }

        // POST: App/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
		[AllowAnonymous] //just pour tester
		//[Authorize(Roles = UserRoles.Administrator)]
		[Route("Delete")]
		public async Task<IActionResult> DeleteConfirmed(int id)
        {
			SAMLPortalContext context = new SAMLPortalContext();
            var app = await context.App.FindAsync(id);
            if (app == null)
            {
                return RedirectToAction(nameof(Index));
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
    }
}