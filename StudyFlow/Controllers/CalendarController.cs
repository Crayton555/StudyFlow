using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudyFlow.Data;
using StudyFlow.Models;

namespace StudyFlow.Controllers
{
    public class CalendarController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CalendarController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Calendar
        public async Task<IActionResult> Index()
        {
              return _context.Calendar != null ? 
                          View(await _context.Calendar.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Class'  is null.");
        }

        // GET: Calendar/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Calendar == null)
            {
                return NotFound();
            }

            var @class = await _context.Calendar
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@class == null)
            {
                return NotFound();
            }

            return View(@class);
        }

        // GET: Calendar/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Calendar/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Date,Description")] Calendar @class)
        {
            if (ModelState.IsValid)
            {
                @class.Id = Guid.NewGuid();
                _context.Add(@class);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@class);
        }

        // GET: Calendar/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Calendar == null)
            {
                return NotFound();
            }

            var @class = await _context.Calendar.FindAsync(id);
            if (@class == null)
            {
                return NotFound();
            }
            return View(@class);
        }

        // POST: Calendar/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,Date,Description")] Calendar @class)
        {
            if (id != @class.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@class);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClassExists(@class.Id))
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
            return View(@class);
        }

        // GET: Calendar/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Calendar == null)
            {
                return NotFound();
            }

            var @class = await _context.Calendar
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@class == null)
            {
                return NotFound();
            }

            return View(@class);
        }

        // POST: Calendar/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Calendar == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Class'  is null.");
            }
            var @class = await _context.Calendar.FindAsync(id);
            if (@class != null)
            {
                _context.Calendar.Remove(@class);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClassExists(Guid id)
        {
          return (_context.Calendar?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
