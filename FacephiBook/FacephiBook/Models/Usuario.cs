using System.ComponentModel.DataAnnotations;

namespace FacephiBook.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El nombre es un campo requerido.")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El apellido es un campo requerido.")]
        public string Apellido { get; set; }
        [Required(ErrorMessage = "El correo electrónico es un campo requerido.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "La contraseña es requerida.")]
        public string Password { get; set; }
        public int ChapterId { get; set; }
        public int SquadId { get; set; }
        public Chapter Chapter { get; set; }
        public Squad Squad { get; set; }

        // Colección de Chapter asociados a este usuario
        public ICollection<Chapter> Chapters { get; set; }

        // Colección de Squad asociados a este usuario
        public ICollection<Squad> Squads { get; set; }

        // Colección de Reservas asociadas a este usuario
        public ICollection<Reserva> Reservas { get; set; }

    }
}
