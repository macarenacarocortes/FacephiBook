namespace FacephiBook.Models
{
    public class Reserva
    {
        public int Id { get; set; }
        public string Hora { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFinal { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        // Colección de Usuarios asociados a esta reserva
        public ICollection<Usuario> Usuarios { get; set; }

        // Id del Usuario asociado a esta reserva
        public int IdUsuario { get; set; }

        // Propiedad para representar solo la fecha sin la hora
        public DateTime Fecha
        {
            get { return FechaInicio.Date; } // Obtener solo la fecha sin la hora
            set { FechaInicio = value; } // Establecer la fecha sin la hora
        }
    }
}
