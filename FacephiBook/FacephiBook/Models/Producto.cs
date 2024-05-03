using System.ComponentModel.DataAnnotations;

namespace FacephiBook.Models
{
    public class Producto
    {
        
            public int Id { get; set; }
            [Required(ErrorMessage = "El nombre del producto es requerido.")]
            public string Nombre { get; set; }
            [Required(ErrorMessage = "La marca es requerida.")]
            public string Marca { get; set; }
            [Required(ErrorMessage = "El código receptor es requerido.")]
            public string CodigoReceptor { get; set; }
            [Required(ErrorMessage = "El sistema operativo es requerido.")]
            public string? SistemaOperativo { get; set; }
            public string? Antutu { get; set; }
        public string? RelacionAspecto { get; set; }
        public int? Stock { get; set; }
            public float? PixelFrontal { get; set; }
            public float? PixelTrasera { get; set; }
             public bool? PixelBining { get; set; }
        public bool? Foco { get; set; }
        public string? Gama { get; set; }
        public float? ResCamara { get; set; }
        public float? ResVideo { get; set; }
        public int CategoriaId { get; set; }
            public int? EstadoId { get; set; }
            public int? ReservaId { get; set; }
        public int? ContadorReserva { get; set; }
        public Categoria Categoria { get; set; }
            public Estado Estado { get; set; }
            public Reserva? Reserva { get; set; }

            // Colección de Reservas asociadas a este producto
            public ICollection<Reserva> Reservas { get; set; }

            // Colección de Estados asociados a este producto
            public ICollection<Estado> Estados { get; set; }

            // Colección de Categorías asociadas a este producto
            public ICollection<Categoria> Categorias { get; set; }


        
    }
}
