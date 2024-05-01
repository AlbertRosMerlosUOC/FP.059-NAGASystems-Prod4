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
    public class TipoAlojamientoController : Controller
    {
        private readonly FP_059_NAGASystems_Prod3Context _context;

        public TipoAlojamientoController(FP_059_NAGASystems_Prod3Context context)
        {
            _context = context;
        }

        // GET: TipoAlojamiento
        public async Task<IActionResult> Index()
        {
            return View(await _context.TipoAlojamiento.ToListAsync());
        }

        // GET: TipoAlojamiento/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoAlojamiento = await _context.TipoAlojamiento
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipoAlojamiento == null)
            {
                return NotFound();
            }

            return View(tipoAlojamiento);
        }

        // GET: TipoAlojamiento/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TipoAlojamiento/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Descripcion,Precio")] TipoAlojamiento tipoAlojamiento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tipoAlojamiento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tipoAlojamiento);
        }

        // GET: TipoAlojamiento/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoAlojamiento = await _context.TipoAlojamiento.FindAsync(id);
            if (tipoAlojamiento == null)
            {
                return NotFound();
            }
            return View(tipoAlojamiento);
        }

        // POST: TipoAlojamiento/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Descripcion,Precio")] TipoAlojamiento tipoAlojamiento)
        {
            if (id != tipoAlojamiento.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipoAlojamiento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoAlojamientoExists(tipoAlojamiento.Id))
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
            return View(tipoAlojamiento);
        }

        // GET: TipoAlojamiento/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoAlojamiento = await _context.TipoAlojamiento
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipoAlojamiento == null)
            {
                return NotFound();
            }

            return View(tipoAlojamiento);
        }

        // POST: TipoAlojamiento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipoAlojamiento = await _context.TipoAlojamiento.FindAsync(id);
            if (tipoAlojamiento != null)
            {
                _context.TipoAlojamiento.Remove(tipoAlojamiento);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TipoAlojamientoExists(int id)
        {
            return _context.TipoAlojamiento.Any(e => e.Id == id);
        }
    }
}
