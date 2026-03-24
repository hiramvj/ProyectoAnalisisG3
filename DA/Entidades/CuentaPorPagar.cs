using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DA.Entidades
{
    [Table("CuentaPorPagar")]
    public class CuentaPorPagar
    {
        [Key]
        public int CuentaPorPagarId { get; set; }

        public int ProveedorId { get; set; }
        public string NumeroFactura { get; set; }

        public DateTime FechaEmision { get; set; }
        public DateTime FechaVencimiento { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal MontoOriginal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SaldoPendiente { get; set; }

        public string Estado { get; set; } = "PENDIENTE";

        // Navigation properties
        // Si tuvieras entidad de ProveedorReal, podrías enlazarla.
        // public virtual Proveedor Proveedor { get; set; }
        public virtual ICollection<PagoProveedor> Pagos { get; set; } = new List<PagoProveedor>();
    }
}
