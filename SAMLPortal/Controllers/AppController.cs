using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAMLPortal.Misc;

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
		[Authorize(Roles = UserRoles.Administrator)]
		public ActionResult Create()
        {
            return View();
        }

        // POST: App/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = UserRoles.Administrator)]
		public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

		// GET: App/Edit/5
		[Authorize(Roles = UserRoles.Administrator)]
		public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: App/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = UserRoles.Administrator)]
		public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

		// GET: App/Delete/5
		[Authorize(Roles = UserRoles.Administrator)]
		public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: App/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = UserRoles.Administrator)]
		public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}