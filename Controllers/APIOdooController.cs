using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.IO;

public class ExportacionesController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult ExportarXml(string tipoExportacion)
    {
        string basePath = AppDomain.CurrentDomain.BaseDirectory;
        string pythonScriptPath = Path.Combine(basePath, "py", "OdooExport.py");
        string pythonExecutable = "python3"; 
        string args = $"{pythonScriptPath} {tipoExportacion}";

        ProcessStartInfo start = new ProcessStartInfo(pythonExecutable)
        {
            Arguments = args,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        try
        {
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    ViewBag.Message = result; // Guarda el mensaje para mostrar en la vista
                }
                using (StreamReader reader = process.StandardError)
                {
                    string errorMsg = reader.ReadToEnd();
                    if (!string.IsNullOrEmpty(errorMsg))
                    {
                        ViewBag.Error = errorMsg; // Guarda el error para mostrar en la vista
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ViewBag.Error = "Failed to run Python script: " + ex.Message;
        }

        return View("Index"); // Regresa a la vista principal
    }
}