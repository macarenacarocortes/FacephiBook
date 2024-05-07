using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FacephiBook.Data;
using FacephiBook.Models;
using System.Globalization;

namespace FacephiBook.Controllers
{
    //prueba
    public class ReservasController : Controller
    {
        private readonly FacephiBookContexto _context;

        public ReservasController(FacephiBookContexto context)
        {
            _context = context;
        }

        // GET: Reservas
        public async Task<IActionResult> Index()
        {
            var facephiBookContexto = _context.Reservas.Include(r => r.Producto).Include(r => r.Usuario);
            return View(await facephiBookContexto.ToListAsync());
        }

        // GET: Reservas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Reservas == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
                .Include(r => r.Producto)
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        // GET: Reservas/Create/"INDEX"
        public IActionResult Create(int productoId)
        {
            // Obtener el correo electrónico del usuario actual
            var userEmail = User.Identity.Name;

            // Buscar al usuario en la tabla Usuarios basándose en el correo electrónico
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == userEmail);
            var producto = _context.Productos.FirstOrDefault(p => p.Id == productoId);

            if (usuario != null && producto != null)
            {
                // Obtener las fechas de reserva del dispositivo seleccionado
                var fechasReserva = _context.Reservas
                    .Where(r => r.ProductoId == productoId)
                    .Select(r => r.FechaInicio)
                    .ToList();

                // Crear una nueva instancia de Reserva para mostrar los datos
                var reserva = new Reserva
                {
                    UsuarioId = usuario.Id,
                    ProductoId = productoId,
                    Producto = producto
                };

                // Pasar las fechas de reserva a la vista
                ViewData["FechasReserva"] = fechasReserva.Select(d => d.ToString("dd/MM/yyyy")).ToList();

                ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "CodigoReceptor", productoId);
                return View(reserva);
            }
            else
            {
                // Usuario o producto no encontrado, redirigir a la vista de registro de usuarios
                return RedirectToAction("CreatePublic", "Usuarios");
            }
        }




        // POST: Reservas/Create/ CREACION DE LA RESERVA
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FechaInicio,FechaFinal,UsuarioId,ProductoId")] Reserva reserva)
        {
            // Obtener el correo electrónico del usuario actual
            var userEmail = User.Identity.Name;

            // Buscar al usuario en la tabla Usuarios basándose en el correo electrónico
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == userEmail);

            if (usuario != null)
            {
                // Crear una nueva instancia de Reserva para mostrar los datos
                reserva.UsuarioId = usuario.Id;

                // Verificar si las fechas seleccionadas están disponibles
                var fechasReservadas = _context.Reservas
                    .Where(r => r.ProductoId == reserva.ProductoId)
                    .Where(r => (reserva.FechaInicio <= r.FechaFinal && reserva.FechaFinal >= r.FechaInicio))
                    .Select(r => new { r.FechaInicio, r.FechaFinal })
                    .ToList();

                if (fechasReservadas.Any())
                {
                    // Se encontraron conflictos de reserva, mostrar mensaje de error
                    ModelState.AddModelError("", "Las fechas seleccionadas están ocupadas. Por favor, elige otras fechas.");
                }
                else
                {
                    // Las fechas están disponibles, guardar la reserva
                    reserva.FechaInicio = reserva.FechaInicio.Date;
                    reserva.FechaFinal = reserva.FechaFinal.Date;

                    _context.Add(reserva);
                    await _context.SaveChangesAsync();

                    // Redirigir a la página de mis reservas
                    return RedirectToAction("MisReservas");
                }
            }
            else
            {
                // Usuario no encontrado, redirigir a la vista de registro de usuarios
                return RedirectToAction("CreatePublic", "Usuarios");
            }

            // Si llegamos a este punto, hay un error, volver a mostrar el formulario con los errores
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "CodigoReceptor", reserva.ProductoId);
            return View(reserva);
        }





        // GET: Reservas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Reservas == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "CodigoReceptor", reserva.ProductoId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Apellido", reserva.UsuarioId);
            return View(reserva);
        }

        // POST: Reservas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Hora,FechaInicio,FechaFinal,UsuarioId,ProductoId,DevolucionId")] Reserva reserva)
        {
            if (id != reserva.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reserva);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservaExists(reserva.Id))
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
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "CodigoReceptor", reserva.ProductoId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Apellido", reserva.UsuarioId);
            return View(reserva);
        }

        // GET: Reservas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Reservas == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
                .Include(r => r.Producto)
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        // POST: Reservas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Reservas == null)
            {
                return Problem("Entity set 'FacephiBookContexto.Reservas'  is null.");
            }
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva != null)
            {
                _context.Reservas.Remove(reserva);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservaExists(int id)
        {
          return (_context.Reservas?.Any(e => e.Id == id)).GetValueOrDefault();
        }


        public async Task<IActionResult> MisReservas()
        {
            /*var facephiBookContexto = _context.Reservas.Include(r => r.Producto).Include(r => r.Usuario);
            return View(await facephiBookContexto.ToListAsync());*/
           
                // Obtener el Id del usuario actual a través del correo electrónico
                var userEmail = User.Identity.Name; // Suponiendo que User.Identity.Name contiene el correo electrónico del usuario

                // Buscar el Id del usuario en la tabla Usuario basándose en el correo electrónico
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == userEmail);

                if (usuario != null)
                {
                    var usuarioId = usuario.Id;

                    // Filtrar las reservas por el Id del usuario encontrado
                    var reservasUsuarioActual = _context.Reservas
                        .Include(r => r.Producto)
                        .Include(r => r.Usuario)
                        .Include(r => r.Devolucion)
                        .Where(r => r.UsuarioId == usuarioId);

                    return View(await reservasUsuarioActual.ToListAsync());
                }
                else
                {
                    // Manejar el caso en que no se encuentre el usuario
                    return NotFound();
                }          
        }
    }
}
