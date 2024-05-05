using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IO;

public class ExportacionesController : Controller
{
    // Método para cargar la vista del formulario (GET request)
    [HttpGet]
    public IActionResult EnviarDatosOdoo()
    {;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> EnviarDatosOdoo(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return View("Error", model: "No file provided or file is empty.");
        }

        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string relativePath = @"Scripts\OdooApi.py"; // Ruta relativa desde el directorio base
        string fullPath = Path.Combine(baseDirectory, relativePath);
        string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

        try
        {
            // Asegurar que el directorio de uploads existe
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var filePath = Path.Combine(uploadPath, file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Aquí se llama al proceso de Python, usando el path del archivo guardado
            ProcessStartInfo start = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = string.Format("\"{0}\" \"{1}\"", fullPath, filePath),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Console.WriteLine(result);
                }

                using (StreamReader reader = process.StandardError)
                {
                    string error = reader.ReadToEnd();
                    if (!string.IsNullOrEmpty(error))
                    {
                        Console.WriteLine("Error: " + error);
                        return View("Error", model: error);
                    }
                }
            }

            return View("Success");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception: " + ex.Message);
            return View("Error", model: ex.Message);
        }
    }
    [HttpPost]
    public IActionResult GenerarXML()
    {
        string scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts", "exportJunto.py");
        string args = ""; // cualquier argumento necesario
        ProcessStartInfo start = new ProcessStartInfo("python", $"{scriptPath} {args}")
        {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        try
        {
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    ViewBag.Message = "XML generado con éxito.";
                }
            }
        }
        catch (Exception ex)
        {
            ViewBag.Message = "Error generando XML: " + ex.Message;
            return View("Error");
        }
        return RedirectToAction("EnviarDatosOdoo");
    }

}
