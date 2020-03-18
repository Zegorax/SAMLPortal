using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAMLPortal.Models;

namespace SAMLPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SAMLAppsController : ControllerBase
    {
        private readonly SAMLPortalContext _context;

        public SAMLAppsController(SAMLPortalContext context)
        {
            _context = context;
        }

        // GET: api/SAMLApp
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SAMLAppDTO>>> GetSAMLApps()
        {
            return await _context.SAMLApps.Select(x => SAMLAppToDTO(x)).ToListAsync();
        }

        // GET: api/SAMLApp/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SAMLAppDTO>> GetSAMLApp(long id)
        {
            var sAMLApp = await _context.SAMLApps.FindAsync(id);

            if (sAMLApp == null)
            {
                return NotFound();
            }

            return SAMLAppToDTO(sAMLApp);
        }

        // PUT: api/SAMLApp/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSAMLApp(long id, SAMLAppDTO sAMLAppDTO)
        {
            if (id != sAMLAppDTO.Id)
            {
                return BadRequest();
            }

            var sAMLApp = await _context.SAMLApps.FindAsync(id);
            if(sAMLApp == null)
            {
                return NotFound();
            }

            sAMLApp.Name = sAMLAppDTO.Name;
            sAMLApp.IsEnabled = sAMLAppDTO.IsEnabled;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!SAMLAppExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/SAMLApp
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<SAMLApp>> PostSAMLApp(SAMLApp sAMLApp)
        {
            _context.SAMLApps.Add(sAMLApp);
            await _context.SaveChangesAsync();

            //return CreatedAtAction("GetSAMLApp", new { id = sAMLApp.Id }, sAMLApp);
            return CreatedAtAction(nameof(GetSAMLApp), new { id = sAMLApp.Id }, sAMLApp);
        }

        // DELETE: api/SAMLApp/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<SAMLApp>> DeleteSAMLApp(long id)
        {
            var sAMLApp = await _context.SAMLApps.FindAsync(id);
            if (sAMLApp == null)
            {
                return NotFound();
            }

            _context.SAMLApps.Remove(sAMLApp);
            await _context.SaveChangesAsync();

            return sAMLApp;
        }

        private bool SAMLAppExists(long id)
        {
            return _context.SAMLApps.Any(e => e.Id == id);
        }

        private static SAMLAppDTO SAMLAppToDTO(SAMLApp sAMLApp) =>
            new SAMLAppDTO
            {
                Id = sAMLApp.Id,
                Name = sAMLApp.Name,
                IsEnabled = sAMLApp.IsEnabled
            };
    }
}
