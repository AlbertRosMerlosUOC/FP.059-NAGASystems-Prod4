using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CapaModelo
{
    public class Habitacion
    {
        [Key]
        public int Numero { get; set; }

        public int Estado { get; set; }

        public int TipoHabitacionId { get; set; }
        [ForeignKey("TipoHabitacionId")]
        public TipoHabitacion? TipoHabitacion { get; set; }

        public List<Reserva> Reservas { get; set; }
        public Habitacion()
        {
            Reservas = new List<Reserva>();
        }
    }


}
