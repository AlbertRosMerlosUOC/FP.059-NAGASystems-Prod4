using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CapaModelo
{
    public class Reserva
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo DNI es obligatorio.")]
        public string? DNI { get; set; }
        [ForeignKey("DNI")]
        public Cliente? Cliente { get; set; }


        [Required(ErrorMessage = "El campo Habitación es obligatorio.")]
        public int HabitacionId { get; set; }
        [ForeignKey("HabitacionId")]
        public Habitacion? Habitacion { get; set; }



        [Required(ErrorMessage = "El campo Tipo de Alojamiento es obligatorio.")]
        public int TipoAlojamientoId { get; set; }
        [ForeignKey("TipoAlojamientoId")]
        public TipoAlojamiento? TipoAlojamiento { get; set; }

        [Required(ErrorMessage = "El campo Tipo de Temporada es obligatorio.")]
        public int TipoTemporadaId { get; set; }
        [ForeignKey("TipoTemporadaId")]
        public TipoTemporada? TipoTemporada { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria.")]
        public DateTime FechaFin { get; set; }

        public static ValidationResult ValidarFechas(DateTime fechaInicio, ValidationContext context)
        {
            var instancia = context.ObjectInstance as Reserva;
            if (instancia != null && instancia.FechaFin <= fechaInicio)
            {
                return new ValidationResult("La fecha de fin debe ser posterior a la fecha de inicio.");
            }
            return ValidationResult.Success;
        }

        // public string Referido { get; set; }



        [Required(ErrorMessage = "El campo Oferta es obligatorio.")]
        public int OfertaId { get; set; }
        [ForeignKey("OfertaId")]
        public Oferta? Oferta { get; set; }

        [Required]
        public decimal Factura { get; set; } = 0.0m;

        [Required]
        public int CheckIn { get; set; } = 0;

        [Required]
        public int Cancelado { get; set; } = 0;
    }
}
