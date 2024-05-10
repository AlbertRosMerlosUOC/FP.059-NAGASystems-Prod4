using Xunit;
using Microsoft.AspNetCore.Mvc;
using FP._059_NAGASystems_Prod3.Controllers;
using FP._059_NAGASystems_Prod3.Data;
using CapaModelo;
using Microsoft.EntityFrameworkCore;
using System;

public class HabitacionControllerTests : IDisposable
{
    private readonly DbContextOptions<FP_059_NAGASystems_Prod3Context> _options;

    public HabitacionControllerTests()
    {
        // Configurar las opciones para usar una base de datos en memoria
        _options = new DbContextOptionsBuilder<FP_059_NAGASystems_Prod3Context>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task Create_PostValidHabitacion_RedirectsToIndex()
    {
        // Arrange
        using (var context = new FP_059_NAGASystems_Prod3Context(_options))
        {
            var controller = new HabitacionController(context);
            var nuevaHabitacion = new Habitacion { Numero = 101, TipoHabitacion = 1, Estado = 1 };

            // Act
            var result = await controller.Create(nuevaHabitacion);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal(1, await context.Habitacion.CountAsync());
        }
    }

    public void Dispose()
    {
        // Limpiar la base de datos en memoria después de que se completen las pruebas
        using (var context = new FP_059_NAGASystems_Prod3Context(_options))
        {
            context.Database.EnsureDeleted();
        }
    }
}
