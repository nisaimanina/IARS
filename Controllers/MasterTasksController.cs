using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IARS.Data;
using IARS.Models;

namespace IARS.Controllers
{
    public class MasterTasksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MasterTasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. INDEX
        public async Task<IActionResult> Index()
        {
            return View(await _context.MasterTasks.ToListAsync());
        }

        // 2. CREATE (GET)
        public IActionResult Create()
        {
            return View();
        }

        // 3. CREATE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MasterTask masterTask)
        {
            if (ModelState.IsValid)
            {
                _context.Add(masterTask);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(masterTask);
        }

        // 4. EDIT (GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var masterTask = await _context.MasterTasks.FindAsync(id);
            if (masterTask == null) return NotFound();
            return View(masterTask);
        }

        // 5. EDIT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MasterTask masterTask)
        {
            if (id != masterTask.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(masterTask);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(masterTask);
        }

        // 6. DELETE (GET)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var masterTask = await _context.MasterTasks.FirstOrDefaultAsync(m => m.Id == id);
            if (masterTask == null) return NotFound();
            return View(masterTask);
        }

        // 7. DELETE (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var masterTask = await _context.MasterTasks.FindAsync(id);
            if (masterTask != null) _context.MasterTasks.Remove(masterTask);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    } // <--- PASTIKAN ADA PENUTUP CLASS DI SINI
} // <--- PASTIKAN ADA PENUTUP NAMESPACE DI SINI