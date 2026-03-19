using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstracciones.Modelos
{
    public class ReporteVentaDto
    {
        public int PedidoVentaId { get; set; }
        public int NumeroPedido { get; set; }
        public DateTime FechaPedido { get; set; }
        public string ClienteNombre { get; set; } = string.Empty;
        public string ProductoNombre { get; set; } = string.Empty;
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal TotalLinea { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}
