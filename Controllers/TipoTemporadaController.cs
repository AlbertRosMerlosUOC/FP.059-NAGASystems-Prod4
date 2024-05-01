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
    public class TipoTemporadaController : Controller
    {
        private readonly FP_059_NAGASystems_Prod3Context _context;

        public TipoTemporadaController(FP_059_NAGASystems_Prod3Context context)
        {
            _context = context;
        }

        // GET: TipoTemporada
        public async Task<IActionResult> Index()
        {
            return View(await _context.TipoTemporada.ToListAsync());
        }

        // GET: TipoTemporada/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoTemporada = await _context.TipoTemporada
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipoTemporada == null)
            {
                return NotFound();
            }

            return View(tipoTemporada);
        }

        // GET: TipoTemporada/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TipoTemporada/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Descripcion,Coeficiente")] TipoTemporada tipoTemporada)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tipoTemporada);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tipoTemporada);
        }

        // GET: TipoTemporada/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoTemporada = await _context.TipoTemporada.FindAsync(id);
            if (tipoTemporada == null)
            {
                return NotFound();
            }
            return View(tipoTemporada);
        }

        // POST: TipoTemporada/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Descripcion,Coeficiente")] TipoTemporada tipoTemporada)
        {
            if (id != tipoTemporada.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipoTemporada);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoTemporadaExists(tipoTemporada.Id))
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
            return View(tipoTemporada);
        }

        // GET: TipoTemporada/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoTemporada = await _context.TipoTemporada
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipoTemporada == null)
            {
                return NotFound();
            }

            return View(tipoTemporada);
        }

        // POST: TipoTemporada/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipoTemporada = await _context.TipoTemporada.FindAsync(id);
            if (tipoTemporada != null)
            {
                _context.TipoTemporada.Remove(tipoTemporada);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TipoTemporadaExists(int id)
        {
            return _context.TipoTemporada.Any(e => e.Id == id);
        }
    }
}
