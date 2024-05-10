using Microsoft.AspNetCore.Mvc;
using FP._059_NAGASystems_Prod3.Controllers;
using FP._059_NAGASystems_Prod3.Data;
using CapaModelo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Xunit;
using FP._059_NAGASystems_Prod3;

public class ReservaServiceTests : IDisposable
{
    private FP_059_NAGASystems_Prod3Context _context;

    public ReservaServiceTests()
    {
        var options = new DbContextOptionsBuilder<FP_059_NAGASystems_Prod3Context>()
            .UseInMemoryDatabase(databaseName: "BaseDeDatosParaTestIndex")
            .Options;

        _context = new FP_059_NAGASystems_Prod3Context(options);

        // Llenar la base de datos en memoria con datos de prueba
        _context.Disponibilidad.AddRange(
            new Disponibilidad { Id = 1, Habitacion = 101, Fecha = DateTime.Today.AddDays(1) },
            new Disponibilidad { Id = 2, Habitacion = 102, Fecha = DateTime.Today.AddDays(2) }
        );
        _context.SaveChanges();
    }

    [Fact]
    public async Task Index_DeberiaDevolverVistaConTodasLasDisponibilidades()
    {
        var controller = new DisponibilidadController(_context);

        // Act
        var result = await controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var modelo = Assert.IsAssignableFrom<IEnumerable<Disponibilidad>>(viewResult.Model);
        Assert.Equal(2, modelo.Count());
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

