using Abstracciones.Interfaces.DA;
using Abstracciones.Modelos;
using DA.Contexto;
using DA.Entidades;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DA.Implementaciones
{
    public class IntentoLoginFallidoDA : IIntentoLoginFallidoDA
    {
        private readonly AppDbContext _context;

        public IntentoLoginFallidoDA(AppDbContext context)
        {
            _context = context;
        }

        public async Task RegistrarAsync(IntentoLoginFallidoDto intento)
        {
            var entidad = new IntentoLoginFallido
            {
                EmailIngresado = intento.EmailIngresado,
                FechaIntento = intento.FechaIntento,
                IpAddress = intento.IpAddress,
                UserAgent = intento.UserAgent,
                Motivo = intento.Motivo
            };

            _context.IntentosLoginFallidos.Add(entidad);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<IntentoLoginFallidoDto>> ObtenerTodosAsync()
        {
            return await _context.IntentosLoginFallidos
                .OrderByDescending(x => x.FechaIntento)
                .Select(x => new IntentoLoginFallidoDto
                {
                    Id = x.Id,
                    EmailIngresado = x.EmailIngresado,
                    FechaIntento = x.FechaIntento,
                    IpAddress = x.IpAddress,
                    UserAgent = x.UserAgent,
                    Motivo = x.Motivo
                })
                .ToListAsync();
        }
    }
}