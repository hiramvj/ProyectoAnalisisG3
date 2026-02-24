using Abstracciones.Modelos;

namespace Abstracciones.Interfaces.Flujo
{
    public interface IFacturaFlujo
    {
        Task<List<FacturaDto>> ListarAsync(string? estado, DateTime? fechaEmision);
        Task<FacturaDto?> ObtenerPorIdAsync(int facturaId);
        Task<bool> CambiarEstadoAsync(int facturaId, string nuevoEstado);
        Task<int> CrearAsync(FacturaDto factura);
        Task<int> CrearDesdePedidoAsync(int pedidoVentaId);
    }
}