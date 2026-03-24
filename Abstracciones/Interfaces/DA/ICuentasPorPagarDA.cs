using Abstracciones.Modelos;

namespace Abstracciones.Interfaces.DA
{
    public interface ICuentasPorPagarDA
    {
        Task<IEnumerable<CuentaPorPagarDto>> ListarCuentasAsync();
        Task<CuentaPorPagarDto> ObtenerCuentaAsync(int cuentaPorPagarId);
        Task<int> CrearCuentaAsync(CuentaPorPagarDto dto);
        
        Task<IEnumerable<PagoProveedorDto>> ListarPagosPorCuentaAsync(int cuentaPorPagarId);
        Task<int> RegistrarPagoAsync(PagoProveedorDto dto);
        Task ActualizarEstadoPagoAsync(int pagoProveedorId, string nuevoEstado);
    }
}
