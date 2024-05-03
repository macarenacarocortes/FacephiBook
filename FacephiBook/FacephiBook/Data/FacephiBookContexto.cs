using Microsoft.EntityFrameworkCore;
using FacephiBook.Models;

namespace FacephiBook.Data
{
    public class FacephiBookContexto : DbContext
    {

        public FacephiBookContexto(DbContextOptions<FacephiBookContexto> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Devolucion> Devoluciones { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Squad> Squads { get; set; }
        public DbSet<Estado> Estados { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public IEnumerable<object> RelacionesAspecto { get; internal set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // Deshabilitar eliminación en cascada en todas las relaciones
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            // Configurar la relación entre Categoria y Producto
    modelBuilder.Entity<Categoria>()
        .HasMany(c => c.Productos)
        .WithOne(p => p.Categoria)
        .HasForeignKey(p => p.CategoriaId)
        .IsRequired(); // Ajusta esto según tus requerimientos de negocio



            // Configurar la relación entre Estado y Producto
            modelBuilder.Entity<Estado>()
                .HasMany(e => e.Productos)
                .WithOne(p => p.Estado)
                .HasForeignKey(p => p.EstadoId)
                .IsRequired(); // Ajusta esto según tus requerimientos de negocio

            // Configurar la relación entre Reserva y Usuario
            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Usuario)
                .WithMany(u => u.Reservas)
                .HasForeignKey(r => r.IdUsuario)
                .IsRequired(); // Ajusta esto según tus requerimientos de negocio
        
    }
      }
}
