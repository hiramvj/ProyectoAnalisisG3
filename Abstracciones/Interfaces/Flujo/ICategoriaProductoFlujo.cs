using Abstracciones.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Abstracciones.Interfaces.Flujo
{
    public interface ICategoriaProductoFlujo
    {
        Task<List<CategoriaProductoDto>> ListarAsync();
        Task<CategoriaProductoDto?> ObtenerPorIdAsync(int id);
    }
}