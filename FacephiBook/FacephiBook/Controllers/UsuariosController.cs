using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FacephiBook.Data;
using FacephiBook.Models;
using Microsoft.AspNetCore.Identity;
using FacephiBook.Areas.Identity.Pages.Account;

namespace FacephiBook.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly FacephiBookContexto _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public UsuariosController(FacephiBookContexto context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            var facephiBookContexto = _context.Usuarios.Include(u => u.Chapter).Include(u => u.Squad);
            return View(await facephiBookContexto.ToListAsync());
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.Chapter)
                .Include(u => u.Squad)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            ViewData["ChapterId"] = new SelectList(_context.Chapters, "Id", "Nombre");
            ViewData["SquadId"] = new SelectList(_context.Squads, "Id", "Nombre");
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Apellido,Email,Password,ChapterId,SquadId")] Usuario usuario)
        {
            // Verificar que los campos necesarios no estén vacíos
            if (!string.IsNullOrEmpty(usuario.Nombre) &&
                !string.IsNullOrEmpty(usuario.Apellido) &&
                !string.IsNullOrEmpty(usuario.Email) &&
                !string.IsNullOrEmpty(usuario.Password))
            {
                try
                {
                    // Crear el nuevo usuario en ASP.NET Identity

                    // Se crea el nuevo usuario
                    var user = new IdentityUser();
                    user.UserName = usuario.Email;
                    user.Email = usuario.Email;

                    var result = await _userManager.CreateAsync(user, usuario.Password);

                    if ((result.Succeeded || result.Errors.Any(error => error.Code == "DuplicateUserName")) &&
                        (result.Errors.All(error => error.Code == "DuplicateUserName")))
                    {
                        await _userManager.AddToRoleAsync(user, "Usuario");

                        // Agregar el usuario a la base de datos local
                        _context.Add(usuario);
                        await _context.SaveChangesAsync();

                        // Iniciar sesión después de crear el usuario
                        await _signInManager.SignInAsync(user, isPersistent: false);

                        // Redirigir al usuario a alguna página después de la creación exitosa
                        return RedirectToAction("Index", "Catalogo");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }

                        // Si hay errores de validación, recargar el formulario con los datos proporcionados por el usuario
                        ViewData["ChapterId"] = new SelectList(_context.Chapters, "Id", "Nombre");
                        ViewData["SquadId"] = new SelectList(_context.Squads, "Id", "Nombre");
                        return View(usuario);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Ocurrió un error al crear el usuario: " + ex.Message);
                }
            }

            // Si falta algún dato o hubo un error, volver a cargar el formulario
            ViewData["ChapterId"] = new SelectList(_context.Chapters, "Id", "Nombre");
            ViewData["SquadId"] = new SelectList(_context.Squads, "Id", "Nombre");
            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            ViewData["ChapterId"] = new SelectList(_context.Chapters, "Id", "Nombre");
            ViewData["SquadId"] = new SelectList(_context.Squads, "Id", "Nombre");
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Apellido,Email,Password,ChapterId,SquadId")] Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return NotFound();
            }

            if (usuario.Nombre != null && usuario.Nombre != "" && usuario.Apellido != null && usuario.Apellido != ""
                && usuario.Email != null && usuario.Email != "" && usuario.Password != null && usuario.Password != "")
            {
                try
                {


                    // Asignar el rol "Usuario" al usuario
                    var user = new IdentityUser { UserName = usuario.Nombre, Email = usuario.Email };
                    var result = await _userManager.CreateAsync(user, usuario.Password);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "Usuario"); //Añade a UserIdentity
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        // Si hay errores de validación, recargar el formulario con los datos proporcionados por el usuario
                        ViewData["ChapterId"] = new SelectList(_context.Chapters, "Id", "Nombre");
                        ViewData["SquadId"] = new SelectList(_context.Squads, "Id", "Nombre");
                        return View(usuario);
                    }

                    // Agregar el usuario a la base de datos
                    _context.Add(usuario);
                    await _context.SaveChangesAsync();

                    // Redirigir al usuario a alguna página después de la creación exitosa
                    return RedirectToAction("Index", "Catalogo");

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
              
            }
            ViewData["ChapterId"] = new SelectList(_context.Chapters, "Id", "Nombre");
            ViewData["SquadId"] = new SelectList(_context.Squads, "Id", "Nombre");
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.Chapter)
                .Include(u => u.Squad)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Usuarios == null)
            {
                return Problem("Entity set 'FacephiBookContexto.Usuarios'  is null.");
            }
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
          return (_context.Usuarios?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public IActionResult CreateAdmin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAdmin([Bind("Email,Password,ConfirmPassword")] RegisterModel.InputModel model)
        {
            // Se crea el nuevo usuario
            var user = new IdentityUser();
            user.UserName = model.Email;
            user.Email = model.Email;
            string usuarioPWD = model.Password;


            // Verificar si el correo electrónico ya está siendo utilizado en ASP.NET Identity
            var existingUserWithEmail = await _userManager.FindByEmailAsync(model.Email);
            if (existingUserWithEmail != null)
            {
                // El correo electrónico ya está en uso, mostrar mensaje de error
                ModelState.AddModelError(string.Empty, "El correo electrónico ya está siendo utilizado por otro usuario.");
                return View(model);
            }

            var result = await _userManager.CreateAsync(user, usuarioPWD);
            // Se asigna el rol de "Administrador" al usuario
            if (result.Succeeded)
            {
                var result1 = await _userManager.AddToRoleAsync(user, "Administrador");
                return RedirectToAction(nameof(Index));
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

               
                return View(model);
            }
        }



        public IActionResult CreatePublic()
        {
            ViewData["ChapterId"] = new SelectList(_context.Chapters, "Id", "Nombre");
            ViewData["SquadId"] = new SelectList(_context.Squads, "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePublic([Bind("Id,Nombre,Apellido,Email,Password,ChapterId,SquadId")] Usuario usuario)
        {
            // Verificar que los campos necesarios no estén vacíos
            if (!string.IsNullOrEmpty(usuario.Nombre) &&
                !string.IsNullOrEmpty(usuario.Apellido) &&
                !string.IsNullOrEmpty(usuario.Email) &&
                !string.IsNullOrEmpty(usuario.Password))
            {
                try
                {
                    // Crear el nuevo usuario en ASP.NET Identity

                    // Se crea el nuevo usuario
                    var user = new IdentityUser();
                    user.UserName = usuario.Email;
                    user.Email = usuario.Email;

                    var result = await _userManager.CreateAsync(user, usuario.Password);

                    if ((result.Succeeded || result.Errors.Any(error => error.Code == "DuplicateUserName")) &&
                        (result.Errors.All(error => error.Code == "DuplicateUserName")))
                    {
                        await _userManager.AddToRoleAsync(user, "Usuario");

                        // Agregar el usuario a la base de datos local
                        _context.Add(usuario);
                        await _context.SaveChangesAsync();

                        // Iniciar sesión después de crear el usuario
                        await _signInManager.SignInAsync(user, isPersistent: false);

                        // Redirigir al usuario a alguna página después de la creación exitosa
                        return RedirectToAction("Index", "Catalogo");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }

                        // Si hay errores de validación, recargar el formulario con los datos proporcionados por el usuario
                        ViewData["ChapterId"] = new SelectList(_context.Chapters, "Id", "Nombre");
                        ViewData["SquadId"] = new SelectList(_context.Squads, "Id", "Nombre");
                        return View(usuario);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Ocurrió un error al crear el usuario: " + ex.Message);
                }
            }

            // Si falta algún dato o hubo un error, volver a cargar el formulario
            ViewData["ChapterId"] = new SelectList(_context.Chapters, "Id", "Nombre");
            ViewData["SquadId"] = new SelectList(_context.Squads, "Id", "Nombre");
            return View(usuario);
        }
    }
}
