using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using StudyFlow.Data;
using StudyFlow.Models.Domain;

namespace StudyFlow.Controllers
{
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tasks
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return RedirectToAction("Index", "Home");

            var loggedInUser = await _context.Users
                .Where(z => z.Id == userId)
                .Include(z => z.Tasks)
                .FirstOrDefaultAsync();

            var tasks = loggedInUser.Tasks.ToList();

            return tasks != null ?
                         View(tasks) :
                         Problem("Entity set 'ApplicationDbContext.Tasks'  is null.");


            /*return _context.Tasks != null ?
                        View(await _context.Tasks.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Tasks'  is null.");*/
        }
        
        public async Task<IActionResult> Dashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return RedirectToAction("Index", "Home");



            var loggedInUser = await _context.Users
                .Where(z => z.Id == userId)
                .Include(z => z.Tasks)
                .FirstOrDefaultAsync();

            var tasks = loggedInUser.Tasks.ToList();

            return tasks != null ?
                         View(tasks) :
                         Problem("Entity set 'ApplicationDbContext.Tasks'  is null.");
        }

        public async Task<IActionResult> Calendar()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return RedirectToAction("Index", "Home");

            var loggedInUser = await _context.Users
                .Where(z => z.Id == userId)
                .Include(z => z.Tasks)
                .FirstOrDefaultAsync();

            var tasks = loggedInUser?.Tasks?.ToList();

            return tasks != null ?
                         View(tasks) :
                         Problem("Entity set 'ApplicationDbContext.Tasks'  is null.");
        }

        // GET: Tasks/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // GET: Tasks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Priority,Status,CreatedAt,DueDate")] Models.Domain.Task task)
        {
            if (ModelState.IsValid)
            {
                task.Id = Guid.NewGuid();

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var loggedInUser = await _context.Users
                    .Where(z => z.Id == userId)
                    .Include(z => z.Tasks)
                    .FirstOrDefaultAsync();

                loggedInUser.Tasks.Add(task);


                _context.Update(loggedInUser);

                task.User = loggedInUser;

                _context.Add(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(task);
        }

        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            return View(task);
        }

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,Description,Priority,Status,CreatedAt,DueDate")] Models.Domain.Task task)
        {
            if (id != task.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(task);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskExists(task.Id))
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
            return View(task);
        }

        // GET: Tasks/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Tasks == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Tasks'  is null.");
            }
            var task = await _context.Tasks.FindAsync(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaskExists(Guid id)
        {
            return (_context.Tasks?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
