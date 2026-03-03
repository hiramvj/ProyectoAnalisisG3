using Abstracciones.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstracciones.Interfaces.DA
{
    public interface IPedidoVentaDA
    {
        Task<int> CrearPedidoAsync(int clienteId, string? observaciones, int? metodoPagoId);
        Task AgregarLineaAsync(int pedidoVentaId, int productoId, int cantidad, decimal precioUnitario);
        Task<List<PedidoVentaListadoDto>> ListarAsync(string? q,DateTime? desde,DateTime? hasta,int? clienteId,string? estado,int? metodoPagoId);
        Task<PedidoVentaDetalleDto?> ObtenerDetalleAsync(int pedidoVentaId);
        Task<int> ActualizarEncabezadoAsync(PedidoVentaEditarDto dto);
    }
}
