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
    public class TipoHabitacionController : Controller
    {
        private readonly FP_059_NAGASystems_Prod4Context _context;

        public TipoHabitacionController(FP_059_NAGASystems_Prod4Context context)
        {
            _context = context;
        }

        // GET: TipoHabitacion
        public async Task<IActionResult> Index()
        {
            return View(await _context.TipoHabitacion.ToListAsync());
        }

        // GET: TipoHabitacion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoHabitacion = await _context.TipoHabitacion
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipoHabitacion == null)
            {
                return NotFound();
            }

            return View(tipoHabitacion);
        }

        // GET: TipoHabitacion/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TipoHabitacion/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Descripcion,Precio")] TipoHabitacion tipoHabitacion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tipoHabitacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tipoHabitacion);
        }

        // GET: TipoHabitacion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoHabitacion = await _context.TipoHabitacion.FindAsync(id);
            if (tipoHabitacion == null)
            {
                return NotFound();
            }
            return View(tipoHabitacion);
        }

        // POST: TipoHabitacion/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Descripcion,Precio")] TipoHabitacion tipoHabitacion)
        {
            if (id != tipoHabitacion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipoHabitacion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoHabitacionExists(tipoHabitacion.Id))
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
            return View(tipoHabitacion);
        }

        // GET: TipoHabitacion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoHabitacion = await _context.TipoHabitacion
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipoHabitacion == null)
            {
                return NotFound();
            }

            return View(tipoHabitacion);
        }

        // POST: TipoHabitacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipoHabitacion = await _context.TipoHabitacion.FindAsync(id);
            if (tipoHabitacion != null)
            {
                _context.TipoHabitacion.Remove(tipoHabitacion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TipoHabitacionExists(int id)
        {
            return _context.TipoHabitacion.Any(e => e.Id == id);
        }
        // Método GET para mostrar el formulario de carga de XML
        [HttpGet]
        public IActionResult ImportarXmlTipoHabitacion()
        {
            return View();
        }
        // Método POST para procesar la carga del archivo XML
        [HttpPost]
        public async Task<IActionResult> ImportarXmlTipoHabitacion(IFormFile archivoXml)
        {
            if (archivoXml == null || archivoXml.Length == 0)
            {
                ModelState.AddModelError("", "Debe proporcionar un archivo XML.");
                return View();
            }

            XmlSerializer serializer = new XmlSerializer(typeof(List<TipoHabitacion>), new XmlRootAttribute("TiposHabitacion"));
            List<TipoHabitacion> tiposHabitacion;

            try
            {
                using (var stream = archivoXml.OpenReadStream())
                {
                    tiposHabitacion = (List<TipoHabitacion>)serializer.Deserialize(stream);
                }

                foreach (var tipo in tiposHabitacion)
                {
                    var tipoExistente = await _context.TipoHabitacion.FindAsync(tipo.Id);
                    if (tipoExistente != null)
                    {
                        _context.Entry(tipoExistente).CurrentValues.SetValues(tipo);
                    }
                    else
                    {
                        _context.TipoHabitacion.Add(tipo);
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); // Redirecciona a la lista de tipos de habitación
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al importar el archivo XML: " + ex.Message);
                return View("Error"); // Vista de manejo de errores
            }
        }
        [HttpGet]
        public async Task<IActionResult> ExportarXmlTiposHabitacion()
        {
            var tiposHabitacion = await _context.TipoHabitacion.ToListAsync();
            XmlSerializer serializer = new XmlSerializer(typeof(List<TipoHabitacion>), new XmlRootAttribute("TiposHabitacion"));

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, tiposHabitacion);
                stream.Position = 0;
                return File(stream.ToArray(), "application/xml", "TiposHabitacion.xml");
            }
        }
    }
}
