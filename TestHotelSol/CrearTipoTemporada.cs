using CapaModelo;
using FP._059_NAGASystems_Prod3.Controllers;
using FP._059_NAGASystems_Prod3;
using FP._059_NAGASystems_Prod3.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Xunit;

public class CrearTipoTemporada : IDisposable
{
    private readonly DbContextOptions<FP_059_NAGASystems_Prod3Context> _options;
    private readonly FP_059_NAGASystems_Prod3Context _context;

    public CrearTipoTemporada()
    {
        _options = new DbContextOptionsBuilder<FP_059_NAGASystems_Prod3Context>()
            .UseInMemoryDatabase(databaseName: "TestDb_CrearTipoTemporada")
            .Options;
        _context = new FP_059_NAGASystems_Prod3Context(_options);
    }

    [Fact]
    public async Task Create_ValidTipoTemporada_RedirectsToIndex()
    {
        // Arrange
        var tipoTemporadaController = new TipoTemporadaController(_context);

        // Crear un TipoTemporada
        var tipoTemporada = new TipoTemporada
        {
            Id = 1,
            Descripcion = "Baja",
            Coeficiente = 1.5
        };

        // Act
        var result = await tipoTemporadaController.Create(tipoTemporada);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectToActionResult.ActionName);

        // Verificar que el TipoTemporada se haya creado correctamente en la base de datos
        var createdTipoTemporada = await _context.TipoTemporada.FirstOrDefaultAsync(t => t.Descripcion == tipoTemporada.Descripcion);
        Assert.NotNull(createdTipoTemporada);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

