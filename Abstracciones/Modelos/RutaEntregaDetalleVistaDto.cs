using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstracciones.Modelos
{
    public class RutaEntregaDetalleVistaDto
    {
        public int RutaDetalleId { get; set; }
        public int RutaId { get; set; }
        public int PedidoVentaId { get; set; }
        public int OrdenParada { get; set; }
        public string EstadoParada { get; set; } = string.Empty;

        // Datos adicionales para vista
        public string? Cliente { get; set; }
        public string? DireccionEntrega { get; set; }
    }
}
