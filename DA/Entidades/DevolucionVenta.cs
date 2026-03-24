namespace DA.Entidades
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("DevolucionVenta")]
    public class DevolucionVenta
    {
        [Key]
        public int DevolucionVentaId { get; set; }

        public int FacturaId { get; set; }

        [ForeignKey("FacturaId")]
        public Factura? Factura { get; set; }

        public DateTime FechaDevolucion { get; set; }

        [MaxLength(500)]
        public string Motivo { get; set; } = "";

        [MaxLength(20)]
        public string Tipo { get; set; } = "TOTAL"; // TOTAL o PARCIAL

        [MaxLength(50)]
        public string Estado { get; set; } = "PROCESADA";

        [Column(TypeName = "decimal(18,2)")]
        public decimal MontoTotal { get; set; }

        public ICollection<DevolucionVentaDetalle> Detalles { get; set; } = new List<DevolucionVentaDetalle>();

        public NotaCredito? NotaCredito { get; set; }
    }
}
