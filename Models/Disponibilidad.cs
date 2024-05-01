using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaModelo
{
    public class Disponibilidad
    {
        [Key]
        public int Id { get; set; }
        public int Habitacion { get; set; }
        public DateTime Fecha { get; set; }
    }
}
