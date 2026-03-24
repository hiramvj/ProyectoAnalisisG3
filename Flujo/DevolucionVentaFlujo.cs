using Abstracciones.Interfaces.DA;
using Abstracciones.Interfaces.Flujo;
using Abstracciones.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flujo
{
    public class DevolucionVentaFlujo : IDevolucionVentaFlujo
    {
        private readonly IDevolucionVentaDA _devolucionDA;

        public DevolucionVentaFlujo(IDevolucionVentaDA devolucionDA)
        {
            _devolucionDA = devolucionDA;
        }

        public async Task<int> ProcesarDevolucionAsync(int facturaId, List<DevolucionLineaDto> lineas, string motivo)
        {
            if (facturaId <= 0)
                throw new Exception("FacturaId inválido.");

            if (lineas == null || !lineas.Any(l => l.CantidadDevuelta > 0))
                throw new Exception("Debe indicar al menos un producto con cantidad a devolver.");

            if (string.IsNullOrWhiteSpace(motivo))
                throw new Exception("Debe indicar el motivo de la devolución.");

            return await _devolucionDA.ProcesarDevolucionAsync(facturaId, lineas, motivo);
        }

        public async Task<DevolucionResultadoDto?> ObtenerPorIdAsync(int devolucionVentaId)
        {
            if (devolucionVentaId <= 0)
                throw new Exception("DevolucionVentaId inválido.");

            return await _devolucionDA.ObtenerPorIdAsync(devolucionVentaId);
        }

        public async Task<List<DevolucionListadoDto>> ObtenerTodosAsync()
        {
            return await _devolucionDA.ObtenerTodosAsync();
        }

        public async Task<List<DevolucionLineaDto>> ObtenerLineasFacturaAsync(int facturaId)
        {
            if (facturaId <= 0)
                throw new Exception("FacturaId inválido.");

            return await _devolucionDA.ObtenerLineasFacturaAsync(facturaId);
        }
    }
}
