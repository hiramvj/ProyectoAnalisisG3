namespace DA.Entidades
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Factura")]
    public class Factura
    {
        [Key]
        public int FacturaId { get; set; }

        public int NumeroFactura { get; set; }

        public int PedidoVentaId { get; set; }

        [ForeignKey("PedidoVentaId")]
        public PedidoVenta? PedidoVenta { get; set; }

        public DateTime FechaEmision { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Impuesto { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        [MaxLength(50)]
        public string Estado { get; set; } = "Emitida";

        public ICollection<FacturaDetalle> Detalles { get; set; } = new List<FacturaDetalle>();
    }
}
