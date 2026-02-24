using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstracciones.Modelos
{
    public class PedidoVentaCrearDto
    {
        public int ClienteId { get; set; }
        public string? Observaciones { get; set; }
        public int? MetodoPagoId { get; set; }
        public List<PedidoVentaLineaDto> Lineas { get; set; } = new();
    }

    public class PedidoVentaLineaDto
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
    }
}