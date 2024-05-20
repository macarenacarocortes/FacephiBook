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
        public async Task<IActionResult> Index(string Marca, string CodReceptor, string Nombre, DateTime? FechaInicio)
        {
            //SELECCIONAR TODAS LAS MARCAS
            var marcas = await _context.Productos
                .Select(p => p.Marca)
                .Distinct()
                .ToListAsync();
            marcas.Insert(0, "Todas");

            //ENTREGARLAS A LA VISTA
            ViewBag.Marcas = new SelectList(marcas);

            // Definir la consulta inicial para filtrar productos
            var productosQuery = _context.Productos.AsQueryable();

            if (!string.IsNullOrEmpty(Marca) && Marca != "Todas")
            {
                productosQuery = productosQuery.Where(p => p.Marca == Marca);
            }

            if (!string.IsNullOrEmpty(CodReceptor))
            {
                productosQuery = productosQuery.Where(p => p.CodigoReceptor.Contains(CodReceptor));
            }

            // Incluir los filtros en la consulta de reservas
            var facephiBookContexto = _context.Reservas
                .Include(r => r.Producto)
                .Include(r => r.Usuario)
                .Where(r => productosQuery.Any(p => p.Id == r.ProductoId));

            if (FechaInicio.HasValue)
            {
                // Filtrar las reservas que tengan el día seleccionado dentro de su rango de fechas
                facephiBookContexto = facephiBookContexto.Where(r => FechaInicio >= r.FechaInicio && FechaInicio <= r.FechaFinal);
            }
            if (!string.IsNullOrEmpty(Nombre))
            {
                facephiBookContexto = facephiBookContexto.Where(r => r.Usuario.Nombre.Contains(Nombre));
            }

            ViewBag.Marcas = _context.Productos.Select(p => p.Marca).Distinct().ToList();

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

        public IActionResult Create(int productoId)
        {
            // Obtener el correo electrónico del usuario actual
            var userEmail = User.Identity.Name;

            // Buscar al usuario en la tabla Usuarios basándose en el correo electrónico
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == userEmail);
            
            //Seleccionar de la tabla productos, aquel que tenga el id pasado por el botón resrvar
            var producto = _context.Productos.FirstOrDefault(p => p.Id == productoId);

            //si es nulo y si su rol no es administrador.
            if (usuario != null && usuario.Rol != 0 && producto != null) //si el usuario actual está en la tabla y ademas el producto no es null
            {
                // Obtener todas las reservas asociadas al producto con el Id dado
                var reservas = _context.Reservas
                    .Where(r => r.ProductoId == productoId && r.FechaInicio >= DateTime.Today)
                    .ToList();

                // Filtrar las reservas que tienen devoluciones
                var reservasSinDevoluciones = reservas
                    .Where(r => !_context.Devoluciones.Any(d => d.ReservaId == r.Id))
                    .ToList();

                // Lista para almacenar las fechas bloqueadas
                var fechasBloqueadas = new List<string>();

                
                foreach (var res in reservasSinDevoluciones) //por cada reserva asociada al producto que NO se haya devuelto
                {
                    var diasReserva = (res.FechaFinal - res.FechaInicio).Days + 1; //Seleccionamos los dias desde fechainicio al final
                    for (int i = 0; i < diasReserva; i++)
                    {
                        var fecha = res.FechaInicio.AddDays(i);
                        fechasBloqueadas.Add(fecha.ToString("dd/MM/yyyy")); // se añaden a fechas bloqueadas
                    }
                }

                //Necesitamos encontrar el PRIMER día hábil para reservar dentro de las fechas bloqueadas
                DateTime fechaInicio = DateTime.Now; 
                bool encontrado = false; 

                while (!encontrado)
                {
                    if (!fechasBloqueadas.Contains(fechaInicio.ToString("dd/MM/yyyy")))
                    {
                        
                        encontrado = true;
                    }
                    else
                    {
                        fechaInicio = fechaInicio.AddDays(1);
                    }
                }

                //Creamos la reserva con las fechas bloqueadas
                var reserva = new Reserva
                {
                    UsuarioId = usuario.Id,
                    ProductoId = productoId,
                    FechaInicio = fechaInicio,
                    FechaFinal = fechaInicio,
                    FechasBloqueadas = fechasBloqueadas // Asignar las fechas bloqueadas a la reserva
                };

                // Obtener los Ids de los usuarios asociados a estas reservas
                var usuariosIds = reservasSinDevoluciones.Select(r => r.UsuarioId).ToList();
                // Cargar los nombres de los usuarios asociados a las reservas
                var usuariosReservas = _context.Usuarios.Where(u => usuariosIds.Contains(u.Id)).ToList();
                ViewBag.Reservas = reservasSinDevoluciones; // Pasar las reservas a la vista
                ViewBag.Usuarios = usuariosReservas;

                ViewData["CodigoReceptor"] = producto.CodigoReceptor;
                ViewData["Marca"] = producto.Marca;
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
                  

                    //ENCONTRAR DENTRO DEL RANGO DE NUESTRA NUEVA RESERVA, FECHAS BLOQUEADAS:
                    // Obtener todas las reservas asociadas al producto con el Id dado
                    var reservas = _context.Reservas
                        .Where(r => r.ProductoId == reserva.ProductoId && r.FechaInicio >= DateTime.Today)
                        .ToList();

                    // Filtrar las reservas que tienen devoluciones
                    var reservasSinDevoluciones = reservas
                        .Where(r => !_context.Devoluciones.Any(d => d.ReservaId == r.Id))
                        .ToList();

                    // Lista para almacenar las fechas bloqueadas
                    var fechasBloqueadasAntiguas = new List<string>();


                    foreach (var res in reservasSinDevoluciones) //por cada reserva asociada al producto que NO se haya devuelto
                    {
                        var diasReserva = (res.FechaFinal - res.FechaInicio).Days + 1; //Seleccionamos los dias desde fechainicio al final
                        for (int i = 0; i < diasReserva; i++)
                        {
                            var fecha = res.FechaInicio.AddDays(i);
                            fechasBloqueadasAntiguas.Add(fecha.ToString("dd/MM/yyyy")); // se añaden a fechas bloqueadas Antiguas
                        }
                    }


                    // UNA VEZ GENERADA LA LISTA DE FECHASBLOQUEADAS ANTIGUA: COMPROBAR SI DENTRO ESTÁ CONTENIDA ALGUNA DEL RANGO DE LA NUEVA RESERVA:
                    var ReservaNueva = (reserva.FechaFinal - reserva.FechaInicio).Days + 1;
                    for (int i = 0; i < ReservaNueva; i++)
                    {
                        var fecha = reserva.FechaInicio.AddDays(i);
                        if (fechasBloqueadasAntiguas.Contains(fecha.ToString("dd/MM/yyyy")))
                        {
                            // Obtener los Ids de los usuarios asociados a estas reservas
                            var usuariosIds = reservasSinDevoluciones.Select(r => r.UsuarioId).ToList();
                            // Cargar los nombres de los usuarios asociados a las reservas
                            var usuariosReservas = _context.Usuarios.Where(u => usuariosIds.Contains(u.Id)).ToList();
                            ViewBag.Reservas = reservasSinDevoluciones; // Pasar las reservas a la vista
                            ViewBag.Usuarios = usuariosReservas;

                            //Seleccionar de la tabla productos, aquel que tenga el id pasado por el botón resrvar
                            var producto = _context.Productos.FirstOrDefault(p => p.Id == reserva.ProductoId);

                            ViewData["CodigoReceptor"] = producto.CodigoReceptor;
                            ViewData["Marca"] = producto.Marca;
                            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "CodigoReceptor", "Marca");
                            ViewData["FechasBloqueadas"] = fechasBloqueadasAntiguas; // Pasar las fechas bloqueadas a la vista

                            ModelState.AddModelError("", "Una o más fechas seleccionadas ya están bloqueadas para reservar.");
                            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Apellido", reserva.UsuarioId);
                            return View(reserva);
                        }
                    }

                    //CREAR NUEVAS! fechas Bloqueadas, con la fechaInicio y FechaFinal Nuevas

                    // Obtener el rango de fechas entre FechaInicio y FechaFinal
                    var fechasBloqueadas = new List<string>();


                    var devolucion = _context.Devoluciones.FirstOrDefault(d => d.ReservaId == reserva.Id);
                    if (devolucion != null)
                    {
                        var diasReservaDev = (devolucion.FechaDevolucion - reserva.FechaInicio).Days + 1;

                        for (int i = 0; i < diasReservaDev; i++)
                        {
                            var fecha = reserva.FechaInicio.AddDays(i);
                            fechasBloqueadas.Add(fecha.ToString("dd/MM/yyyy"));
                        }
                    }
                    else {

                        var diasReserva = (reserva.FechaFinal - reserva.FechaInicio).Days + 1;

                        for (int i = 0; i < diasReserva; i++)
                        {
                            var fecha = reserva.FechaInicio.AddDays(i);
                            fechasBloqueadas.Add(fecha.ToString("dd/MM/yyyy"));
                        }
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

            return User.IsInRole("Administrador") ? RedirectToAction(nameof(Index)) : RedirectToAction("MisReservas", "Reservas"); ;
          
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
