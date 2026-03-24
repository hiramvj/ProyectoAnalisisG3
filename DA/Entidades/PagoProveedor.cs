using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DA.Entidades
{
    [Table("PagoProveedor")]
    public class PagoProveedor
    {
        [Key]
        public int PagoProveedorId { get; set; }

        public int CuentaPorPagarId { get; set; }

        public DateTime FechaPago { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Monto { get; set; }

        public string MetodoPago { get; set; }
        public string TipoTransaccion { get; set; } // PAGO, ANTICIPO
        public string Estado { get; set; } // COMPLETADO, PROGRAMADO

        public string? Notas { get; set; }

        // Navigation
        [ForeignKey("CuentaPorPagarId")]
        public virtual CuentaPorPagar CuentaPorPagar { get; set; }
    }
}
