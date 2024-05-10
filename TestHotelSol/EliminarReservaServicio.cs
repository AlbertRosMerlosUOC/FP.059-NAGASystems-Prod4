using CapaModelo;
using FP._059_NAGASystems_Prod3.Controllers;
using FP._059_NAGASystems_Prod3;
using FP._059_NAGASystems_Prod3.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

public class EliminarReservaServicio : IDisposable
{
    private readonly DbContextOptions<FP_059_NAGASystems_Prod3Context> _options;
    private readonly FP_059_NAGASystems_Prod3Context _context;

    public EliminarReservaServicio()
    {
        _options = new DbContextOptionsBuilder<FP_059_NAGASystems_Prod3Context>()
            .UseInMemoryDatabase(databaseName: "TestDb_EliminarReservaServicio")
            .Options;
        _context = new FP_059_NAGASystems_Prod3Context(_options);
    }

    [Fact]
    public async Task Delete_ValidReservaServicioId_DeletesReservaServicioAndRedirectsToIndex()
    {
        // Arrange
        var reservaServicioController = new ReservaServicioController(_context);

        // Crear reserva de servicio
        var reservaServicio = new ReservaServicio
        {
            Id = 1,
            Reserva = 1,
            Servicio = 1,
            Fecha = DateTime.Today
        };
        _context.ReservaServicio.Add(reservaServicio);
        await _context.SaveChangesAsync();

        // Act
        var result = await reservaServicioController.DeleteConfirmed(reservaServicio.Id);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectToActionResult.ActionName);

        var deletedReservaServicio = await _context.ReservaServicio.FindAsync(reservaServicio.Id);
        Assert.Null(deletedReservaServicio);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
