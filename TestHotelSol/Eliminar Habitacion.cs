using Microsoft.AspNetCore.Mvc;
using FP._059_NAGASystems_Prod3.Controllers;
using FP._059_NAGASystems_Prod3.Data;
using CapaModelo;
using Microsoft.EntityFrameworkCore;
using System;

public class EliminarHabitacion : IDisposable
{
    private FP_059_NAGASystems_Prod3Context _context;

    public EliminarHabitacion()
    {
        var options = new DbContextOptionsBuilder<FP_059_NAGASystems_Prod3Context>()
            .UseInMemoryDatabase(databaseName: "TestDb_DeleteHabitacion")
            .Options;

        _context = new FP_059_NAGASystems_Prod3Context(options);

        // Insertar datos de prueba en la base de datos en memoria
        _context.Habitacion.Add(new Habitacion { Numero = 103, TipoHabitacion = 1, Estado = 1 });
        _context.SaveChanges();
    }

    [Fact]
    public async Task DeleteConfirmed_ValidHabitacionId_DeletesHabitacionAndRedirectsToIndex()
    {
        var controller = new HabitacionController(_context);

        // Act
        var result = await controller.DeleteConfirmed(103);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectToActionResult.ActionName);
        Assert.Equal(0, await _context.Habitacion.CountAsync());
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

