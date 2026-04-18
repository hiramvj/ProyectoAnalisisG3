using Abstracciones.Interfaces.DA;
using Abstracciones.Interfaces.Flujo;
using Abstracciones.Modelos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Flujo
{
    public class CuentasPorPagarFlujo : ICuentasPorPagarFlujo
    {
        private readonly ICuentasPorPagarDA _da;

        public CuentasPorPagarFlujo(ICuentasPorPagarDA da)
        {
            _da = da;
        }

        public async Task<IEnumerable<CuentaPorPagarDto>> ListarCuentasAsync()
        {
            return await _da.ListarCuentasAsync();
        }

        public async Task<CuentaPorPagarDto> ObtenerCuentaAsync(int cuentaPorPagarId)
        {
            return await _da.ObtenerCuentaAsync(cuentaPorPagarId);
        }

        public async Task<int> CrearCuentaAsync(CuentaPorPagarDto dto)
        {
            if (dto.MontoOriginal <= 0)
                throw new ArgumentException("El monto de la factura debe ser mayor a 0.");

            if (dto.FechaVencimiento < dto.FechaEmision)
                throw new ArgumentException("La fecha de vencimiento no puede ser anterior a la de emisión.");

            return await _da.CrearCuentaAsync(dto);
        }

        public async Task<IEnumerable<PagoProveedorDto>> ListarPagosPorCuentaAsync(int cuentaPorPagarId)
        {
            return await _da.ListarPagosPorCuentaAsync(cuentaPorPagarId);
        }

        public async Task<int> RegistrarPagoAsync(PagoProveedorDto dto)
        {
            if (dto.Monto <= 0)
                throw new ArgumentException("El monto debe ser mayor a 0.");

            var cuenta = await _da.ObtenerCuentaAsync(dto.CuentaPorPagarId);
            if (cuenta == null)
                throw new ArgumentException("La cuenta por pagar no existe.");

            if (cuenta.SaldoPendiente <= 0)
                throw new ArgumentException("Esta cuenta ya está pagada.");

            if (dto.Estado == "COMPLETADO" && dto.Monto > cuenta.SaldoPendiente)
                throw new ArgumentException($"El monto no puede ser mayor al saldo pendiente (₡{cuenta.SaldoPendiente:N2}).");

            return await _da.RegistrarPagoAsync(dto);
        }

        public async Task CompletarPagoProgramadoAsync(int pagoProveedorId)
        {
            await _da.ActualizarEstadoPagoAsync(pagoProveedorId, "COMPLETADO");
        }
        public async Task<IEnumerable<FacturaDto>> ListarFacturasAsync()
        {
            return await _da.ListarFacturasAsync();
        }
    }
}
