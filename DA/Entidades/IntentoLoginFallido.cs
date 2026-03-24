using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DA.Entidades
{
    public class IntentoLoginFallido
    {
        public int Id { get; set; }
        public string? EmailIngresado { get; set; }
        public DateTime FechaIntento { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? Motivo { get; set; }
    }
}
