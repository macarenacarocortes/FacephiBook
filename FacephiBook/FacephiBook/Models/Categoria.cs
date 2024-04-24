using System.ComponentModel.DataAnnotations;

namespace FacephiBook.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El nombre de la categoría es requerido.")]
        public string Nombre { get; set; }

        // Colección de Productos asociados a esta categoría
        public ICollection<Producto> Productos { get; set; }
    }
}
