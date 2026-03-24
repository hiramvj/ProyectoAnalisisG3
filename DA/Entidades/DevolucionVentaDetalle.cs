namespace DA.Entidades
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("DevolucionVentaDetalle")]
    public class DevolucionVentaDetalle
    {
        [Key]
        public int DevolucionVentaDetalleId { get; set; }

        public int DevolucionVentaId { get; set; }

        [ForeignKey("DevolucionVentaId")]
        public DevolucionVenta? DevolucionVenta { get; set; }

        public int ProductoId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CantidadDevuelta { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioUnitario { get; set; }
    }
}
