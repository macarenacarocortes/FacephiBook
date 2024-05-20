using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FacephiBook.Data;
using FacephiBook.Models.FacephiBook.Models;

namespace FacephiBook.Controllers
{
    public class NotificacionesController : Controller
    {
        private readonly FacephiBookContexto _context;

        public NotificacionesController(FacephiBookContexto context)
        {
            _context = context;
        }

        // GET: Notificaciones
        public async Task<IActionResult> Index()
        {
            var facephiBookContexto = _context.Notificacion.Include(n => n.Devolucion).Include(n => n.Producto).Include(n => n.Reserva).Include(n => n.Usuario);
            return View(await facephiBookContexto.ToListAsync());
        }

        // GET: Notificaciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Notificacion == null)
            {
                return NotFound();
            }

            var notificacion = await _context.Notificacion
                .Include(n => n.Devolucion)
                .Include(n => n.Producto)
                .Include(n => n.Reserva)
                .Include(n => n.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (notificacion == null)
            {
                return NotFound();
            }

            return View(notificacion);
        }

        // GET: Notificaciones/Create
        public IActionResult Create()
        {
            ViewData["DevolucionId"] = new SelectList(_context.Devoluciones, "Id", "Id");
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "CodigoReceptor");
            ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id");
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Apellido");
            return View();
        }

        // POST: Notificaciones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Descripcion,UsuarioId,UsuarioNombre,ReservaId,DevolucionId,ProductoId,Fecha")] Notificacion notificacion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(notificacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DevolucionId"] = new SelectList(_context.Devoluciones, "Id", "Id", notificacion.DevolucionId);
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "CodigoReceptor", notificacion.ProductoId);
            ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id", notificacion.ReservaId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Apellido", notificacion.UsuarioId);
            return View(notificacion);
        }

        // GET: Notificaciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Notificacion == null)
            {
                return NotFound();
            }

            var notificacion = await _context.Notificacion.FindAsync(id);
            if (notificacion == null)
            {
                return NotFound();
            }
            ViewData["DevolucionId"] = new SelectList(_context.Devoluciones, "Id", "Id", notificacion.DevolucionId);
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "CodigoReceptor", notificacion.ProductoId);
            ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id", notificacion.ReservaId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Apellido", notificacion.UsuarioId);
            return View(notificacion);
        }

        // POST: Notificaciones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Descripcion,UsuarioId,UsuarioNombre,ReservaId,DevolucionId,ProductoId,Fecha")] Notificacion notificacion)
        {
            if (id != notificacion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(notificacion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NotificacionExists(notificacion.Id))
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
            ViewData["DevolucionId"] = new SelectList(_context.Devoluciones, "Id", "Id", notificacion.DevolucionId);
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "CodigoReceptor", notificacion.ProductoId);
            ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id", notificacion.ReservaId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Apellido", notificacion.UsuarioId);
            return View(notificacion);
        }

        // GET: Notificaciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Notificacion == null)
            {
                return NotFound();
            }

            var notificacion = await _context.Notificacion
                .Include(n => n.Devolucion)
                .Include(n => n.Producto)
                .Include(n => n.Reserva)
                .Include(n => n.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (notificacion == null)
            {
                return NotFound();
            }

            return View(notificacion);
        }

        // POST: Notificaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Notificacion == null)
            {
                return Problem("Entity set 'FacephiBookContexto.Notificacion'  is null.");
            }
            var notificacion = await _context.Notificacion.FindAsync(id);
            if (notificacion != null)
            {
                _context.Notificacion.Remove(notificacion);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NotificacionExists(int id)
        {
          return (_context.Notificacion?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
