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
    public class ReservaServicioController : Controller
    {
        private readonly FP_059_NAGASystems_Prod3Context _context;

        public ReservaServicioController(FP_059_NAGASystems_Prod3Context context)
        {
            _context = context;
        }

        // GET: ReservaServicio
        public async Task<IActionResult> Index()
        {
            return View(await _context.ReservaServicio.ToListAsync());
        }

        // GET: ReservaServicio/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservaServicio = await _context.ReservaServicio
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservaServicio == null)
            {
                return NotFound();
            }

            return View(reservaServicio);
        }

        // GET: ReservaServicio/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ReservaServicio/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Reserva,Servicio,Fecha")] ReservaServicio reservaServicio)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reservaServicio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(reservaServicio);
        }

        // GET: ReservaServicio/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservaServicio = await _context.ReservaServicio.FindAsync(id);
            if (reservaServicio == null)
            {
                return NotFound();
            }
            return View(reservaServicio);
        }

        // POST: ReservaServicio/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Reserva,Servicio,Fecha")] ReservaServicio reservaServicio)
        {
            if (id != reservaServicio.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservaServicio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservaServicioExists(reservaServicio.Id))
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
            return View(reservaServicio);
        }

        // GET: ReservaServicio/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservaServicio = await _context.ReservaServicio
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservaServicio == null)
            {
                return NotFound();
            }

            return View(reservaServicio);
        }

        // POST: ReservaServicio/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservaServicio = await _context.ReservaServicio.FindAsync(id);
            if (reservaServicio != null)
            {
                _context.ReservaServicio.Remove(reservaServicio);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservaServicioExists(int id)
        {
            return _context.ReservaServicio.Any(e => e.Id == id);
        }
    }
}
