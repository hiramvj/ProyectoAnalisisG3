using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstracciones.Modelos
{
    public class PedidoVentaDto
    {
        public int PedidoVentaId { get; set; }
        public int NumeroPedido { get; set; }
        public int ClienteId { get; set; }
        public string Estado { get; set; } = "";
        public DateTime FechaPedido { get; set; }
        public string? Observaciones { get; set; }
        public int? MetodoPagoId { get; set; }
    }
}
