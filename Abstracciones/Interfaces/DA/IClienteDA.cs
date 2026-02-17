using Abstracciones.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstracciones.Interfaces.DA
{
    public interface IClienteDA
    {
        Task<List<ClienteDto>> ListarPorEstadoAsync(bool activo);
        Task<ClienteDto?> ObtenerPorIdAsync(int clienteId);

        Task<int> InsertarAsync(ClienteDto cliente);
        Task<int> ActualizarAsync(ClienteDto cliente);

        Task<int> CambiarEstadoAsync(int clienteId, bool activo);
    }
}
