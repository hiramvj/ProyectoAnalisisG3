using Abstracciones.Modelos;

namespace Abstracciones.Interfaces.Flujo
{
    public interface ICuentasPorPagarFlujo
    {
        Task<IEnumerable<CuentaPorPagarDto>> ListarCuentasAsync();
        Task<CuentaPorPagarDto> ObtenerCuentaAsync(int cuentaPorPagarId);
        Task<int> CrearCuentaAsync(CuentaPorPagarDto dto);
        
        Task<IEnumerable<PagoProveedorDto>> ListarPagosPorCuentaAsync(int cuentaPorPagarId);
        Task<int> RegistrarPagoAsync(PagoProveedorDto dto);
        Task CompletarPagoProgramadoAsync(int pagoProveedorId);
        Task<IEnumerable<FacturaDto>> ListarFacturasAsync();
    }
}
