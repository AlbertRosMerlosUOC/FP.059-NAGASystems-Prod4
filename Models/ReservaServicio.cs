using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CapaModelo
{
    public class ReservaServicio
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Reserva")]
        public int ReservaId { get; set; }
        public Reserva Reserva { get; set; }

        [ForeignKey("Servicio")]
        public int ServicioId { get; set; }
        public Servicio Servicio { get; set; }

        public DateTime Fecha { get; set; }

        [Required]
        public int Cantidad { get; set; }
    }
}