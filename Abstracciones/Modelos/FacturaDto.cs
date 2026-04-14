using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Abstracciones.Modelos
{
    [Table("Factura")]
    public class FacturaDto
    {
        public int FacturaId { get; set; }

        [Required(ErrorMessage = "El número de factura es requerido.")]
        public int NumeroFactura { get; set; }

        [Required(ErrorMessage = "El pedido de venta es requerido.")]
        public int PedidoVentaId { get; set; }

        [Required(ErrorMessage = "La fecha de emisión es requerida.")]
        [DataType(DataType.Date)]
        public DateTime FechaEmision { get; set; }

        [Required(ErrorMessage = "El subtotal es requerido.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        [Required(ErrorMessage = "El impuesto es requerido.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Impuesto { get; set; }

        [Required(ErrorMessage = "El total es requerido.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        [Required(ErrorMessage = "El estado es requerido.")]
        [StringLength(20, ErrorMessage = "El estado no puede exceder 20 caracteres.")]
        public string Estado { get; set; } = default!;
    }
}