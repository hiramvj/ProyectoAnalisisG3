using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DA.Entidades
{
    [Table("asistencia")]
    public class Asistencia
    {
        [Key]
        [Column("asistenciaid")]
        public long AsistenciaId { get; set; }

        [Column("empleadoid")]
        public int EmpleadoId { get; set; }

        [Column("fecha", TypeName = "date")]
        public DateTime Fecha { get; set; }

        [Column("horaentrada")]
        public TimeSpan? HoraEntrada { get; set; }

        [Column("horasalida")]
        public TimeSpan? HoraSalida { get; set; }
        [Column("tipo")]
        public string Tipo { get; set; } = "ASISTENCIA";
    }
}