using Abstracciones.Interfaces.DA;
using Abstracciones.Interfaces.Flujo;
using Abstracciones.Modelos;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Flujo
{
    public class OrdenCompraFlujo : IOrdenCompraFlujo
    {
        private readonly IOrdenCompraDA _da;

        public OrdenCompraFlujo(IOrdenCompraDA da)
        {
            _da = da;
        }

        public async Task<int> CrearOrdenAsync(OrdenCompraCrearDto dto)
        {
            // Validaciones de negocio
            if (dto.ProveedorId <= 0)
                throw new Exception("El proveedor es requerido.");

            if (dto.Lineas == null || !dto.Lineas.Any())
                throw new Exception("La orden debe tener al menos una línea.");

            foreach (var l in dto.Lineas)
            {
                if (l.Cantidad <= 0)
                    throw new Exception("La cantidad debe ser mayor a cero.");
                if (l.CostoUnitario < 0)
                    throw new Exception("El costo unitario no puede ser negativo.");
            }

            // Llamada a la capa de acceso a datos
            return await _da.CrearOrdenAsync(dto);
        }
    }
}
