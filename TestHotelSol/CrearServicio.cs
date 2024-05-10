using CapaModelo;
using FP._059_NAGASystems_Prod3.Controllers;
using FP._059_NAGASystems_Prod3;
using FP._059_NAGASystems_Prod3.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

public class CrearServicio : IDisposable
{
    private readonly DbContextOptions<FP_059_NAGASystems_Prod3Context> _options;
    private readonly FP_059_NAGASystems_Prod3Context _context;

    public CrearServicio()
    {
        _options = new DbContextOptionsBuilder<FP_059_NAGASystems_Prod3Context>()
            .UseInMemoryDatabase(databaseName: "TestDb_CrearServicio")
            .Options;
        _context = new FP_059_NAGASystems_Prod3Context(_options);
    }

    [Fact]
    public async Task Create_PostValidServicio_RedirectsToIndex()
    {
        // Arrange
        var servicioController = new ServicioController(_context);

        // Crear servicio
        var servicio = new Servicio
        {
            Id = 1,
            Descripcion = "Servicio de limpieza diaria de habitaciones",
            Precio = 50
        };

        // Act
        var result = await servicioController.Create(servicio);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectToActionResult.ActionName);

        var createdServicio = await _context.Servicio.FirstOrDefaultAsync(s => s.Id == servicio.Id);
        Assert.NotNull(createdServicio);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

