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
    public class OfertaController : Controller
    {
        private readonly FP_059_NAGASystems_Prod4Context _context;

        public OfertaController(FP_059_NAGASystems_Prod4Context context)
        {
            _context = context;
        }

        // GET: Oferta
        public async Task<IActionResult> Index()
        {
            return View(await _context.Oferta.ToListAsync());
        }

        // GET: Oferta/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var oferta = await _context.Oferta
                .FirstOrDefaultAsync(m => m.Id == id);
            if (oferta == null)
            {
                return NotFound();
            }

            return View(oferta);
        }

        // GET: Oferta/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Oferta/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Descripcion,Coeficiente")] Oferta oferta)
        {
            if (ModelState.IsValid)
            {
                _context.Add(oferta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(oferta);
        }

        // GET: Oferta/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var oferta = await _context.Oferta.FindAsync(id);
            if (oferta == null)
            {
                return NotFound();
            }
            return View(oferta);
        }

        // POST: Oferta/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Descripcion,Coeficiente")] Oferta oferta)
        {
            if (id != oferta.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(oferta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OfertaExists(oferta.Id))
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
            return View(oferta);
        }

        // GET: Oferta/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var oferta = await _context.Oferta
                .FirstOrDefaultAsync(m => m.Id == id);
            if (oferta == null)
            {
                return NotFound();
            }

            return View(oferta);
        }

        // POST: Oferta/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var oferta = await _context.Oferta.FindAsync(id);
            if (oferta != null)
            {
                _context.Oferta.Remove(oferta);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OfertaExists(int id)
        {
            return _context.Oferta.Any(e => e.Id == id);
        }
        public IActionResult ImportarXmlOferta()
        {
            // Este es para la solicitud GET para mostrar el formulario
            return View();
        }

        // Método POST para procesar la carga de XML
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportarXml(IFormFile archivoXml)
        {
            if (archivoXml == null || archivoXml.Length == 0)
            {
                ModelState.AddModelError("", "Debe seleccionar un archivo XML para cargar.");
                return View();
            }

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Oferta>), new XmlRootAttribute("Ofertas"));
                List<Oferta> ofertasImportadas;

                using (var stream = archivoXml.OpenReadStream())
                {
                    ofertasImportadas = (List<Oferta>)serializer.Deserialize(stream);
                }

                foreach (var oferta in ofertasImportadas)
                {
                    var ofertaExistente = await _context.Oferta.FindAsync(oferta.Id);
                    if (ofertaExistente != null)
                    {
                        _context.Entry(ofertaExistente).CurrentValues.SetValues(oferta);
                    }
                    else
                    {
                        _context.Add(oferta);
                    }
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Ofertas importadas correctamente.";
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", "Error al procesar el archivo XML: " + ex.Message);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error general al importar ofertas: " + ex.Message);
                return View();
            }

            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> ExportarXmlOfertas()
        {
            var ofertas = await _context.Oferta.ToListAsync();
            XmlSerializer serializer = new XmlSerializer(typeof(List<Oferta>), new XmlRootAttribute("Ofertas"));

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, ofertas);
                stream.Position = 0;
                return File(stream.ToArray(), "application/xml", "Ofertas.xml");
            }
        }
    }
}
