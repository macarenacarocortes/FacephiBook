using System.ComponentModel.DataAnnotations;

namespace FacephiBook.Models
{
    public class Estado
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El nombre del estado es requerido.")]
        public string Nombre { get; set; }

        // Colección de Productos asociados a este estado
        public ICollection<Producto> Productos { get; set; }
    }
}
