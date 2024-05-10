using CapaModelo;
using FP._059_NAGASystems_Prod3.Controllers;
using FP._059_NAGASystems_Prod3;
using FP._059_NAGASystems_Prod3.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

public class CrearTipoAlojamiento : IDisposable
{
    private readonly DbContextOptions<FP_059_NAGASystems_Prod3Context> _options;
    private readonly FP_059_NAGASystems_Prod3Context _context;

    public CrearTipoAlojamiento()
    {
        _options = new DbContextOptionsBuilder<FP_059_NAGASystems_Prod3Context>()
            .UseInMemoryDatabase(databaseName: "TestDb_CrearTipoAlojamiento")
            .Options;
        _context = new FP_059_NAGASystems_Prod3Context(_options);
    }

    [Fact]
    public async Task Create_ValidTipoAlojamiento_RedirectsToIndex()
    {
        // Arrange
        var tipoAlojamientoController = new TipoAlojamientoController(_context);

        // Crear un TipoAlojamiento
        var tipoAlojamiento = new TipoAlojamiento
        {
            Id = 1,
            Descripcion = "Media Pensión",
            Precio = 100
        };

        // Act
        var result = await tipoAlojamientoController.Create(tipoAlojamiento);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectToActionResult.ActionName);

        // Verificar que el TipoAlojamiento fue añadido a la base de datos
        var tipoAlojamientoInDb = await _context.TipoAlojamiento.FirstOrDefaultAsync(t => t.Descripcion == tipoAlojamiento.Descripcion);
        Assert.NotNull(tipoAlojamientoInDb);
        Assert.Equal("Media Pensión", tipoAlojamientoInDb.Descripcion);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

