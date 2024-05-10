using CapaModelo;
using FP._059_NAGASystems_Prod3.Controllers;
using FP._059_NAGASystems_Prod3;
using FP._059_NAGASystems_Prod3.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

public class EliminarTipoHabitacion : IDisposable
{
    private readonly DbContextOptions<FP_059_NAGASystems_Prod3Context> _options;
    private readonly FP_059_NAGASystems_Prod3Context _context;

    public EliminarTipoHabitacion()
    {
        _options = new DbContextOptionsBuilder<FP_059_NAGASystems_Prod3Context>()
            .UseInMemoryDatabase(databaseName: "TestDb_EliminarTipoHabitacion")
            .Options;
        _context = new FP_059_NAGASystems_Prod3Context(_options);
    }

    [Fact]
    public async Task Delete_ValidTipoHabitacionId_RemovesTipoHabitacionAndRedirectsToIndex()
    {
        // Arrange
        var tipoHabitacionController = new TipoHabitacionController(_context);

        // Crear un TipoHabitacion a eliminar
        var tipoHabitacion = new TipoHabitacion
        {
            Id = 1,
            Descripcion = "Individual",
            Precio = 50
        };
        _context.TipoHabitacion.Add(tipoHabitacion);
        await _context.SaveChangesAsync();

        // Act
        var result = await tipoHabitacionController.DeleteConfirmed(tipoHabitacion.Id);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectToActionResult.ActionName);

        // Verificar que el TipoHabitacion se haya eliminado correctamente de la base de datos
        var deletedTipoHabitacion = await _context.TipoHabitacion.FindAsync(tipoHabitacion.Id);
        Assert.Null(deletedTipoHabitacion);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

