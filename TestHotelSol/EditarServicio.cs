using CapaModelo;
using FP._059_NAGASystems_Prod3.Controllers;
using FP._059_NAGASystems_Prod3;
using FP._059_NAGASystems_Prod3.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Xunit;

public class EditarServicio : IDisposable
{
    private readonly DbContextOptions<FP_059_NAGASystems_Prod3Context> _options;
    private readonly FP_059_NAGASystems_Prod3Context _context;

    public EditarServicio()
    {
        _options = new DbContextOptionsBuilder<FP_059_NAGASystems_Prod3Context>()
            .UseInMemoryDatabase(databaseName: "TestDb_EditarServicio")
            .Options;
        _context = new FP_059_NAGASystems_Prod3Context(_options);
    }

    [Fact]
    public async Task Edit_PostValidServicioData_UpdatesServicioAndRedirectsToIndex()
    {
        // Arrange
        var servicioController = new ServicioController(_context);

        // Crear servicio original
        var servicioOriginal = new Servicio
        {
            Id = 1,
            Descripcion = "Servicio de limpieza diaria de habitaciones",
            Precio = 50
        };
        _context.Servicio.Add(servicioOriginal);
        await _context.SaveChangesAsync();

        // Modificar el servicio original
        servicioOriginal.Descripcion = "Servicio de limpieza diaria y de lavandería de habitaciones";
        servicioOriginal.Precio = 60;

        // Act
        var result = await servicioController.Edit(servicioOriginal.Id, servicioOriginal);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectToActionResult.ActionName);

        var updatedServicio = await _context.Servicio.FindAsync(servicioOriginal.Id);
        Assert.Equal("Servicio de limpieza diaria y de lavandería de habitaciones", updatedServicio.Descripcion);
        Assert.Equal(60, updatedServicio.Precio);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
