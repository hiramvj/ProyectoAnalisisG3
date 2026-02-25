using Abstracciones.Modelos;
using System.Threading.Tasks;

namespace Abstracciones.Interfaces.DA
{
    public interface IOrdenCompraDA
    {
        Task<int> CrearOrdenAsync(OrdenCompraCrearDto dto);
    }
}
