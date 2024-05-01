using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Xml.Serialization;
using CapaModelo;
using FP._059_NAGASystems_Prod3.Data;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FP._059_NAGASystems_Prod3.Controllers
{
    public class ClienteController : Controller
    {
        private readonly FP_059_NAGASystems_Prod3Context _context;

        public ClienteController(FP_059_NAGASystems_Prod3Context context)
        {
            _context = context;
        }

        // GET: Cliente
        public async Task<IActionResult> Index()
        {
            return View(await _context.Cliente.ToListAsync());
        }

        // GET: Cliente/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Cliente
                .FirstOrDefaultAsync(m => m.DNI == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // GET: Cliente/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cliente/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DNI,Nombre,Apellido1,Apellido2,Telefono,Direccion,Email,VIP,Estado")] Cliente cliente)
        {
            // Verificar si ya existe un cliente con el mismo DNI
            bool dniExists = _context.Cliente.Any(c => c.DNI == cliente.DNI);
            if (dniExists)
            {
                // Añadir un mensaje de error al ModelState para mostrar en la vista
                ModelState.AddModelError("DNI", "El DNI introducido ya está en uso.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(cliente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // Si el ModelState no es válido o el DNI ya existe, se devuelve a la vista de creación con los datos introducidos
            return View(cliente);
        }


        // GET: Cliente/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Cliente.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        // POST: Cliente/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("DNI,Nombre,Apellido1,Apellido2,Telefono,Direccion,Email,VIP,Estado")] Cliente cliente)
        {
            if (id != cliente.DNI)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.DNI))
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
            return View(cliente);
        }

        // GET: Cliente/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Cliente
                .FirstOrDefaultAsync(m => m.DNI == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // POST: Cliente/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var cliente = await _context.Cliente.FindAsync(id);
            if (cliente != null)
            {
                _context.Cliente.Remove(cliente);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClienteExists(string id)
        {
            return _context.Cliente.Any(e => e.DNI == id);
        }


        public IActionResult ImportarXml()
        {
            // Este es para la solicitud GET para mostrar el formulario
            return View();
        }



        // Método POST para procesar la carga de XML
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportarXml(IFormFile archivoXml)
        {
            if (archivoXml != null && archivoXml.Length > 0)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Cliente>), new XmlRootAttribute("Clientes"));
                List<Cliente> clientesImportados;
                List<string> dnisActualizados = new List<string>();

                using (var stream = archivoXml.OpenReadStream())
                {
                    clientesImportados = (List<Cliente>)serializer.Deserialize(stream);
                }

                foreach (var cliente in clientesImportados)
                {
                    var clienteExistente = await _context.Cliente.FindAsync(cliente.DNI);
                    if (clienteExistente != null)
                    {
                        _context.Entry(clienteExistente).CurrentValues.SetValues(cliente);
                        dnisActualizados.Add(cliente.DNI); // Guardar DNI del cliente actualizado
                    }
                    else
                    {
                        _context.Add(cliente);
                    }
                }

                await _context.SaveChangesAsync();

                // Guardar los DNIs actualizados en TempData para mostrar en la vista
                if (dnisActualizados.Any())
                {
                    TempData["DnisActualizados"] = string.Join(", ", dnisActualizados) + " ya estaban incluidos en la base de datos y se han actualizado los datos con los proporcionados por el XML.";
                }

                return RedirectToAction(nameof(Index));
            }

            return View();
        }
    }

}
