using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstracciones.Modelos
{
    public class AsistenciaDto
    {
        public long AsistenciaId { get; set; }

        [Required]
        public int EmpleadoId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        public TimeSpan? HoraEntrada { get; set; }

        public TimeSpan? HoraSalida { get; set; }

        public string? NombreEmpleado { get; set; }
        public string Tipo { get; set; } = "ASISTENCIA";
    }
}