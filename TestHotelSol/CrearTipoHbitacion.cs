using CapaModelo;
using FP._059_NAGASystems_Prod3.Controllers;
using FP._059_NAGASystems_Prod3;
using FP._059_NAGASystems_Prod3.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

public class CrearTipoHabitacion : IDisposable
{
    private readonly DbContextOptions<FP_059_NAGASystems_Prod3Context> _options;
    private readonly FP_059_NAGASystems_Prod3Context _context;

    public CrearTipoHabitacion()
    {
        _options = new DbContextOptionsBuilder<FP_059_NAGASystems_Prod3Context>()
            .UseInMemoryDatabase(databaseName: "TestDb_CrearTipoHabitacion")
            .Options;
        _context = new FP_059_NAGASystems_Prod3Context(_options);
    }

    [Fact]
    public async Task Create_ValidTipoHabitacion_RedirectsToIndex()
    {
        // Arrange
        var tipoHabitacionController = new TipoHabitacionController(_context);

        // Crear un TipoHabitacion
        var tipoHabitacion = new TipoHabitacion
        {
            Id = 1,
            Descripcion = "Estandar",
            Precio = 100
        };

        // Act
        var result = await tipoHabitacionController.Create(tipoHabitacion);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectToActionResult.ActionName);

        // Verificar que el TipoHabitacion se haya agregado correctamente a la base de datos
        var tipoHabitacionInDb = await _context.TipoHabitacion.FirstOrDefaultAsync(th => th.Descripcion == tipoHabitacion.Descripcion);
        Assert.NotNull(tipoHabitacionInDb);
        Assert.Equal(1, tipoHabitacionInDb.Id);
        Assert.Equal("Estandar", tipoHabitacionInDb.Descripcion);
        Assert.Equal(100, tipoHabitacionInDb.Precio);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

