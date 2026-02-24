using Abstracciones.Interfaces.DA;
using Abstracciones.Interfaces.Flujo;
using Abstracciones.Modelos;

namespace Flujo
{
    public class FacturaFlujo : IFacturaFlujo
    {

        private readonly IFacturaDA _facturaDA;

        public FacturaFlujo(IFacturaDA facturaDA)
        {
            _facturaDA = facturaDA;
        }


        public Task<List<FacturaDto>> ListarAsync(string? estado, DateTime? fechaEmision)
            => _facturaDA.ListarAsync(estado, fechaEmision);

        public Task<FacturaDto?> ObtenerPorIdAsync(int facturaId)
            => _facturaDA.ObtenerPorIdAsync(facturaId);

        public async Task<int> CrearAsync(FacturaDto factura)
        {
            factura.FechaEmision = DateTime.Now;
            factura.Estado = "Pendiente Envío";

            return await _facturaDA.InsertarAsync(factura);
        }

        public async Task<bool> CambiarEstadoAsync(int facturaId, string nuevoEstado)
        {
            var filas = await _facturaDA.CambiarEstadoAsync(facturaId, nuevoEstado);
            return filas > 0;
        }

        public async Task<int> CrearDesdePedidoAsync(int pedidoVentaId)
        {
            throw new NotImplementedException("CrearDesdePedidoAsync no está implementado.");
        }
    }
}