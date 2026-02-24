using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DA.Entidades
{
    public class PedidoVentaDetalle
    {
        public int PedidoVentaDetalleId { get; set; }
        public int PedidoVentaId { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }

        public PedidoVenta PedidoVenta { get; set; } = null!;
    }
}
