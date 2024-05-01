using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CapaModelo;
using FP._059_NAGASystems_Prod3.Data;

namespace FP._059_NAGASystems_Prod3
{
    public class DisponibilidadController : Controller
    {
        private readonly FP_059_NAGASystems_Prod3Context _context;

        public DisponibilidadController(FP_059_NAGASystems_Prod3Context context)
        {
            _context = context;
        }

        // GET: Disponibilidad
        public async Task<IActionResult> Index()
        {
            return View(await _context.Disponibilidad.ToListAsync());
        }

        // GET: Disponibilidad/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var disponibilidad = await _context.Disponibilidad
                .FirstOrDefaultAsync(m => m.Id == id);
            if (disponibilidad == null)
            {
                return NotFound();
            }

            return View(disponibilidad);
        }

        // GET: Disponibilidad/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Disponibilidad/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Habitacion,Fecha")] Disponibilidad disponibilidad)
        {
            if (ModelState.IsValid)
            {
                _context.Add(disponibilidad);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(disponibilidad);
        }

        // GET: Disponibilidad/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var disponibilidad = await _context.Disponibilidad.FindAsync(id);
            if (disponibilidad == null)
            {
                return NotFound();
            }
            return View(disponibilidad);
        }

        // POST: Disponibilidad/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Habitacion,Fecha")] Disponibilidad disponibilidad)
        {
            if (id != disponibilidad.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(disponibilidad);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DisponibilidadExists(disponibilidad.Id))
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
            return View(disponibilidad);
        }

        // GET: Disponibilidad/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var disponibilidad = await _context.Disponibilidad
                .FirstOrDefaultAsync(m => m.Id == id);
            if (disponibilidad == null)
            {
                return NotFound();
            }

            return View(disponibilidad);
        }

        // POST: Disponibilidad/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var disponibilidad = await _context.Disponibilidad.FindAsync(id);
            if (disponibilidad != null)
            {
                _context.Disponibilidad.Remove(disponibilidad);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DisponibilidadExists(int id)
        {
            return _context.Disponibilidad.Any(e => e.Id == id);
        }
    }
}
