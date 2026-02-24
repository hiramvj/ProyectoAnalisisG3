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
    public class CategoriaProductoFlujo : ICategoriaProductoFlujo
    {
        private readonly ICategoriaProductoDA _categoriaDA;

        public CategoriaProductoFlujo(ICategoriaProductoDA categoriaDA)
        {
            _categoriaDA = categoriaDA;
        }

        public Task<List<CategoriaProductoDto>> ListarAsync()
            => _categoriaDA.ListarAsync();

        public Task<CategoriaProductoDto?> ObtenerPorIdAsync(int id)
            => _categoriaDA.ObtenerPorIdAsync(id);
    }
}