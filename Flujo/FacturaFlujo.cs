using Abstracciones.Interfaces.DA;
using Abstracciones.Interfaces.Flujo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flujo
{
    public class FacturaFlujo : IFacturaFlujo
    {
        private readonly IFacturaDA _facturaDA;

        public FacturaFlujo(IFacturaDA facturaDA)
        {
            _facturaDA = facturaDA;
        }

        public async Task<int> CrearDesdePedidoAsync(int pedidoVentaId)
        {
            if (pedidoVentaId <= 0)
                throw new Exception("PedidoVentaId inválido.");

            var facturaId = await _facturaDA.CrearDesdePedidoAsync(pedidoVentaId);

            if (facturaId <= 0)
                throw new Exception("No se pudo generar la factura.");

            return facturaId;
        }
    }
}