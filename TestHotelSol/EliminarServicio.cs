using CapaModelo;
using FP._059_NAGASystems_Prod3.Controllers;
using FP._059_NAGASystems_Prod3;
using FP._059_NAGASystems_Prod3.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Xunit;

public class EliminarServicio : IDisposable
{
    private readonly DbContextOptions<FP_059_NAGASystems_Prod3Context> _options;
    private readonly FP_059_NAGASystems_Prod3Context _context;

    public EliminarServicio()
    {
        _options = new DbContextOptionsBuilder<FP_059_NAGASystems_Prod3Context>()
            .UseInMemoryDatabase(databaseName: "TestDb_EliminarServicio")
            .Options;
        _context = new FP_059_NAGASystems_Prod3Context(_options);
    }

    [Fact]
    public async Task DeleteConfirmed_ValidServicioId_DeletesServicioAndRedirectsToIndex()
    {
        // Arrange
        var servicioController = new ServicioController(_context);

        // Crear servicio a eliminar
        var servicioAEliminar = new Servicio
        {
            Id = 1,
            Descripcion = "Servicio de limpieza diaria de habitaciones",
            Precio = 50
        };
        _context.Servicio.Add(servicioAEliminar);
        await _context.SaveChangesAsync();

        // Act
        var result = await servicioController.DeleteConfirmed(servicioAEliminar.Id);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectToActionResult.ActionName);

        var deletedServicio = await _context.Servicio.FindAsync(servicioAEliminar.Id);
        Assert.Null(deletedServicio); // Verificar que el servicio fue eliminado de la base de datos
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

