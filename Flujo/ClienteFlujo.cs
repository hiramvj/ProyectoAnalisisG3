using Abstracciones.Interfaces.DA;
using Abstracciones.Interfaces.Flujo;
using Abstracciones.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flujo
{
    public class ClienteFlujo : IClienteFlujo
    {
        private readonly IClienteDA _clienteDA;

        public ClienteFlujo(IClienteDA clienteDA)
        {
            _clienteDA = clienteDA;
        }

        public async Task<List<ClienteDto>> ObtenerTodosAsync(bool soloActivos)
        {
            return await _clienteDA.ListarPorEstadoAsync(soloActivos);
        }

        public Task<ClienteDto?> ObtenerPorIdAsync(int clienteId)
        {
            return _clienteDA.ObtenerPorIdAsync(clienteId);
        }

        public async Task<int> AgregarAsync(ClienteDto cliente)
        {
            ValidarCliente(cliente, esNuevo: true);
            return await _clienteDA.InsertarAsync(cliente);
        }

        public async Task<bool> EditarAsync(ClienteDto cliente)
        {
            if (cliente.ClienteId <= 0)
                throw new Exception("ClienteId inválido.");

            ValidarCliente(cliente, esNuevo: false);
            var filas = await _clienteDA.ActualizarAsync(cliente);

            return filas > 0;
        }

        public async Task<bool> CambiarEstadoAsync(int clienteId, bool activo)
        {
            if (clienteId <= 0) throw new Exception("ClienteId inválido.");

            var filas = await _clienteDA.CambiarEstadoAsync(clienteId, activo);
            return filas > 0;
        }

        private static void ValidarCliente(ClienteDto cliente, bool esNuevo)
        {
            if (string.IsNullOrWhiteSpace(cliente.NombreCompleto))
                throw new Exception("Nombre completo es requerido.");

            if (cliente.NombreCompleto.Length > 150)
                throw new Exception("Nombre completo supera el máximo permitido (150).");

            if (!string.IsNullOrEmpty(cliente.Identificacion) && cliente.Identificacion.Length > 30)
                throw new Exception("Identificación supera el máximo permitido (30).");

            if (!string.IsNullOrEmpty(cliente.Correo) && cliente.Correo.Length > 120)
                throw new Exception("Correo supera el máximo permitido (120).");

            if (!string.IsNullOrEmpty(cliente.Telefono) && cliente.Telefono.Length > 20)
                throw new Exception("Teléfono supera el máximo permitido (20).");

            if (!string.IsNullOrEmpty(cliente.Direccion) && cliente.Direccion.Length > 250)
                throw new Exception("Dirección supera el máximo permitido (250).");
        }
    }
}
