using CookComputing.XmlRpc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Xml;

namespace FP._059_NAGASystems_Prod3.Controllers
{
    public class ExportacionesController : Controller
    {
        // GET: ExportacionesController
        public ActionResult Index()
        {
            return View();
        }

        // POST: ExportacionesController/EnviarDatosOdoo
        [HttpPost]
        public ActionResult EnviarDatosOdoo(IFormCollection collection)
        {
            try
            {
                // Procesar los datos del formulario
                var file = collection.Files.GetFile("file");
                var batchDate = DateTime.Parse(collection["batchDate"]);

                // Leer el archivo XML
                var xmlDocument = new XmlDocument();
                xmlDocument.Load(file.FileName);

                // Obtener los elementos de la tabla
                var tablas = xmlDocument.DocumentElement.ChildNodes;

                // Crear un proxy para el servicio common
                var common = XmlRpcProxyGen.Create<ICommon>();
                common.Url = "https://hotelsol.odoo.com/xmlrpc/2/common";

                // Autenticarse en Odoo
                var uid = common.authenticate("database", "galmirallm@uoc.edu", "Nagasystems", new XmlRpcStruct());

                // Crear un proxy para el servicio object
                var obj = XmlRpcProxyGen.Create<IObject>();
                obj.Url = "https://hotelsol.odoo.com/xmlrpc/2/object";

                // Iterar sobre las tablas
                foreach (XmlNode tabla in tablas)
                {
                    // Obtener el nombre de la tabla
                    var nombreTabla = tabla.Name;

                    // Iterar sobre las filas de la tabla
                    foreach (XmlNode fila in tabla.ChildNodes)
                    {
                        // Crear un diccionario para almacenar los datos de la fila
                        var datosFila = new Dictionary<string, object>();

                        // Iterar sobre las columnas de la fila
                        foreach (XmlNode columna in fila.ChildNodes)
                        {
                            // Agregar los datos de la columna al diccionario
                            datosFila.Add(columna.Name, columna.InnerText);
                        }

                        // Enviar los datos a Odoo
                        var result = obj.execute_kw("database", uid, "Nagasystems", nombreTabla, "create", new object[] { datosFila });
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }

    public interface ICommon : IXmlRpcProxy
    {
        [XmlRpcMethod("login")]
        int authenticate(string db, string username, string password, XmlRpcStruct parameters);
    }

    public interface IObject : IXmlRpcProxy
    {
        [XmlRpcMethod("execute_kw")]
        object execute_kw(string db, int uid, string password, string model, string method, object[] args);
    }
}
