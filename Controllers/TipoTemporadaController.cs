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

namespace FP._059_NAGASystems_Prod4
{
    public class TipoTemporadaController : Controller
    {
        private readonly FP_059_NAGASystems_Prod4Context _context;

        public TipoTemporadaController(FP_059_NAGASystems_Prod4Context context)
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
        public IActionResult ImportarXmlTemporada()
        {
            // Este es para la solicitud GET para mostrar el formulario
            return View();
        }
        // Método POST para procesar la carga de XML
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportarXmlTemporada(IFormFile archivoXml)
        {
            if (archivoXml == null || archivoXml.Length == 0)
            {
                ModelState.AddModelError("", "Debe seleccionar un archivo XML para cargar.");
                return View();
            }

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<TipoTemporada>), new XmlRootAttribute("Temporadas"));
                List<TipoTemporada> temporadasImportadas;

                using (var stream = archivoXml.OpenReadStream())
                {
                    temporadasImportadas = (List<TipoTemporada>)serializer.Deserialize(stream);
                }

                foreach (var temporada in temporadasImportadas)
                {
                    var temporadaExistente = await _context.Oferta.FindAsync(temporada.Id);
                    if (temporadaExistente != null)
                    {
                        _context.Entry(temporadaExistente).CurrentValues.SetValues(temporada);
                    }
                    else
                    {
                        _context.Add(temporada);
                    }
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Temporadas importadas correctamente.";
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", "Error al procesar el archivo XML: " + ex.Message);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error general al importar Temporadas: " + ex.Message);
                return View();
            }

            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> ExportarXmlTiposTemporada()
        {
            var tiposTemporada = await _context.TipoTemporada.ToListAsync();
            XmlSerializer serializer = new XmlSerializer(typeof(List<TipoTemporada>), new XmlRootAttribute("TiposTemporada"));

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, tiposTemporada);
                stream.Position = 0;
                return File(stream.ToArray(), "application/xml", "TiposTemporada.xml");
            }
        }
    }
}
