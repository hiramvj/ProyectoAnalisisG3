using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DA.Entidades
{
    [Table("ordencompra")]
    public class OrdenCompra
    {
        [Key]
        [Column("ordencompraid")]
        public int OrdenCompraId { get; set; }

        [Column("bodegaid")]
        public int? BodegaId { get; set; } // Puede ser null si aún no se mapea estrictamente o no es obligatorio en UI.

        [Column("proveedorid")]
        public int ProveedorId { get; set; }

        [Column("numeroorden")]
        public string? NumeroOrden { get; set; } // O int dependiendo de la db, la DB dice varchar(50) por default o int. 

        [Column("fechaemision")]
        public DateTime FechaEmision { get; set; }

        [Column("estado")]
        public string Estado { get; set; } = "CREADA"; // CREADA, APROBADA, RECIBIDA, CANCELADA

        [Column("observaciones")]
        public string? Observaciones { get; set; }

        [Column("subtotal", TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        [Column("impuesto", TypeName = "decimal(18,2)")]
        public decimal Impuesto { get; set; }

        [Column("total", TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        // Navegación
        public ICollection<OrdenCompraDetalle> Detalles { get; set; } = new List<OrdenCompraDetalle>();
    }
}
