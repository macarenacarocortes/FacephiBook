using Microsoft.EntityFrameworkCore;

namespace FacephiBook.Data
{
    public class FacephiBookContexto : DbContext
    {

        public FacephiBookContexto(DbContextOptions<FacephiBookContexto> options)
            : base(options)
        {
        }

        /*public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Devolucion> Devoluciones { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Squad> Squads { get; set; }
        public DbSet<Estado> Estados { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }*/

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // Deshabilitar eliminación en cascada en todas las relaciones
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
      }
}
