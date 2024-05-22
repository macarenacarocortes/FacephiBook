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
    public class ProductosController : Controller
    {
        private readonly FacephiBookContexto _context;

        public ProductosController(FacephiBookContexto context)
        {
            _context = context;
        }

        // GET: Productos
        public async Task<IActionResult> Index(string strCadenaBusqueda, string Marca, string CodReceptor, string RelacionAspecto, string Estado, string Categoria, string Gama)
        {
            // Obtener todas las marcas disponibles
            var marcas = await _context.Productos
                .Select(p => p.Marca)
                .Distinct()
                .ToListAsync();
            marcas.Insert(0, "Todas");

            // Obtener todas las relaciones de aspecto disponibles
            var relacionesAspecto = await _context.Productos
                .Select(p => p.RelacionAspecto)
                .Distinct()
                .ToListAsync();
            relacionesAspecto.Insert(0, "Todas");

            // Obtener todos los estados disponibles
            var estados = await _context.Estados
                .Select(e => e.Nombre)
                .Distinct()
                .ToListAsync();
            estados.Insert(0, "Todos");

            // Obtener todas las categorías disponibles
            var categorias = await _context.Categorias
                .Select(c => c.Nombre)
                .Distinct()
                .ToListAsync();
            categorias.Insert(0, "Todas");

            var gama = await _context.Productos
                .Select(c => c.Gama)
                .Distinct()
                .ToListAsync();
            gama.Insert(0, "Todas");

            ViewBag.Marcas = new SelectList(marcas);
            ViewBag.RelacionesAspecto = new SelectList(relacionesAspecto);
            ViewBag.Estados = new SelectList(estados);
            ViewBag.Categorias = new SelectList(categorias);
            ViewBag.Gama = new SelectList(gama);

            // Filtrar los productos según los criterios seleccionados
            IQueryable<Producto> productosQuery = _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Estado)
                .Include(p => p.Reserva);

            if (!string.IsNullOrEmpty(Marca) && Marca != "Todas")
            {
                productosQuery = productosQuery.Where(p => p.Marca == Marca);
            }

            if (!string.IsNullOrEmpty(CodReceptor))
            {
                productosQuery = productosQuery.Where(p => p.CodigoReceptor.Contains(CodReceptor));
            }

            if (!string.IsNullOrEmpty(RelacionAspecto) && RelacionAspecto != "Todas")
            {
                productosQuery = productosQuery.Where(p => p.RelacionAspecto == RelacionAspecto);
            }

            if (!string.IsNullOrEmpty(Estado) && Estado != "Todos")
            {
                productosQuery = productosQuery.Where(p => p.Estado.Nombre == Estado);
            }

            if (!string.IsNullOrEmpty(Categoria) && Categoria != "Todas")
            {
                productosQuery = productosQuery.Where(p => p.Categoria.Nombre == Categoria);
            }

            if (!string.IsNullOrEmpty(Gama) && Gama != "Todas")
            {
                productosQuery = productosQuery.Where(p => p.Gama == Gama);
            }

            if (!string.IsNullOrEmpty(strCadenaBusqueda))
            {
                productosQuery = productosQuery.Where(p => p.Nombre.Contains(strCadenaBusqueda));
            }

            ViewBag.Marcas = _context.Productos.Select(p => p.Marca).Distinct().ToList();
            ViewBag.RelacionesAspecto = _context.Productos.Select(p => p.RelacionAspecto).Distinct().ToList();
            ViewBag.Estados = _context.Estados.Select(p => p.Nombre).Distinct().ToList();
            ViewBag.Categorias = _context.Categorias.Select(p => p.Nombre).Distinct().ToList();
            ViewBag.Gamas = _context.Productos.Select(p => p.Gama).Distinct().ToList();

            var productos = await productosQuery.ToListAsync();
            return View(productos);
        }

        // GET: Productos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Productos == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Estado)
                .Include(p => p.Reserva)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // GET: Productos/Create
        public IActionResult Create()
        {


            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre");
            ViewData["EstadoId"] = new SelectList(_context.Estados, "Id", "Nombre");
            ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id");
            return View();
        }

        // POST: Productos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Marca,CodigoReceptor,SistemaOperativo,Antutu,RelacionAspecto,Stock,PixelFrontal,PixelTrasera,PixelBining,Foco,Gama,ResCamara,ResVideo,CategoriaId,EstadoId,ReservaId,ContadorReserva")] Producto producto)
        {


           if (!string.IsNullOrWhiteSpace(producto.Nombre) &&
                !string.IsNullOrWhiteSpace(producto.Marca) &&
                !string.IsNullOrWhiteSpace(producto.CodigoReceptor) &&
                  !string.IsNullOrWhiteSpace(producto.SistemaOperativo) &&
               producto.EstadoId != null && producto.EstadoId != 0 &&
             producto.CategoriaId != null && producto.CategoriaId != 0)
            {
                _context.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else {

                ModelState.AddModelError(string.Empty, "Por favor, complete todos los campos obligatorios correctamente.");
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre", producto.CategoriaId);
            ViewData["EstadoId"] = new SelectList(_context.Estados, "Id", "Nombre", producto.EstadoId);
            ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id", producto.ReservaId);
            return View(producto);
        }

        // GET: Productos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Productos == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre", producto.CategoriaId);
            ViewData["EstadoId"] = new SelectList(_context.Estados, "Id", "Nombre", producto.EstadoId);
            ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id", producto.ReservaId);
            return View(producto);
        }

        // POST: Productos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Marca,CodigoReceptor,SistemaOperativo,Antutu,RelacionAspecto,Stock,PixelFrontal,PixelTrasera,PixelBining,Foco,Gama,ResCamara,ResVideo,CategoriaId,EstadoId,ReservaId,ContadorReserva")] Producto producto)
        {
            if (id != producto.Id)
            {
                return NotFound();
            }






            if (!string.IsNullOrWhiteSpace(producto.Nombre) &&
                !string.IsNullOrWhiteSpace(producto.Marca) &&
                !string.IsNullOrWhiteSpace(producto.CodigoReceptor) &&
                  !string.IsNullOrWhiteSpace(producto.SistemaOperativo) &&
               producto.EstadoId != null && producto.EstadoId != 0 &&
             producto.CategoriaId != null && producto.CategoriaId != 0)
            {
                try
                {
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.Id))
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
            else
            {

                ModelState.AddModelError(string.Empty, "Por favor, complete todos los campos obligatorios correctamente.");
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre", producto.CategoriaId);
            ViewData["EstadoId"] = new SelectList(_context.Estados, "Id", "Nombre", producto.EstadoId);
            ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id", producto.ReservaId);
            return View(producto);
        }

        // GET: Productos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Productos == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Estado)
                .Include(p => p.Reserva)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Productos == null)
            {
                return Problem("Entity set 'FacephiBookContexto.Productos'  is null.");
            }
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                // Encontrar todas las reservas asociadas al producto
                var reservas = _context.Reservas.Where(r => r.ProductoId == id).ToList();
                if (reservas != null)
                {
                    // Eliminar todas las reservas asociadas al usuario
                    _context.Reservas.RemoveRange(reservas);
                    await _context.SaveChangesAsync();

                }
               _context.Productos.Remove(producto);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductoExists(int id)
        {
          return (_context.Productos?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
