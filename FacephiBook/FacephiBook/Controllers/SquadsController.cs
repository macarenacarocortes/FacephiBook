using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FacephiBook.Data;
using FacephiBook.Models;

namespace FacephiBook.Controllers
{
    public class SquadsController : Controller
    {
        private readonly FacephiBookContexto _context;

        public SquadsController(FacephiBookContexto context)
        {
            _context = context;
        }

        // GET: Squads
        public async Task<IActionResult> Index()
        {
              return _context.Squads != null ? 
                          View(await _context.Squads.ToListAsync()) :
                          Problem("Entity set 'FacephiBookContexto.Squads'  is null.");
        }

        // GET: Squads/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Squads == null)
            {
                return NotFound();
            }

            var squad = await _context.Squads
                .FirstOrDefaultAsync(m => m.Id == id);
            if (squad == null)
            {
                return NotFound();
            }

            return View(squad);
        }

        // GET: Squads/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Squads/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre")] Squad squad)
        {
            if (ModelState.IsValid)
            {
                _context.Add(squad);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(squad);
        }

        // GET: Squads/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Squads == null)
            {
                return NotFound();
            }

            var squad = await _context.Squads.FindAsync(id);
            if (squad == null)
            {
                return NotFound();
            }
            return View(squad);
        }

        // POST: Squads/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre")] Squad squad)
        {
            if (id != squad.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(squad);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SquadExists(squad.Id))
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
            return View(squad);
        }

        // GET: Squads/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Squads == null)
            {
                return NotFound();
            }

            var squad = await _context.Squads
                .FirstOrDefaultAsync(m => m.Id == id);
            if (squad == null)
            {
                return NotFound();
            }

            return View(squad);
        }

        // POST: Squads/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Squads == null)
            {
                return Problem("Entity set 'FacephiBookContexto.Squads'  is null.");
            }
            var squad = await _context.Squads.FindAsync(id);
            if (squad != null)
            {
                _context.Squads.Remove(squad);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SquadExists(int id)
        {
          return (_context.Squads?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
