using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaModelo
{
    public class TipoTemporada
    {
        [Key]
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public double Coeficiente { get; set; }
        public List<Reserva> Reservas { get; set; }
        public TipoTemporada()
        {
            Reservas = new List<Reserva>();
        }
    }
}
