using System;
using System.Collections.Generic;

namespace Abstracciones.Modelos
{
    public class DevolucionListadoDto
    {
        public int DevolucionVentaId { get; set; }
        public int NumeroFactura { get; set; }
        public DateTime FechaDevolucion { get; set; }
        public string Tipo { get; set; } = "";
        public string Estado { get; set; } = "";
        public decimal MontoTotal { get; set; }
        public int NumeroNotaCredito { get; set; }
    }
}
