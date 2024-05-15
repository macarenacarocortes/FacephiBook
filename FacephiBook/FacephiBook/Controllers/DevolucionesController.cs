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
    public class DevolucionesController : Controller
    {
        private readonly FacephiBookContexto _context;

        public DevolucionesController(FacephiBookContexto context)
        {
            _context = context;
        }

        // GET: Devoluciones
        public async Task<IActionResult> Index()
        {
            var facephiBookContexto = _context.Devoluciones.Include(d => d.Reserva);
            return View(await facephiBookContexto.ToListAsync());
        }

        // GET: Devoluciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Devoluciones == null)
            {
                return NotFound();
            }

            var devolucion = await _context.Devoluciones
                .Include(d => d.Reserva)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (devolucion == null)
            {
                return NotFound();
            }

            return View(devolucion);
        }

        // GET: Devoluciones/Create
        public IActionResult Create()
        {
            ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id");
            return View();
        }

        // POST: Devoluciones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FechaDevolucion,ReservaId,UsuarioId")] Devolucion devolucion)
        {
            
            devolucion.FechaDevolucion = DateTime.Now;

            // Obtener el correo electrónico del usuario actual
            var userEmail = User.Identity.Name;

            // Buscar al usuario en la tabla Usuarios basándose en el correo electrónico
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == userEmail);

             devolucion.UsuarioId = usuario.Id ;



            if (devolucion.ReservaId != null && devolucion.FechaDevolucion != null && devolucion.UsuarioId != null)
            {
                _context.Add(devolucion);
                await _context.SaveChangesAsync();
                return RedirectToAction("MisReservas", "Reservas");
            }
            ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id", devolucion.ReservaId);
            return RedirectToAction("MisReservas", "Reservas");
        }

        // GET: Devoluciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Devoluciones == null)
            {
                return NotFound();
            }

            var devolucion = await _context.Devoluciones.FindAsync(id);
            if (devolucion == null)
            {
                return NotFound();
            }
            ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id", devolucion.ReservaId);
            return View(devolucion);
        }

        // POST: Devoluciones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FechaDevolucion,ReservaId,UsuarioId")] Devolucion devolucion)
        {
            if (id != devolucion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(devolucion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DevolucionExists(devolucion.Id))
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
            ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id", devolucion.ReservaId);
            return View(devolucion);
        }

        // GET: Devoluciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Devoluciones == null)
            {
                return NotFound();
            }

            var devolucion = await _context.Devoluciones
                .Include(d => d.Reserva)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (devolucion == null)
            {
                return NotFound();
            }

            return View(devolucion);
        }

        // POST: Devoluciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Devoluciones == null)
            {
                return Problem("Entity set 'FacephiBookContexto.Devoluciones'  is null.");
            }
            var devolucion = await _context.Devoluciones.FindAsync(id);
            if (devolucion != null)
            {
                _context.Devoluciones.Remove(devolucion);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DevolucionExists(int id)
        {
          return (_context.Devoluciones?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
