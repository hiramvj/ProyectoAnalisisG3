using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstracciones.Interfaces.Flujo
{
    public interface IFacturaFlujo
    {
        Task<int> CrearDesdePedidoAsync(int pedidoVentaId);
    }
}
