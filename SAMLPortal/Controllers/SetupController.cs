using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SAMLPortal.Models;

namespace SAMLPortal.Controllers
{
    public class SetupController : Controller
    {
        private readonly SAMLPortalContext _context;

        public SetupController(SAMLPortalContext context)
        {
            _context = context;
        }

        // GET: Setup
        public async Task<IActionResult> Index()
        {
            return View(await _context.Setup.ToListAsync());
        }

        // GET: Setup/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var setup = await _context.Setup
                .FirstOrDefaultAsync(m => m.Id == id);
            if (setup == null)
            {
                return NotFound();
            }

            return View(setup);
        }

        // GET: Setup/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Setup/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CompanyName,IsConfigured,IsInMaintenance,LdapHost,BindDn,BindPass,SearchBase,UsersFilter,AdministratorsFilter,UidAttr,MemberOfAttr,DisplayNameAttr,TemporaryPassword")] Setup setup)
        {
            if (ModelState.IsValid)
            {
                _context.Add(setup);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(setup);
        }

        // GET: Setup/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var setup = await _context.Setup.FindAsync(id);
            if (setup == null)
            {
                return NotFound();
            }
            return View(setup);
        }

        // POST: Setup/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CompanyName,IsConfigured,IsInMaintenance,LdapHost,BindDn,BindPass,SearchBase,UsersFilter,AdministratorsFilter,UidAttr,MemberOfAttr,DisplayNameAttr,TemporaryPassword")] Setup setup)
        {
            if (id != setup.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(setup);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SetupExists(setup.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(setup);
        }

        // GET: Setup/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var setup = await _context.Setup
                .FirstOrDefaultAsync(m => m.Id == id);
            if (setup == null)
            {
                return NotFound();
            }

            return View(setup);
        }

        // POST: Setup/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var setup = await _context.Setup.FindAsync(id);
            _context.Setup.Remove(setup);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SetupExists(int id)
        {
            return _context.Setup.Any(e => e.Id == id);
        }
    }
}
