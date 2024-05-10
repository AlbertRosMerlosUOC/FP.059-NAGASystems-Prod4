using CapaModelo;
using FP._059_NAGASystems_Prod3.Controllers;
using FP._059_NAGASystems_Prod3;
using FP._059_NAGASystems_Prod3.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Xunit;

public class EliminarTipoTemporada : IDisposable
{
    private readonly DbContextOptions<FP_059_NAGASystems_Prod3Context> _options;
    private readonly FP_059_NAGASystems_Prod3Context _context;

    public EliminarTipoTemporada()
    {
        _options = new DbContextOptionsBuilder<FP_059_NAGASystems_Prod3Context>()
            .UseInMemoryDatabase(databaseName: "TestDb_EliminarTipoTemporada")
            .Options;
        _context = new FP_059_NAGASystems_Prod3Context(_options);
    }

    [Fact]
    public async Task Delete_ValidTipoTemporadaId_DeletesTipoTemporadaAndRedirectsToIndex()
    {
        // Arrange
        var tipoTemporadaController = new TipoTemporadaController(_context);

        // Crear un TipoTemporada
        var tipoTemporada = new TipoTemporada
        {
            Descripcion = "Baja",
            Coeficiente = 1.5
        };
        _context.TipoTemporada.Add(tipoTemporada);
        await _context.SaveChangesAsync();

        // Act
        var result = await tipoTemporadaController.DeleteConfirmed(tipoTemporada.Id);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectToActionResult.ActionName);

        // Verificar que el TipoTemporada se haya eliminado correctamente de la base de datos
        var deletedTipoTemporada = await _context.TipoTemporada.FindAsync(tipoTemporada.Id);
        Assert.Null(deletedTipoTemporada);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

