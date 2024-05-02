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
    public class ServicioController : Controller
    {
        private readonly FP_059_NAGASystems_Prod4Context _context;

        public ServicioController(FP_059_NAGASystems_Prod4Context context)
        {
            _context = context;
        }

        // GET: Servicio
        public async Task<IActionResult> Index()
        {
            return View(await _context.Servicio.ToListAsync());
        }

        // GET: Servicio/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicio = await _context.Servicio
                .FirstOrDefaultAsync(m => m.Id == id);
            if (servicio == null)
            {
                return NotFound();
            }

            return View(servicio);
        }

        // GET: Servicio/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Servicio/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Descripcion,Precio")] Servicio servicio)
        {
            if (ModelState.IsValid)
            {
                _context.Add(servicio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(servicio);
        }

        // GET: Servicio/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicio = await _context.Servicio.FindAsync(id);
            if (servicio == null)
            {
                return NotFound();
            }
            return View(servicio);
        }

        // POST: Servicio/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Descripcion,Precio")] Servicio servicio)
        {
            if (id != servicio.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(servicio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServicioExists(servicio.Id))
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
            return View(servicio);
        }

        // GET: Servicio/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicio = await _context.Servicio
                .FirstOrDefaultAsync(m => m.Id == id);
            if (servicio == null)
            {
                return NotFound();
            }

            return View(servicio);
        }

        // POST: Servicio/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var servicio = await _context.Servicio.FindAsync(id);
            if (servicio != null)
            {
                _context.Servicio.Remove(servicio);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServicioExists(int id)
        {
            return _context.Servicio.Any(e => e.Id == id);
        }
        // Método GET para mostrar la vista de carga
        [HttpGet]
        public IActionResult ImportarXmlServicio()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportarXmlServicio(IFormFile archivoXml)
        {
            if (archivoXml == null || archivoXml.Length == 0)
            {
                ModelState.AddModelError("", "Debe seleccionar un archivo XML para cargar.");
                return View();
            }

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Servicio>), new XmlRootAttribute("Servicios"));
                List<Servicio> serviciosImportados;

                using (var stream = archivoXml.OpenReadStream())
                {
                    serviciosImportados = (List<Servicio>)serializer.Deserialize(stream);
                }

                foreach (var servicio in serviciosImportados)
                {
                    var servicioExistente = await _context.Servicio.FindAsync(servicio.Id);
                    if (servicioExistente != null)
                    {
                        _context.Entry(servicioExistente).CurrentValues.SetValues(servicio);
                    }
                    else
                    {
                        _context.Add(servicio);
                    }
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Servicios importados correctamente.";
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", "Error al procesar el archivo XML: " + ex.Message);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error general al importar servicios: " + ex.Message);
                return View();
            }

            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> ExportarXmlServicios()
        {
            var servicios = await _context.Servicio.ToListAsync();
            XmlSerializer serializer = new XmlSerializer(typeof(List<Servicio>), new XmlRootAttribute("Servicios"));

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, servicios);
                stream.Position = 0;
                return File(stream.ToArray(), "application/xml", "Servicios.xml");
            }
        }

    }
}
