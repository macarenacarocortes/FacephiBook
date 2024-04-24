using System.ComponentModel.DataAnnotations;

namespace FacephiBook.Models
{
    public class Devolucion
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "La fecha de devolución es requerida.")]
        public DateTime Fecha { get; set; }
        public int ReservaId { get; set; }
        public Reserva Reserva { get; set; }
        // Id del Usuario asociado a esta devolución
        public int IdUsuario { get; set; }

        // Id de la Reserva asociada a esta devolución
        public int IdReserva { get; set; }

        // Propiedad para representar solo la fecha sin la hora
        public DateTime FechaDevolucion
        {
            get { return Fecha.Date; } // Obtener solo la fecha sin la hora
            set { Fecha = value; } // Establecer la fecha sin la hora
        }
    }

}
