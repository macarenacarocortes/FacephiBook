using System.ComponentModel.DataAnnotations;

namespace FacephiBook.Models
{
    namespace FacephiBook.Models
    {
        public class Formulario
        {
            public int Id { get; set; }
            [Required(ErrorMessage = "La descripción es requerida.")]
            public string Descripcion { get; set; }
            public int UsuarioId { get; set; }
            public string UsuarioNombre { get; set; }
            public Usuario Usuario { get; set; }
            [Required(ErrorMessage = "El código de producto es requerido.")]
            public int ProductoId { get; set; }
            public Producto Producto { get; set; }

            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
            public DateTime Fecha { get; set; }
        }
    }
}
