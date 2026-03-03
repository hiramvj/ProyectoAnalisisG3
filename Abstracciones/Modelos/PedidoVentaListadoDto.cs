using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstracciones.Modelos
{
    public class PedidoVentaListadoDto
    {
        public int PedidoVentaId { get; set; }
        public int NumeroPedido { get; set; }
        public DateTime FechaPedido { get; set; }
        public string Estado { get; set; } = "";
        public string ClienteNombre { get; set; } = "";
        public string? MetodoPagoNombre { get; set; }
        public decimal Total { get; set; }
    }
}
