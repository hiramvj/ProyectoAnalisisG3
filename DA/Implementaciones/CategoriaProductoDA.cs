using Abstracciones.Interfaces.DA;
using Abstracciones.Modelos;
using DA.Contexto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DA.Implementaciones
{
    public class CategoriaProductoDA : ICategoriaProductoDA
    {
        private readonly AppDbContext _context;

        public CategoriaProductoDA(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoriaProductoDto>> ListarAsync()
        {
            return await _context.CategoriasProducto
                .AsNoTracking()
                .OrderBy(c => c.Nombre)
                .ToListAsync();
        }

        public async Task<CategoriaProductoDto?> ObtenerPorIdAsync(int id)
        {
            return await _context.CategoriasProducto
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CategoriaProductoId == id);
        }
    }
}