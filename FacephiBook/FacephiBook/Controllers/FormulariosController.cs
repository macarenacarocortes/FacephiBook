using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FacephiBook.Data;
using FacephiBook.Models.FacephiBook.Models;
using System.Globalization;

namespace FacephiBook.Controllers
{
    public class FormulariosController : Controller
    {
        private readonly FacephiBookContexto _context;

        public FormulariosController(FacephiBookContexto context)
        {
            _context = context;
        }

        // GET: Formularios
        public async Task<IActionResult> Index()
        {
            var facephiBookContexto = _context.Formulario.Include(f => f.Producto).Include(f => f.Usuario);
            return View(await facephiBookContexto.ToListAsync());
        }

        // GET: Formularios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Formulario == null)
            {
                return NotFound();
            }

            var formulario = await _context.Formulario
                .Include(f => f.Producto)
                .Include(f => f.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (formulario == null)
            {
                return NotFound();
            }

            return View(formulario);
        }

        // GET: Formularios/Create
        public IActionResult Create()
        {

            // 1. Obtener el usuario que ha iniciado sesión
            var email = User.Identity.Name;
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == email);


            if (usuario == null)
            {
                return NotFound(); // Manejar el caso en que el usuario no sea encontrado
            }

            // 2. Asociar el usuario obtenido al modelo de formulario
            var formulario = new Formulario
            {
                UsuarioNombre = usuario.Nombre,
                UsuarioId = usuario.Id,
                Fecha = DateTime.Now //
            };

            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "CodigoReceptor");
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Nombre");
            return View(formulario);
        }

        // POST: Formularios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Descripcion,UsuarioId,UsuarioNombre,ProductoId,Fecha")] Formulario formulario)
        {
           
                _context.Add(formulario);
                await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Catalogo");

        }

        // GET: Formularios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Formulario == null)
            {
                return NotFound();
            }

            var formulario = await _context.Formulario.FindAsync(id);
            if (formulario == null)
            {
                return NotFound();
            }
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "CodigoReceptor", formulario.ProductoId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Apellido", formulario.UsuarioId);
            return View(formulario);
        }

        // POST: Formularios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Descripcion,UsuarioId,ProductoId,Fecha")] Formulario formulario)
        {
            if (id != formulario.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(formulario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FormularioExists(formulario.Id))
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
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "CodigoReceptor", formulario.ProductoId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Apellido", formulario.UsuarioId);
            return View(formulario);
        }

        // GET: Formularios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Formulario == null)
            {
                return NotFound();
            }

            var formulario = await _context.Formulario
                .Include(f => f.Producto)
                .Include(f => f.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (formulario == null)
            {
                return NotFound();
            }

            // Llama directamente al método DeleteConfirmed y devuelve su resultado
            return await DeleteConfirmed(id.Value);

        }

        // POST: Formularios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Formulario == null)
            {
                return Problem("Entity set 'FacephiBookContexto.Formulario'  is null.");
            }
            var formulario = await _context.Formulario.FindAsync(id);
            if (formulario != null)
            {
                _context.Formulario.Remove(formulario);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FormularioExists(int id)
        {
          return (_context.Formulario?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
