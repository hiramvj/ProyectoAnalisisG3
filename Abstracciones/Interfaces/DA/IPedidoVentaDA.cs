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
    }
}
