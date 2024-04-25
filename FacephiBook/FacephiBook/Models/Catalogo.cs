namespace FacephiBook.Models
{
    public class Catalogo
    {

        public string? ProductoId { get; set; }
        public virtual Producto? Producto { get; set; }

        public string? EstadoId { get; set; }
        public Estado? Estado { get; set; }

        public string? UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

    }
}
