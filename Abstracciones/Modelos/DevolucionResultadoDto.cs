using System;
using System.Collections.Generic;

namespace Abstracciones.Modelos
{
    public class DevolucionResultadoDto
    {
        public int DevolucionVentaId { get; set; }
        public int FacturaId { get; set; }
        public int NumeroFactura { get; set; }
        public DateTime FechaDevolucion { get; set; }
        public string Motivo { get; set; } = "";
        public string Tipo { get; set; } = "";
        public string Estado { get; set; } = "";
        public decimal MontoTotal { get; set; }

        // Nota de crédito
        public int NotaCreditoId { get; set; }
        public int NumeroNotaCredito { get; set; }
        public decimal NCSubtotal { get; set; }
        public decimal NCImpuesto { get; set; }
        public decimal NCTotal { get; set; }

        public List<DevolucionLineaDto> Lineas { get; set; } = new();
    }
}
