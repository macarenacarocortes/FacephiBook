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
        public async Task<IActionResult> Index()
        {
            var facephiBookContexto = _context.Productos.Include(p => p.Categoria).Include(p => p.Estado).Include(p => p.Reserva);
            return View(await facephiBookContexto.ToListAsync());
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
