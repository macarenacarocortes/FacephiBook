using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FacephiBook.Data;
using FacephiBook.Models;
     
namespace FacephiBook
{
    public class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            // Comprobar y crear los roles predeterminados
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            await CrearRolesAsync(roleManager);
            
            // Comprobar y crear el administrador predeterminado
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
            var dbContext = services.GetRequiredService<FacephiBookContexto>();
            await CrearAdminAsync(userManager, dbContext);


            // Obtener la lista de usuarios de la tabla local
            var usuarios = dbContext.Usuarios.ToList();

            // Recorrer la lista de usuarios y agregarlos si no existen en ASP.NET Identity
            foreach (var usuarioLocal in usuarios)
            {
                // Verificar si el usuario ya existe en ASP.NET Identity
                var usuarioAspNet = await userManager.FindByEmailAsync(usuarioLocal.Email);
                if (usuarioAspNet == null)
                {
                    // El usuario no existe, crearlo en ASP.NET Identity y asignarle su rol
                    var usuarioNuevo = new IdentityUser
                    {
                        UserName = usuarioLocal.Email,
                        Email = usuarioLocal.Email
                    };
                    string usuarioPWD = usuarioLocal.Password;

                    var result = await userManager.CreateAsync(usuarioNuevo, usuarioPWD);
                    if (result.Succeeded)
                    {
                        // Asignar el rol correspondiente
                        var result1 = await userManager.AddToRoleAsync(usuarioNuevo, usuarioLocal.Rol.ToString());
                    }
                }
            }
        }
        private static async Task CrearRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            // Si no existe, se crea el rol predeterminado "Administrador"
            string nombreRol = "Administrador";
            var yaExiste = await roleManager.RoleExistsAsync(nombreRol);
            if (!yaExiste)
                await roleManager.CreateAsync(new IdentityRole(nombreRol));
            // Si no existe, se crea el rol predeterminado "Usuario"
            nombreRol = "Usuario";
            yaExiste = await roleManager.RoleExistsAsync(nombreRol);
            if (!yaExiste)
                await roleManager.CreateAsync(new IdentityRole(nombreRol));
        }
        private static async Task CrearAdminAsync(UserManager<IdentityUser> userManager, FacephiBookContexto dbContext)
        {

            // Se crea el nuevo usuario
            var user = new IdentityUser();
            user.UserName = "admin@facephi.com";
            user.Email = "admin@facephi.com";

            string usuarioPWD = "Facephi123.";

            // Verificar si el correo electrónico ya está siendo utilizado
            var existingUserWithEmail = await userManager.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existingUserWithEmail != null) return;


            var result = await userManager.CreateAsync(user, usuarioPWD);
            // Se asigna el rol de "Administrador" al usuario
            if (result.Succeeded)
            {
                var result1 = await userManager.AddToRoleAsync(user, "Administrador");
            }

        }
    }
}
