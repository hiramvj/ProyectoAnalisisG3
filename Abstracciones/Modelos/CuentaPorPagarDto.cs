using System.ComponentModel.DataAnnotations;

namespace Abstracciones.Modelos
{
    public class CuentaPorPagarDto
    {
        public int CuentaPorPagarId { get; set; }
        public int ProveedorId { get; set; }
        public string ProveedorNombre { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El número de factura es obligatorio")]
        public string NumeroFactura { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de emisión es obligatoria")]
        public DateTime FechaEmision { get; set; }

        [Required(ErrorMessage = "La fecha de vencimiento es obligatoria")]
        public DateTime FechaVencimiento { get; set; }

        [Required(ErrorMessage = "El monto es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        public decimal MontoOriginal { get; set; }

        public decimal SaldoPendiente { get; set; }
        public string Estado { get; set; } = "PENDIENTE";
    }

    public class PagoProveedorDto
    {
        public int PagoProveedorId { get; set; }
        public int CuentaPorPagarId { get; set; }
        
        [Required(ErrorMessage = "La fecha de pago es obligatoria")]
        public DateTime FechaPago { get; set; }

        [Required(ErrorMessage = "El monto es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        public decimal Monto { get; set; }

        [Required(ErrorMessage = "El método de pago es obligatorio")]
        public string MetodoPago { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de transacción es obligatorio")]
        public string TipoTransaccion { get; set; } = "PAGO"; // PAGO, ANTICIPO

        [Required(ErrorMessage = "El estado es obligatorio")]
        public string Estado { get; set; } = "COMPLETADO"; // COMPLETADO, PROGRAMADO

        public string? Notas { get; set; }
    }
}
