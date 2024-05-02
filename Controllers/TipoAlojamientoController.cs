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
    public class TipoAlojamientoController : Controller
    {
        private readonly FP_059_NAGASystems_Prod4Context _context;

        public TipoAlojamientoController(FP_059_NAGASystems_Prod4Context context)
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

        // Método GET para mostrar la vista de carga
        [HttpGet]
        public IActionResult ImportarXmlTipoAlojamiento()
        {
            return View();
        }

        // Método POST para procesar la carga del archivo XML
        [HttpPost]
        public async Task<IActionResult> ImportarXmlTipoAlojamiento(IFormFile archivoXml)
        {
            if (archivoXml == null || archivoXml.Length == 0)
            {
                ModelState.AddModelError("", "Debe proporcionar un archivo XML.");
                return View();
            }

            XmlSerializer serializer = new XmlSerializer(typeof(List<TipoAlojamiento>), new XmlRootAttribute("TiposAlojamiento"));
            List<TipoAlojamiento> tiposAlojamiento;

            try
            {
                using (var stream = archivoXml.OpenReadStream())
                {
                    tiposAlojamiento = (List<TipoAlojamiento>)serializer.Deserialize(stream);
                }

                foreach (var tipo in tiposAlojamiento)
                {
                    var tipoExistente = await _context.TipoAlojamiento.FindAsync(tipo.Id);
                    if (tipoExistente != null)
                    {
                        _context.Entry(tipoExistente).CurrentValues.SetValues(tipo);
                    }
                    else
                    {
                        _context.TipoAlojamiento.Add(tipo);
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); // Redirecciona a la lista de tipos de Alojamiento
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al importar el archivo XML: " + ex.Message);
                return View("Error"); // Vista de manejo de errores
            }
        }
        [HttpGet]
        public async Task<IActionResult> ExportarXmlTiposAlojamiento()
        {
            var tiposAlojamiento = await _context.TipoAlojamiento.ToListAsync();
            XmlSerializer serializer = new XmlSerializer(typeof(List<TipoAlojamiento>), new XmlRootAttribute("TiposAlojamiento"));

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, tiposAlojamiento);
                stream.Position = 0;
                return File(stream.ToArray(), "application/xml", "TiposAlojamiento.xml");
            }
        }
    }
}
