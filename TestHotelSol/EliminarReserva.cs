using CapaModelo;
using FP._059_NAGASystems_Prod3;
using FP._059_NAGASystems_Prod3.Controllers;
using FP._059_NAGASystems_Prod3.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

public class EliminarReserva : IDisposable
{
    private readonly DbContextOptions<FP_059_NAGASystems_Prod3Context> _options;
    private readonly FP_059_NAGASystems_Prod3Context _context;

    public EliminarReserva()
    {
        _options = new DbContextOptionsBuilder<FP_059_NAGASystems_Prod3Context>()
            .UseInMemoryDatabase(databaseName: "TestDb_EliminarReserva")
            .Options;
        _context = new FP_059_NAGASystems_Prod3Context(_options);
    }

    [Fact]
    public async Task DeleteConfirmed_ValidReservaId_DeletesReservaAndRedirectsToIndex()
    {
        // Arrange
        var reservaController = new ReservaController(_context);

        // Crear reserva
        var reserva = new Reserva
        {
            DNI = "12345678X",
            Habitacion = 101,
            TipoAlojamiento = 1,
            TipoTemporada = 1,
            FechaInicio = DateTime.Today,
            FechaFin = DateTime.Today.AddDays(3),
            Factura = 500,
            Referido = "Referido",
            CheckIn = 1,
            Cancelado = 0,
            Oferta = 1
        };
        _context.Reserva.Add(reserva);
        await _context.SaveChangesAsync();

        // Act
        var result = await reservaController.DeleteConfirmed(reserva.Id);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectToActionResult.ActionName);

        var deletedReserva = await _context.Reserva.FindAsync(reserva.Id);
        Assert.Null(deletedReserva);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

