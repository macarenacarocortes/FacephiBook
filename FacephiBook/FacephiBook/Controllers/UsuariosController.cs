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
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create *ESTE ES PARA CREAR UN USUARIO PÚBLICO, DESDE ADMIN*
        public IActionResult Create()
        {
            ViewData["ChapterId"] = new SelectList(_context.Chapters, "Id", "Nombre");
            ViewData["SquadId"] = new SelectList(_context.Squads, "Id", "Nombre");
            return View();
        }



        // POST: Usuarios/Create  *ESTE ES PARA CREAR UN USUARIO PÚBLICO, DESDE ADMIN*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Apellido,Email,Password,ChapterId,SquadId")] Usuario usuario)
        {
            usuario.Rol = Rol.Usuario; 

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

                        return RedirectToAction("Index");
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




        // GET: Usuarios/Edit/5 *EDITAR USUARIO PÚBLICO, DESDE ADMIN*
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





        //*EDITAR USUARIO PÚBLICO, DESDE ADMIN*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Apellido,Email,Password,ChapterId,SquadId,Rol")] Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return NotFound();
            }

            if (usuario.Nombre != null && usuario.Nombre != "" && usuario.Apellido != null && usuario.Apellido != ""
                && usuario.Email != null && usuario.Email != "" && usuario.Password != null && usuario.Password != "" && usuario.Rol != null)
            {
                try
                {


                    // Obtener el usuario actual por su nombre de usuario (correo electrónico en este caso)
                    var user = await _userManager.FindByNameAsync(usuario.Email);


                    if (user == null)
                    {
                        return NotFound(); // Usuario no encontrado
                    }

                    // Actualizar los datos del usuario en la base de datos
                    user.UserName = usuario.Email;
                    user.Email = usuario.Email;

                    var result = await _userManager.UpdateAsync(user);
                    if (!result.Succeeded)
                    {
                        // Manejar errores de actualización
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        // Volver a mostrar el formulario de edición con los errores
                        return View(usuario);
                    }


                    var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var changePasswordResult = await _userManager.ResetPasswordAsync(user, resetToken, usuario.Password);

                    if (!changePasswordResult.Succeeded)
                    {
                        // Manejar errores de cambio de contraseña
                        foreach (var error in changePasswordResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        // Volver a mostrar el formulario de edición con los errores
                        ViewData["ChapterId"] = new SelectList(_context.Chapters, "Id", "Nombre");
                        ViewData["SquadId"] = new SelectList(_context.Squads, "Id", "Nombre");
                        return View(usuario);
                    }


                    // Actualizar el rol del usuario
                    var rolesAnteriores = await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRolesAsync(user, rolesAnteriores);//Eliminamos rol
                    await _userManager.AddToRoleAsync(user, usuario.Rol.ToString()); //Asignamos rol


                    // Actualizar usuario a la base de datos
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();

                    // Redirigir al usuario a alguna página después de la creación exitosa
                    return RedirectToAction("Index");

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




        // GET: Usuarios/Delete/5 *BORRAR USUARIO PÚBLICO, DESDE ADMIN*
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
            // Buscar el usuario en la tabla Usuarios por su ID
            var usuarioToDelete = await _context.Usuarios.FindAsync(id);
            if (usuarioToDelete == null)
            {
                return NotFound();
            }

            // Obtener el correo electrónico del usuario
            var email = usuarioToDelete.Email;

            // Eliminar el usuario de ASP.NET Identity por correo electrónico
            var usuarioIdentity = await _userManager.FindByEmailAsync(email);
            if (usuarioIdentity != null)
            {
                var result = await _userManager.DeleteAsync(usuarioIdentity);
                if (!result.Succeeded)
                {
                    // Manejar el caso en el que no se pudo eliminar el usuario de ASP.NET Identity
                    ModelState.AddModelError(string.Empty, "Error al eliminar el usuario de ASP.NET Identity.");
                    return View("Error");
                }
            }

            // Ahora puedes eliminar la entidad de la tabla Usuarios
            _context.Usuarios.Remove(usuarioToDelete);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
          return (_context.Usuarios?.Any(e => e.Id == id)).GetValueOrDefault();
        }


        //*CREAR USUARIO ADMINISTRADOR, DESDE ADMIN*
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

            if (model.Password != null & model.Password != "" && model.ConfirmPassword != null & model.ConfirmPassword != "")
            {
                var result = await _userManager.CreateAsync(user, usuarioPWD);
                // Se asigna el rol de "Administrador" al usuario

                if (result.Succeeded)
                {
                    var result1 = await _userManager.AddToRoleAsync(user, "Administrador");

                    // Crear el usuario en tu tabla "Usuarios"
                    var usuario = new Usuario
                    {
                        Nombre = model.Email,
                        Apellido = " ",
                        Email = model.Email,
                        Password = model.Password,
                        ChapterId = 1,
                        SquadId = 1,
                        Rol = Rol.Administrador // Asignar el rol en tu tabla de usuarios
                    };

                    _context.Add(usuario);
                    await _context.SaveChangesAsync();


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
            else {
                ModelState.AddModelError(string.Empty, "Por favor, complete todos los campos obligatorios correctamente.");
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

            usuario.Rol = Rol.Usuario;

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
        public async Task<IActionResult> MisDatos()
        {
            
            // Obtener el usuario actualmente autenticado
            var userEmail = User.Identity.Name; // Suponiendo que el nombre de usuario es el correo electrónico
            if (string.IsNullOrEmpty(userEmail))
            {
                return NotFound();
            }

            // Buscar el usuario a editar por su correo electrónico
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (usuario == null)
            {
                return NotFound();
            }

            // Obtener el email del usuario actual
            var currentUserEmail = User.Identity.Name;
            ViewData["CurrentEmail"] = currentUserEmail;

            // Cargar datos adicionales necesarios para la vista
            ViewData["ChapterId"] = new SelectList(_context.Chapters, "Id", "Nombre");
            ViewData["SquadId"] = new SelectList(_context.Squads, "Id", "Nombre");

            return View(usuario);
        }

       //EDITAR TUS DATOS PERSONALES COMO PÚBLICO
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MisDatos(int id, [Bind("Id,Nombre,Apellido,Email,Password,ChapterId,SquadId")] Usuario usuario)
        {
            usuario.Rol = Rol.Usuario;
            if (id != usuario.Id)
            {
                return NotFound();
            }

            if (usuario.Nombre != null && usuario.Nombre != "" && usuario.Apellido != null && usuario.Apellido != ""
                && usuario.Email != null && usuario.Email != "" && usuario.Password != null && usuario.Password != "")
            {
                try
                {
                    // Obtener el usuario actual
                    var user = await _userManager.GetUserAsync(User);

                    if (user == null)
                    {
                        return NotFound(); // Usuario no encontrado
                    }

                    // Actualizar los datos del usuario en la base de datos
                    user.UserName = usuario.Email;
                    user.Email = usuario.Email;
                 
                    var result = await _userManager.UpdateAsync(user);
                    if (!result.Succeeded)
                    {
                        // Manejar errores de actualización
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        // Volver a mostrar el formulario de edición con los errores
                        return View(usuario);
                    }


                    var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var changePasswordResult = await _userManager.ResetPasswordAsync(user, resetToken, usuario.Password); 
                    
                    if (!changePasswordResult.Succeeded)
                    {
                        // Manejar errores de cambio de contraseña
                        foreach (var error in changePasswordResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        // Volver a mostrar el formulario de edición con los errores
                        ViewData["ChapterId"] = new SelectList(_context.Chapters, "Id", "Nombre");
                        ViewData["SquadId"] = new SelectList(_context.Squads, "Id", "Nombre");
                        return View(usuario);
                    }

                    // Actualizar usuario a la base de datos
                    _context.Update(usuario);
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

    }
}
