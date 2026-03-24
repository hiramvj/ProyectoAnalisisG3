using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstracciones.Modelos
{
    public class NotaContableDto
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public decimal Monto { get; set; }
        public string Detalle { get; set; }
        public string Motivo { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public int FacturaId { get; set; }
        public decimal MontoMaximo { get; set; }
    }
}
