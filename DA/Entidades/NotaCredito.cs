namespace DA.Entidades
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("NotaCredito")]
    public class NotaCredito
    {
        [Key]
        public int NotaCreditoId { get; set; }

        public int DevolucionVentaId { get; set; }

        [ForeignKey("DevolucionVentaId")]
        public DevolucionVenta? DevolucionVenta { get; set; }

        public int NumeroNotaCredito { get; set; }

        public DateTime FechaEmision { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Impuesto { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        [MaxLength(50)]
        public string Estado { get; set; } = "EMITIDA";
    }
}
