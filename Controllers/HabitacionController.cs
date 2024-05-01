using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CapaModelo;
using FP._059_NAGASystems_Prod3.Data;
using static CapaModelo.Habitacion;

namespace FP._059_NAGASystems_Prod3.Controllers
{
    public class HabitacionController : Controller
    {
        private readonly FP_059_NAGASystems_Prod3Context _context;
        private readonly HabitacionServicio _habitacionServicio;

        public HabitacionController(FP_059_NAGASystems_Prod3Context context)
        {
            _context = context;
            _habitacionServicio = new HabitacionServicio(context); 
        }

        // Método para mostrar habitaciones disponibles
        [HttpPost]
        public async Task<IActionResult> VerHabitacionesDisponibles(string fechaInicioStr, string fechaFinStr)
        {
            try
            {
                if (string.IsNullOrEmpty(fechaInicioStr) || string.IsNullOrEmpty(fechaFinStr))
                {
                    ModelState.AddModelError("", "Ambas fechas son requeridas.");
                    return View();
                }

                var fechaInicio = DateTime.Parse(fechaInicioStr);
                var fechaFin = DateTime.Parse(fechaFinStr);

                if (fechaInicio >= fechaFin)
                {
                    ModelState.AddModelError("", "La fecha de inicio debe ser anterior a la fecha de fin.");
                    return View();
                }

                var habitaciones = await _habitacionServicio.ObtenerHabitacionesDisponibles(fechaInicio, fechaFin);
                return View("HabitacionesDisponibles", habitaciones);
            }
            catch (FormatException)
            {
                ModelState.AddModelError("", "Formato de fecha inválido.");
                return View();
            }
        }
        public IActionResult VerHabitacionesDisponibles()
        {
            return View("VerHabitacionesDisponibles");
        }
        // GET: Habitacion
        public async Task<IActionResult> Index()
        {
            var habitaciones = _context.Habitacion.Include(h => h.TipoHabitacion);
            return View(await habitaciones.ToListAsync());
        }

        // GET: Habitacion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var habitacion = await _context.Habitacion
                .Include(h => h.TipoHabitacion)  // Añade esta línea
                .FirstOrDefaultAsync(m => m.Numero == id);
            if (habitacion == null)
            {
                return NotFound();
            }

            return View(habitacion);
        }

        // GET: Habitacion/Create
        public IActionResult Create()
        {
            ViewBag.TipoHabitacionId = new SelectList(_context.TipoHabitacion, "Id", "Descripcion");
            return View();
        }

        // POST: Habitacion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TipoHabitacionId,Estado")] Habitacion habitacion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(habitacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.TipoHabitacionId = new SelectList(_context.TipoHabitacion, "Id", "Descripcion", habitacion.TipoHabitacionId);
            return View(habitacion);
        }

        // GET: Habitacion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var habitacion = await _context.Habitacion
                .Include(h => h.TipoHabitacion)  // Añade esta línea
                .FirstOrDefaultAsync(m => m.Numero == id);
            if (habitacion == null)
            {
                return NotFound();
            }
            ViewBag.TipoHabitacionId = new SelectList(_context.TipoHabitacion, "Id", "Descripcion", habitacion.TipoHabitacionId);
            return View(habitacion);
        }

        // POST: Habitacion/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Numero,TipoHabitacionId,Estado")] Habitacion habitacion)
        {
            if (id != habitacion.Numero)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(habitacion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HabitacionExists(habitacion.Numero))
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
            return View(habitacion);
        }

        // GET: Habitacion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var habitacion = await _context.Habitacion
                .FirstOrDefaultAsync(m => m.Numero == id);
            if (habitacion == null)
            {
                return NotFound();
            }

            return View(habitacion);
        }

        // POST: Habitacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var habitacion = await _context.Habitacion.FindAsync(id);
            if (habitacion != null)
            {
                _context.Habitacion.Remove(habitacion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HabitacionExists(int id)
        {
            return _context.Habitacion.Any(e => e.Numero == id);
        }
    }
}
