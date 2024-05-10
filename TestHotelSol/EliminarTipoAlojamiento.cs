using CapaModelo;
using FP._059_NAGASystems_Prod3.Controllers;
using FP._059_NAGASystems_Prod3;
using FP._059_NAGASystems_Prod3.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Xunit;

public class EliminarTipoAlojamiento : IDisposable
{
    private readonly DbContextOptions<FP_059_NAGASystems_Prod3Context> _options;
    private readonly FP_059_NAGASystems_Prod3Context _context;

    public EliminarTipoAlojamiento()
    {
        _options = new DbContextOptionsBuilder<FP_059_NAGASystems_Prod3Context>()
            .UseInMemoryDatabase(databaseName: "TestDb_EliminarTipoAlojamiento")
            .Options;
        _context = new FP_059_NAGASystems_Prod3Context(_options);
    }

    [Fact]
    public async Task Delete_ValidTipoAlojamientoId_DeletesTipoAlojamientoAndRedirectsToIndex()
    {
        // Arrange
        var tipoAlojamientoController = new TipoAlojamientoController(_context);

        // Crear un TipoAlojamiento
        var tipoAlojamiento = new TipoAlojamiento
        {
            Id = 1,
            Descripcion = "Estandar",
            Precio = 100
        };
        _context.TipoAlojamiento.Add(tipoAlojamiento);
        await _context.SaveChangesAsync();

        // Act
        var result = await tipoAlojamientoController.DeleteConfirmed(tipoAlojamiento.Id);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectToActionResult.ActionName);

        // Verificar que el TipoAlojamiento se haya eliminado correctamente de la base de datos
        var deletedTipoAlojamiento = await _context.TipoAlojamiento.FindAsync(tipoAlojamiento.Id);
        Assert.Null(deletedTipoAlojamiento);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

