using CapaModelo;
using FP._059_NAGASystems_Prod3.Controllers;
using FP._059_NAGASystems_Prod3;
using FP._059_NAGASystems_Prod3.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Xunit;

public class EditarTipoTemporada : IDisposable
{
    private readonly DbContextOptions<FP_059_NAGASystems_Prod3Context> _options;
    private readonly FP_059_NAGASystems_Prod3Context _context;

    public EditarTipoTemporada()
    {
        _options = new DbContextOptionsBuilder<FP_059_NAGASystems_Prod3Context>()
            .UseInMemoryDatabase(databaseName: "TestDb_EditarTipoTemporada")
            .Options;
        _context = new FP_059_NAGASystems_Prod3Context(_options);
    }

    [Fact]
    public async Task Edit_ValidTipoTemporadaData_UpdatesTipoTemporadaAndRedirectsToIndex()
    {
        // Arrange
        var tipoTemporadaController = new TipoTemporadaController(_context);

        // Crear un TipoTemporada original
        var originalTipoTemporada = new TipoTemporada
        {
            Descripcion = "Baja",
            Coeficiente = 1.5
        };
        _context.TipoTemporada.Add(originalTipoTemporada);
        await _context.SaveChangesAsync();

        // Modificar los datos del TipoTemporada
        originalTipoTemporada.Descripcion = "Alta";
        originalTipoTemporada.Coeficiente = 4.2;

        // Act
        var result = await tipoTemporadaController.Edit(originalTipoTemporada.Id, originalTipoTemporada);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectToActionResult.ActionName);

        // Verificar que el TipoTemporada se haya actualizado correctamente en la base de datos
        var updatedTipoTemporada = await _context.TipoTemporada.FindAsync(originalTipoTemporada.Id);
        Assert.Equal("Alta", updatedTipoTemporada.Descripcion);
        Assert.Equal(4.2, updatedTipoTemporada.Coeficiente);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
