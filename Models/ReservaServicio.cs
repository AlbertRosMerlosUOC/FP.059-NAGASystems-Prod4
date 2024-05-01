using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CapaModelo
{
    public class ReservaServicio
    {
        [Key]
        public int Id { get; set; }
        public int Reserva { get; set; }
        public int Servicio { get; set; }
        public DateTime Fecha { get; set; }
        public int Cantidad { get; set; }
    }
}
