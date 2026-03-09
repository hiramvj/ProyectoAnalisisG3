using System;
using System.Collections.Generic;

namespace Abstracciones.Modelos
{
    public class OrdenCompraDto
    {
        public int OrdenCompraId { get; set; }
        public int ProveedorId { get; set; }
        public int? BodegaId { get; set; }
        public string? NumeroOrden { get; set; }
        public DateTime FechaEmision { get; set; }
        public string Estado { get; set; } = "CREADA";
        public string? Observaciones { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Impuesto { get; set; }
        public decimal Total { get; set; }
    }
}
