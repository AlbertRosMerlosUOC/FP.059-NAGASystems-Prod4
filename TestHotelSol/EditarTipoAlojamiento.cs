using CapaModelo;
using FP._059_NAGASystems_Prod3.Controllers;
using FP._059_NAGASystems_Prod3;
using FP._059_NAGASystems_Prod3.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Xunit;

public class EditarTipoAlojamiento : IDisposable
{
    private readonly DbContextOptions<FP_059_NAGASystems_Prod3Context> _options;
    private readonly FP_059_NAGASystems_Prod3Context _context;

    public EditarTipoAlojamiento()
    {
        _options = new DbContextOptionsBuilder<FP_059_NAGASystems_Prod3Context>()
            .UseInMemoryDatabase(databaseName: "TestDb_EditarTipoAlojamiento")
            .Options;
        _context = new FP_059_NAGASystems_Prod3Context(_options);
    }

    [Fact]
    public async Task Edit_ValidTipoAlojamientoData_UpdatesTipoAlojamientoAndRedirectsToIndex()
    {
        // Arrange
        var tipoAlojamientoController = new TipoAlojamientoController(_context);

        // Crear un TipoAlojamiento original
        var originalTipoAlojamiento = new TipoAlojamiento
        {
            Id = 1,
            Descripcion = "Media Pensión",
            Precio = 100
        };
        _context.TipoAlojamiento.Add(originalTipoAlojamiento);
        await _context.SaveChangesAsync();

        // Modificar los datos del TipoAlojamiento
        originalTipoAlojamiento.Precio = 150; // Nuevo precio

        // Act
        var result = await tipoAlojamientoController.Edit(originalTipoAlojamiento.Id, originalTipoAlojamiento);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectToActionResult.ActionName);

        // Verificar que el TipoAlojamiento se haya actualizado correctamente en la base de datos
        var updatedTipoAlojamiento = await _context.TipoAlojamiento.FindAsync(originalTipoAlojamiento.Id);
        Assert.Equal(150, updatedTipoAlojamiento.Precio);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}