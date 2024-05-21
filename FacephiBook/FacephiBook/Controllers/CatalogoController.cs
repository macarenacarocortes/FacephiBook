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
    public class CatalogoController : Controller
    {
        private readonly FacephiBookContexto _context;

        public CatalogoController(FacephiBookContexto context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string strCadenaBusqueda, string Marca, string CodReceptor, string RelacionAspecto, string Estado, string Categoria)
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

            ViewBag.Marcas = new SelectList(marcas);
            ViewBag.RelacionesAspecto = new SelectList(relacionesAspecto);
            ViewBag.Estados = new SelectList(estados);
            ViewBag.Categorias = new SelectList(categorias);

            // Filtrar los productos según los criterios seleccionados
            IQueryable<Producto> productosQuery = _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Estado)
                .Include(p => p.Reserva) ;

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

            if (!string.IsNullOrEmpty(strCadenaBusqueda))
            {
                productosQuery = productosQuery.Where(p => p.Nombre.Contains(strCadenaBusqueda));
            }

            ViewBag.Marcas = _context.Productos.Select(p => p.Marca).Distinct().ToList();
            ViewBag.RelacionesAspecto = _context.Productos.Select(p => p.RelacionAspecto).Distinct().ToList();
            ViewBag.Estados = _context.Estados.Select(p => p.Nombre).Distinct().ToList();
            ViewBag.Categorias = _context.Categorias.Select(p => p.Nombre).Distinct().ToList();

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
    }
}
