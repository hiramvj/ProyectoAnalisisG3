using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstracciones.Modelos
{
    public class PedidoVentaDetalleLineaDto
    {
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; } = "";
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal TotalLinea => Cantidad * PrecioUnitario;
    }

    public class PedidoVentaDetalleDto
    {
        public int PedidoVentaId { get; set; }
        public int NumeroPedido { get; set; }
        public DateTime FechaPedido { get; set; }
        public string Estado { get; set; } = "";
        public int ClienteId { get; set; }
        public string ClienteNombre { get; set; } = "";
        public int? MetodoPagoId { get; set; }
        public string MetodoPagoNombre { get; set; } = "-";
        public string? Observaciones { get; set; }

        public List<PedidoVentaDetalleLineaDto> Lineas { get; set; } = new();

        public decimal Subtotal => Lineas.Sum(x => x.TotalLinea);
        public decimal IVA => Math.Round(Subtotal * 0.13m, 2);
        public decimal Total => Subtotal + IVA;
    }

    // Para editar (solo lo editable)
    public class PedidoVentaEditarDto
    {
        public int PedidoVentaId { get; set; }
        public string Estado { get; set; } = "";
        public int? MetodoPagoId { get; set; }
        public string? Observaciones { get; set; }
    }
}