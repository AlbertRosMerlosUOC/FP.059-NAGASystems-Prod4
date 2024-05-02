using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CapaModelo;
using FP._059_NAGASystems_Prod4.Data;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;

namespace FP._059_NAGASystems_Prod4
{
    public class ReservaController : Controller
    {
        private readonly FP_059_NAGASystems_Prod4Context _context;
        private readonly ILogger<ReservaController> _logger;

        // Constructor que utiliza el contexto y logger
        public ReservaController(FP_059_NAGASystems_Prod4Context context, ILogger<ReservaController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Reserva
        public async Task<IActionResult> Index()
        {
            return View(await _context.Reserva.ToListAsync());
        }

        // GET: Reserva/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reserva
                .Include(r => r.Habitacion)
                    .ThenInclude(h => h.TipoHabitacion)
                .Include(r => r.TipoAlojamiento)
                .Include(r => r.TipoTemporada)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }


        // GET: Reserva/CheckOut/5
        [HttpGet]
        public async Task<IActionResult> CheckOut(int id)
        {
            var reserva = await _context.Reserva
                .Include(r => r.Habitacion)
                    .ThenInclude(h => h.TipoHabitacion)
                .Include(r => r.TipoAlojamiento)
                .Include(r => r.TipoTemporada)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reserva == null || reserva.Cancelado == 1)
            {
                return NotFound("Reserva no encontrada o ya está cancelada.");
            }
            var servicios = await _context.Servicio.ToListAsync();
            ViewBag.ServiciosDisponibles = servicios;
            ViewBag.NoServices = !servicios.Any(); // Flag para verificar si hay servicios

            // Detalles de cálculo
            double precioHabitacion = reserva.Habitacion.TipoHabitacion.Precio;
            double precioTipoAlojamiento = reserva.TipoAlojamiento.Precio;
            double coeficiente = reserva.TipoTemporada.Coeficiente / 100.0; // Convertir a decimal
            int totalDias = (reserva.FechaFin - reserva.FechaInicio).Days;
            if (totalDias == 0) totalDias = 1;

            double costoBase = (precioHabitacion + precioTipoAlojamiento) * coeficiente;
            double totalCosto = costoBase * totalDias;

            // Pasar a la vista
            ViewBag.PrecioHabitacion = precioHabitacion;
            ViewBag.PrecioTipoAlojamiento = precioTipoAlojamiento;
            ViewBag.Coeficiente = reserva.TipoTemporada.Coeficiente; // Mostrar como porcentaje
            ViewBag.TotalDias = totalDias;
            ViewBag.TotalCosto = totalCosto;

            return View(reserva);
        }

        // POST: Reserva/ConfirmCheckOut/5
        [HttpPost, ActionName("ConfirmCheckOut")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmCheckOut(int id, decimal totalFactura)
        {
            var reserva = await _context.Reserva
                .Include(r => r.Habitacion)
                    .ThenInclude(h => h.TipoHabitacion)
                .Include(r => r.TipoAlojamiento)
                .Include(r => r.TipoTemporada)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reserva == null)
            {
                return NotFound();
            }

            reserva.Factura = totalFactura;

            // Guarda los cambios en la base de datos.
            _context.Update(reserva);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = reserva.Id });
        }


        // GET: Reserva/Create
        [HttpGet]
        public IActionResult Create(int? habitacionId, DateTime? fechaInicio, DateTime? fechaFin)
        {
            PopulateSelectLists();  // Configura los SelectList

            // Preconfigura los datos si vienen de la página de disponibilidad de habitaciones
            var reserva = new Reserva
            {
                HabitacionId = habitacionId ?? 0,
                FechaInicio = fechaInicio ?? DateTime.Today,
                FechaFin = fechaFin ?? DateTime.Today.AddDays(1),
                Factura = 0,
                Cancelado = 0,
                CheckIn = 0
            };

            // Configura el ViewBag para los DNI
            var clientes = _context.Cliente.ToList();
            var dniItems = clientes.Select(c => new SelectListItem
            {
                Value = c.DNI.ToString(),
                Text = $"{c.DNI} - {c.Nombre}"
            }).ToList();
            dniItems.Insert(0, new SelectListItem { Value = null, Text = "Seleccione un DNI", Selected = true });
            ViewBag.DNI = new SelectList(dniItems, "Value", "Text");

            return View(reserva);
        }

        // POST: Reserva/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DNI,HabitacionId,TipoAlojamientoId,TipoTemporadaId,FechaInicio,FechaFin,Factura,CheckIn,Cancelado,OfertaId")] Reserva reserva)
        {
            if (ModelState.IsValid)
            {
                // Comprueba si la habitación está disponible en las fechas seleccionadas
                bool isRoomAvailable = !_context.Reserva.Any(r => r.HabitacionId == reserva.HabitacionId &&
                                                                  r.FechaFin > reserva.FechaInicio &&
                                                                  r.FechaInicio < reserva.FechaFin &&
                                                                  r.Cancelado == 0);
                if (!isRoomAvailable)
                {
                    ModelState.AddModelError("", "La habitación seleccionada no está disponible en las fechas elegidas. Por favor, elige otra habitación o cambia las fechas.");
                }
                else
                {
                    // Buscar la habitación seleccionada
                    var habitacion = await _context.Habitacion.FirstOrDefaultAsync(h => h.Numero == reserva.HabitacionId);
                    if (habitacion == null)
                    {
                        ModelState.AddModelError("", "La habitación seleccionada no existe.");
                    }
                    else
                    {
                        // Actualizar el estado de la habitación seleccionada
                        habitacion.Estado = 1;  // Marcar la habitación como no disponible
                        _context.Update(habitacion);

                        // Agregar la reserva a la base de datos
                        _context.Add(reserva);
                        await _context.SaveChangesAsync();

                        return RedirectToAction(nameof(Index));
                    }
                }
            }

            // Preparar las listas desplegables y los datos necesarios para volver a la vista en caso de error
            PopulateSelectLists(reserva.TipoAlojamientoId, reserva.TipoTemporadaId, reserva.OfertaId, reserva.HabitacionId);
            PrepareDniSelectList();
            return View(reserva);
        }

        [HttpGet]
        public IActionResult CrearReservaDesdeDisponibilidad(int numeroHabitacion, string fechaInicioStr, string fechaFinStr)
        {
            if (string.IsNullOrEmpty(fechaInicioStr) || string.IsNullOrEmpty(fechaFinStr))
            {
                ViewBag.Error = "Debe especificar ambas fechas.";
                return View("Error");
            }

            if (!DateTime.TryParse(fechaInicioStr, out DateTime fechaInicio) || !DateTime.TryParse(fechaFinStr, out DateTime fechaFin))
            {
                ViewBag.Error = "Formato de fecha incorrecto.";
                return View("Error");
            }

            return RedirectToAction("Create", "Reserva", new { habitacionId = numeroHabitacion, fechaInicio = fechaInicio, fechaFin = fechaFin });
        }

        private void PrepareDniSelectList()
        {
            var clientes = _context.Cliente.ToList();
            var dniItems = clientes.Select(c => new SelectListItem
            {
                Value = c.DNI.ToString(),
                Text = $"{c.DNI} - {c.Nombre}"
            }).ToList();

            dniItems.Insert(0, new SelectListItem { Value = null, Text = "Seleccione un DNI", Selected = true });
            ViewBag.DNI = new SelectList(dniItems, "Value", "Text");
        }




        // GET: Reserva/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reserva.FindAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }

            // Obtener una lista de todos los clientes
            var clientes = _context.Cliente.ToList();

            // Crear una lista de SelectListItem donde cada elemento tiene el DNI como valor y el DNI y el nombre como texto
            var dniItems = clientes.Select(c => new SelectListItem
            {
                Value = c.DNI.ToString(),
                Text = $"{c.DNI} - {c.Nombre}"
            }).ToList();

            // Agregar una opción "Ninguno" al principio de la lista
            dniItems.Insert(0, new SelectListItem { Value = null, Text = "Ninguno", Selected = true });

            ViewBag.DNI = new SelectList(dniItems, "Value", "Text");

            PopulateSelectLists(reserva.TipoAlojamientoId, reserva.TipoTemporadaId, reserva.OfertaId, reserva.HabitacionId);
            return View(reserva);
        }


        // POST: Reserva/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DNI,HabitacionId,TipoAlojamientoId,TipoTemporadaId,FechaInicio,FechaFin,Referido,CheckIn,Cancelado,OfertaId")] Reserva reserva)
        {
            if (id != reserva.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reserva);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservaExists(reserva.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            PopulateSelectLists(reserva.TipoAlojamientoId, reserva.TipoTemporadaId, reserva.OfertaId, reserva.HabitacionId);
            return View(reserva);
        }

        private List<Habitacion> GetHabitacionesDisponibles()
        {
            // Obtener todas las habitaciones de la tabla Habitacion
            var habitacionesDisponibles = _context.Habitacion.ToList();

            return habitacionesDisponibles;
        }


        private void PopulateSelectLists(int tipoAlojamientoId = 1, int tipoTemporadaId = 1, int ofertaId = 1, int numero = 1)
        {
            ViewBag.TipoAlojamientoId = new SelectList(_context.TipoAlojamiento, "Id", "Descripcion", tipoAlojamientoId);
            ViewBag.TipoTemporadaId = new SelectList(_context.TipoTemporada, "Id", "Descripcion", tipoTemporadaId);
            ViewBag.OfertaId = new SelectList(_context.Oferta, "Id", "Descripcion", ofertaId);

            // Obtener las habitaciones disponibles
            var habitacionesDisponibles = GetHabitacionesDisponibles();

            ViewBag.HabitacionId = new SelectList(habitacionesDisponibles, "Numero", "Numero", numero);
        }


        // GET: Reserva/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reserva
                .Include(r => r.Habitacion)
                    .ThenInclude(h => h.TipoHabitacion)
                .Include(r => r.TipoAlojamiento)
                .Include(r => r.TipoTemporada)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }


        // POST: Reserva/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reserva = await _context.Reserva.FindAsync(id);
            _context.Reserva.Remove(reserva);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool ReservaExists(int id)
        {
            return _context.Reserva.Any(e => e.Id == id);
        }
        // POST: Reserva/CheckIn/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckIn(int id)
        {
            var reserva = await _context.Reserva.FindAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }

            if (reserva.CheckIn == 0)
            {
                reserva.CheckIn = 1;
                _context.Update(reserva);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> ListaPendientesCheckIns()
        {
            var currentDate = DateTime.Now;
            var reservasPendientes = await _context.Reserva
                .Where(r => r.FechaInicio < currentDate && r.CheckIn == 0 && r.Cancelado != 1)
                .ToListAsync();

            return View(reservasPendientes);
        }

        // POST: Reserva/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var reserva = await _context.Reserva.FindAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }

            reserva.Cancelado = 1;  // Anula la reserva
            _context.Update(reserva);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ListaPendientesCheckIns));
        }
        // POST: Reserva/CancelReservation/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelReservation(int id)
        {
            var reserva = await _context.Reserva.FindAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }

            reserva.Cancelado = 1;  // Marca la reserva como cancelada
            _context.Update(reserva);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult ImportarXmlReserva()
        {
            // Este es para la solicitud GET para mostrar el formulario
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportarXmlReserva(IFormFile archivoXml)
        {
            if (archivoXml == null || archivoXml.Length == 0)
            {
                _logger.LogWarning("Archivo XML no proporcionado o está vacío.");
                ModelState.AddModelError("", "Archivo XML no proporcionado o está vacío.");
                return View();
            }

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Reserva>), new XmlRootAttribute("Reservas"));
                List<Reserva> reservasImportadas;

                using (var stream = archivoXml.OpenReadStream())
                {
                    reservasImportadas = (List<Reserva>)serializer.Deserialize(stream);
                }

                _logger.LogInformation($"Deserialización completada. Procesando {reservasImportadas.Count} reservas.");

                foreach (var reserva in reservasImportadas)
                {
                    // Asegura que todos los componentes de la reserva existen antes de añadirlo a la base de datos
                    if (!await ValidarDependencias(reserva))
                    {
                        _logger.LogWarning($"Dependencias faltantes para la reserva con DNI: {reserva.DNI}");
                        continue;  // Saltar esta reserva si alguna dependencia no existe
                    }

                    _context.Add(reserva);
                    _logger.LogInformation($"Añadiendo nueva reserva con DNI: {reserva.DNI}");
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al importar reservas desde XML");
                ModelState.AddModelError("", "Error al procesar el archivo XML: " + ex.Message);
                return View();
            }
        }

        private async Task<bool> ValidarDependencias(Reserva reserva)
        {
            // Comprueba la existencia de Cliente
            bool clienteExiste = await _context.Cliente.AnyAsync(c => c.DNI == reserva.DNI);
            if (!clienteExiste)
            {
                _logger.LogWarning($"Cliente con DNI: {reserva.DNI} no existe.");
            }

            // Comprueba la existencia de Habitacion
            bool habitacionExiste = await _context.Habitacion.AnyAsync(h => h.Numero == reserva.HabitacionId);
            if (!habitacionExiste)
            {
                _logger.LogWarning($"Habitación con ID: {reserva.HabitacionId} no existe.");
            }

            // Comprueba la existencia de TipoAlojamiento
            bool tipoAlojamientoExiste = await _context.TipoAlojamiento.AnyAsync(t => t.Id == reserva.TipoAlojamientoId);
            if (!tipoAlojamientoExiste)
            {
                _logger.LogWarning($"Tipo de Alojamiento con ID: {reserva.TipoAlojamientoId} no existe.");
            }

            // Comprueba la existencia de TipoTemporada
            bool tipoTemporadaExiste = await _context.TipoTemporada.AnyAsync(t => t.Id == reserva.TipoTemporadaId);
            if (!tipoTemporadaExiste)
            {
                _logger.LogWarning($"Tipo de Temporada con ID: {reserva.TipoTemporadaId} no existe.");
            }

            // Comprueba la existencia de Oferta
            bool ofertaExiste = await _context.Oferta.AnyAsync(o => o.Id == reserva.OfertaId);
            if (!ofertaExiste)
            {
                _logger.LogWarning($"Oferta con ID: {reserva.OfertaId} no existe.");
            }

            return clienteExiste && habitacionExiste && tipoAlojamientoExiste && tipoTemporadaExiste && ofertaExiste;
        }
        [HttpGet]
        public async Task<IActionResult> ExportarXmlReservas()
        {
            var reservas = await _context.Reserva.ToListAsync();
            XmlSerializer serializer = new XmlSerializer(typeof(List<Reserva>), new XmlRootAttribute("Reservas"));

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, reservas);
                stream.Position = 0;
                return File(stream.ToArray(), "application/xml", "Reservas.xml");
            }
        }
    }
}
