using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Abstracciones.Modelos
{
    [Table("Producto")]
    public class ProductoDto
    {
        public int ProductoId { get; set; }

        [Required(ErrorMessage = "El SKU es requerido.")]
        [StringLength(50, ErrorMessage = "El SKU no puede exceder 50 caracteres.")]
        public string SKU { get; set; } = default!;

        [Required(ErrorMessage = "El nombre es requerido.")]
        [StringLength(150, ErrorMessage = "El nombre no puede exceder 150 caracteres.")]
        public string Nombre { get; set; } = default!;

        public int? CategoriaProductoId { get; set; }

        [Required(ErrorMessage = "La unidad de medida es requerida.")]
        public int UnidadMedidaId { get; set; }

        [Required(ErrorMessage = "El costo es requerido.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Costo { get; set; }

        [Required(ErrorMessage = "El precio es requerido.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "El stock mínimo es requerido.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal StockMinimo { get; set; }

        public bool Activo { get; set; } = true;

        [Required(ErrorMessage = "La fecha de creación es requerida.")]
        public DateTime FechaCreacion { get; set; }

        [Required(ErrorMessage = "El stock es requerido.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Stock { get; set; }
    }
}