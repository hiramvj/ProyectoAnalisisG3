using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DA.Entidades
{
    [Table("ordencompradetalle")]
    public class OrdenCompraDetalle
    {
        [Key]
        [Column("ordencompradetalleid")]
        public int OrdenCompraDetalleId { get; set; }

        [Column("ordencompraid")]
        public int OrdenCompraId { get; set; }

        [Column("productoid")]
        public int ProductoId { get; set; }

        [Column("cantidad", TypeName = "decimal(18,2)")]
        public decimal Cantidad { get; set; }

        [Column("costounitario", TypeName = "decimal(18,2)")]
        public decimal CostoUnitario { get; set; }

        // Navegación
        [ForeignKey("OrdenCompraId")]
        public OrdenCompra? OrdenCompra { get; set; }
    }
}
