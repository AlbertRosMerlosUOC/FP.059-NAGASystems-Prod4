using CapaModelo;
using FP._059_NAGASystems_Prod3.Controllers;
using FP._059_NAGASystems_Prod3;
using FP._059_NAGASystems_Prod3.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

public class CrearReservaServicio : IDisposable
{
    private readonly DbContextOptions<FP_059_NAGASystems_Prod3Context> _options;
    private readonly FP_059_NAGASystems_Prod3Context _context;

    public CrearReservaServicio()
    {
        _options = new DbContextOptionsBuilder<FP_059_NAGASystems_Prod3Context>()
            .UseInMemoryDatabase(databaseName: "TestDb_CrearReservaServicio")
            .Options;
        _context = new FP_059_NAGASystems_Prod3Context(_options);
    }

    [Fact]
    public async Task Create_ValidReservaServicio_RedirectsToIndex()
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

        // Act
        var result = await reservaServicioController.Create(reservaServicio);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectToActionResult.ActionName);

        var createdReservaServicio = await _context.ReservaServicio.FirstOrDefaultAsync();
        Assert.NotNull(createdReservaServicio); // Verificar que se creó la reserva de servicio en la base de datos
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

