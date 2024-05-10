using CapaModelo;
using FP._059_NAGASystems_Prod3.Controllers;
using FP._059_NAGASystems_Prod3;
using FP._059_NAGASystems_Prod3.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

public class EditarTipoHabitacion : IDisposable
{
    private readonly DbContextOptions<FP_059_NAGASystems_Prod3Context> _options;
    private readonly FP_059_NAGASystems_Prod3Context _context;

    public EditarTipoHabitacion()
    {
        _options = new DbContextOptionsBuilder<FP_059_NAGASystems_Prod3Context>()
            .UseInMemoryDatabase(databaseName: "TestDb_EditarTipoHabitacion")
            .Options;
        _context = new FP_059_NAGASystems_Prod3Context(_options);
    }

    [Fact]
    public async Task Edit_ValidTipoHabitacionData_UpdatesTipoHabitacionAndRedirectsToIndex()
    {
        // Arrange
        var tipoHabitacionController = new TipoHabitacionController(_context);

        // Crear un TipoHabitacion original
        var originalTipoHabitacion = new TipoHabitacion
        {
            Id = 1,
            Descripcion = "Individual",
            Precio = 50
        };
        _context.TipoHabitacion.Add(originalTipoHabitacion);
        await _context.SaveChangesAsync();

        // Modificar los datos del TipoHabitacion
        originalTipoHabitacion.Descripcion = "Doble";
        originalTipoHabitacion.Precio = 100;

        // Act
        var result = await tipoHabitacionController.Edit(originalTipoHabitacion.Id, originalTipoHabitacion);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectToActionResult.ActionName);

        // Verificar que el TipoHabitacion se haya actualizado correctamente en la base de datos
        var updatedTipoHabitacion = await _context.TipoHabitacion.FindAsync(originalTipoHabitacion.Id);
        Assert.Equal("Doble", updatedTipoHabitacion.Descripcion);
        Assert.Equal(100, updatedTipoHabitacion.Precio);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
