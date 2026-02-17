using Abstracciones.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstracciones.Interfaces.Flujo
{
    public interface IClienteFlujo
    {
        Task<List<ClienteDto>> ObtenerTodosAsync(bool soloActivos);
        Task<ClienteDto?> ObtenerPorIdAsync(int clienteId);

        Task<int> AgregarAsync(ClienteDto cliente);
        Task<bool> EditarAsync(ClienteDto cliente);

        Task<bool> CambiarEstadoAsync(int clienteId, bool activo);
    }
}
