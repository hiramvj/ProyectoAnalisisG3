using Abstracciones.Modelos;
using System.Threading.Tasks;

namespace Abstracciones.Interfaces.Flujo
{
    public interface IOrdenCompraFlujo
    {
        Task<int> CrearOrdenAsync(OrdenCompraCrearDto dto);
    }
}
