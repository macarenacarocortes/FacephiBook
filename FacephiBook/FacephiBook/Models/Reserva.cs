using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FacephiBook.Models
{
    public class Reserva
    {
        public int Id { get; set; }

        public TimeSpan Hora { get; set; } // Si representa la hora del día, considera usar TimeSpan

        [Required(ErrorMessage = "La fecha de inicio es requerida.")]

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha final es requerida.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaFinal { get; set; }

        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        [Required(ErrorMessage = "El nombre del producto es requerido.")]
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }

        public int? DevolucionId { get; set; }
        public Devolucion? Devolucion { get; set; }

        // Agregar una propiedad para almacenar las fechas bloqueadas
        [NotMapped] // Indica a EF que ignore esta propiedad al mapearla a la base de datos

        public List<string> FechasBloqueadas { get; set; }

        public DateTime FechaReserva { get; set; }

    }
}
