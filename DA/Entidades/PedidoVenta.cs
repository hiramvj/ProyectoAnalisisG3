using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DA.Entidades
{public class PedidoVenta
    {
        public int PedidoVentaId { get; set; }
        public int NumeroPedido { get; set; }
        public int ClienteId { get; set; }
        public string Estado { get; set; } = null!;
        public DateTime FechaPedido { get; set; }
        public string? Observaciones { get; set; }
        public int? MetodoPagoId { get; set; }

        public List<PedidoVentaDetalle> Detalles { get; set; } = new();
    }
}
