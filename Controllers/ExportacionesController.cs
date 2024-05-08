using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Text;

namespace FP._059_NAGASystems_Prod3.Controllers
{
    public class ExportacionesController : Controller
    {
        private readonly ILogger<ExportacionesController> _logger;

        public ExportacionesController(ILogger<ExportacionesController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        //Exportar XML por separado
        public IActionResult ExportarXML()
        {
            string pythonScriptPath = @"C:\Users\usuario\Repositories\UOC\Nagasystems\FP.059-NAGASystems-Prod4\py\exportSeparado.py"; //Especificar la ruta personal que corresponda a cada usuario
            string pythonExecutablePath = @"C:\Users\usuario\AppData\Local\Programs\Python\Python39\python.exe"; //Especificar la ruta personal que corresponda a cada usuario

            if (!System.IO.File.Exists(pythonScriptPath) || !System.IO.File.Exists(pythonExecutablePath))
            {
                ViewBag.Error = "La ruta del script de Python o del ejecutable no es válida.";
                _logger.LogError("La ruta del script de Python o del ejecutable no es válida.");
                return View();
            }

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = pythonExecutablePath,
                Arguments = $"\"{pythonScriptPath}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            try
            {
                using (var process = Process.Start(psi))
                {
                    if (process == null)
                    {
                        ViewBag.Error = "No se pudo iniciar el proceso de Python.";
                        _logger.LogError("No se pudo iniciar el proceso de Python.");
                        return View();
                    }

                    // Espera a que el proceso termine antes de leer los streams
                    process.WaitForExit();

                    // Lee los streams directamente usando los métodos ReadToEnd
                    string result = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        ViewBag.Result = result;
                    }

                    if (!string.IsNullOrWhiteSpace(error))
                    {
                        ViewBag.Error = error;
                    }
                    else if (process.ExitCode != 0)
                    {
                        ViewBag.Error = $"El script de Python terminó con el código de salida {process.ExitCode}, lo que indica un error.";
                        _logger.LogWarning($"El script de Python terminó con el código de salida {process.ExitCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al ejecutar el script de Python");
                ViewBag.Error = $"Error al intentar ejecutar el script de Python '{pythonScriptPath}': {ex.Message}";
            }

            return View();
        }

        //Exportar XML junto
        public IActionResult ExportarXMLTodo()
        {
            string pythonScriptPath = @"C:\Users\usuario\Repositories\UOC\Nagasystems\FP.059-NAGASystems-Prod4\py\exportJunto.py"; //Especificar la ruta personal que corresponda a cada usuario
            string pythonExecutablePath = @"C:\Users\usuario\AppData\Local\Programs\Python\Python39\python.exe"; //Especificar la ruta personal que corresponda a cada usuario

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = pythonExecutablePath,
                Arguments = $"\"{pythonScriptPath}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            StringBuilder output = new StringBuilder();
            StringBuilder error = new StringBuilder();

            try
            {
                using (var process = Process.Start(psi))
                {
                    process.OutputDataReceived += (sender, args) => output.AppendLine(args.Data);
                    process.ErrorDataReceived += (sender, args) => error.AppendLine(args.Data);

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();

                    ViewBag.Result = output.ToString();
                    ViewBag.Error = error.ToString();

                    if (process.ExitCode != 0)
                    {
                        ViewBag.Error += "\nEl proceso no se completó exitosamente.";
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al ejecutar el proceso de Python: " + ex.Message;
            }

            return View("ExportarXML"); // Utiliza la misma vista o una específica si es necesario
        }


       
    }

}

