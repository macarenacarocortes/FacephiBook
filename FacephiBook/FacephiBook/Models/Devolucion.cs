﻿using System.ComponentModel.DataAnnotations;

namespace FacephiBook.Models
{
    public class Devolucion
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "La fecha de devolución es requerida.")]
        public DateTime FechaDevolucion { get; set; }
        public int ReservaId { get; set; }
        public Reserva Reserva { get; set; }

        public int UsuarioId { get; set; }

        public ICollection<Reserva> Reservas { get; set; }

    }

}
