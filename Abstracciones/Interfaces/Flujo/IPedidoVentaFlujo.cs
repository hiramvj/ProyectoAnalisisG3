using Abstracciones.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstracciones.Interfaces.Flujo
{
    public interface IPedidoVentaFlujo
    {
        Task<int> CrearPedidoAsync(PedidoVentaCrearDto dto);
        Task<List<PedidoVentaListadoDto>> ListarAsync(string? q,DateTime? desde,DateTime? hasta,int? clienteId,string? estado,int? metodoPagoId);
        Task<PedidoVentaDetalleDto?> ObtenerDetalleAsync(int pedidoVentaId);
        Task<bool> EditarEncabezadoAsync(PedidoVentaEditarDto dto);
        Task<List<PedidoVentaListadoDto>> ObtenerHistorialClienteAsync(int clienteId);
        Task<List<ProductoMasVendidoDto>> ObtenerProductosMasVendidosAsync();
    }
}
