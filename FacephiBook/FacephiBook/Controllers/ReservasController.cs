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

        public IActionResult GetReservas(int productoId)
        {
            // Obtener la lista de reservas para el productoId especificado
            var reservas = _context.Reservas.Where(r => r.ProductoId == productoId).ToList();

            // Crear una lista para almacenar todos los rangos de días
            var rangosDias = new List<string>();

            // Iterar sobre cada reserva para generar los rangos de días
            foreach (var reserva in reservas)
            {
                var fechaInicio = reserva.FechaInicio;
                var fechaFinal = reserva.FechaFinal;

                // Calcular la cantidad de días entre la fecha de inicio y la fecha final
                var diasReserva = (fechaFinal - fechaInicio).Days + 1;

                // Generar el rango de días y agregarlo a la lista
                for (int i = 0; i < diasReserva; i++)
                {
                    var fecha = fechaInicio.AddDays(i);
                    rangosDias.Add(fecha.ToString("dd/MM/yyyy"));
                }
            }

            // Convertir la lista de rangos de días a un arreglo para el JSON
            var rangosArray = rangosDias.ToArray();

            return Json(rangosArray);
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

        // GET: Reservas/Create/    VISTA:"INDEX"
        public IActionResult Create(int productoId)
        {
            // Obtener el correo electrónico del usuario actual
            var userEmail = User.Identity.Name;

            // Buscar al usuario en la tabla Usuarios basándose en el correo electrónico
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == userEmail);
            var producto = _context.Productos.FirstOrDefault(p => p.Id == productoId); 

            if (usuario != null && producto != null)
            {
                // Obtener todas las reservas asociadas al producto con el Id dado
                var reservas = _context.Reservas.Where(r => r.ProductoId == productoId).ToList();

                // Lista para Obtener el rango de fechas entre FechaInicio y FechaFinal
                var fechasBloqueadas = new List<string>();

                foreach (var res in reservas) //seleccionamos cada reserva asociada al producto
                {
                    var diasReserva = (res.FechaFinal - res.FechaInicio).Days + 1;
                    for (int i = 0; i < diasReserva; i++)
                    {
                        var fecha = res.FechaInicio.AddDays(i);
                        fechasBloqueadas.Add(fecha.ToString("dd/MM/yyyy")); // le pasaremos las fechas a la lista
                    }

                }

                // Crear una nueva instancia de Reserva para mostrar los datos
                var reserva = new Reserva
                {
                    UsuarioId = usuario.Id,
                    ProductoId = productoId,
                    FechaInicio = DateTime.Now,
                    FechaFinal = DateTime.Now,
                    FechasBloqueadas = fechasBloqueadas // Asignar las fechas bloqueadas a la reserva
                };

                ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "CodigoReceptor", "Marca");
                ViewData["FechasBloqueadas"] = fechasBloqueadas; // Pasar las fechas bloqueadas a la vista
                return View(reserva);
            }
            else
            {
                // Usuario no encontrado o producto no encontrado, redirigir a la vista adecuada
                return RedirectToAction("CreatePublic", "Usuarios");
            }
        }

        // POST: Reservas/Create/ CREACION DE LA RESERVA
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: Reservas/Create/
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

                // Verificar si la reserva tiene fechas válidas
                if (reserva.FechaInicio != null && reserva.FechaFinal != null && reserva.FechaInicio <= reserva.FechaFinal)
                {
                    // Obtener el rango de fechas entre FechaInicio y FechaFinal
                    var fechasBloqueadas = new List<string>();
                    var diasReserva = (reserva.FechaFinal - reserva.FechaInicio).Days + 1;

                    for (int i = 0; i < diasReserva; i++)
                    {
                        var fecha = reserva.FechaInicio.AddDays(i);
                        fechasBloqueadas.Add(fecha.ToString("dd/MM/yyyy"));
                    }

                    // Asignar las fechas bloqueadas a la reserva
                    reserva.FechasBloqueadas = fechasBloqueadas;

                }
                else
                {
                    // Manejar el caso de fechas inválidas
                    ModelState.AddModelError("", "Las fechas de reserva son inválidas.");
                    ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "CodigoReceptor", reserva.ProductoId);
                    ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Apellido", reserva.UsuarioId);
                    return View(reserva);
                }

                // Guardar la reserva en la base de datos
                _context.Add(reserva);
                await _context.SaveChangesAsync();

                // Redirigir a la acción MisReservas
                return RedirectToAction("MisReservas", "Reservas");
            }

            // Si el usuario no es válido, redirigir a la vista de registro de usuarios
            return RedirectToAction("CreatePublic", "Usuarios");
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
