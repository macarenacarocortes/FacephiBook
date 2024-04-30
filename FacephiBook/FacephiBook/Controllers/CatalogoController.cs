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


        public async Task<IActionResult> Index(string strCadenaBusqueda, string Marca)
        {
            // Obtener todas las marcas disponibles
            var marcas = await _context.Productos
                .Select(p => p.Marca)
                .Distinct()
                .ToListAsync();

            // Insertar "Todas" al principio de la lista
            marcas.Insert(0, "Todas");

            // Crear el SelectList para la vista
            ViewBag.Marcas = new SelectList(marcas);

            // Filtrar los productos según la marca seleccionada
            IQueryable<Producto> productosQuery = _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Estado)
                .Include(p => p.Reserva);

            if (!string.IsNullOrEmpty(Marca) && Marca != "Todas")
            {
                productosQuery = productosQuery.Where(p => p.Marca == Marca);
            }

            if (!string.IsNullOrEmpty(strCadenaBusqueda))
            {
                productosQuery = productosQuery.Where(p => p.Nombre.Contains(strCadenaBusqueda));
            }

            ViewBag.Marcas = _context.Productos.Select(p => p.Marca).Distinct().ToList();

            var productos = await productosQuery.ToListAsync();
            return View(productos);

            //var facephiBookContexto = _context.Productos.Include(p => p.Categoria).Include(p => p.Estado).Include(p => p.Reserva);
            //return View(await facephiBookContexto.ToListAsync());
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

