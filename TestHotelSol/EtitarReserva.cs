using CapaModelo;
using FP._059_NAGASystems_Prod3;
using FP._059_NAGASystems_Prod3.Controllers;
using FP._059_NAGASystems_Prod3.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

public class EditarReserva : IDisposable
{
    private readonly DbContextOptions<FP_059_NAGASystems_Prod3Context> _options;
    private readonly FP_059_NAGASystems_Prod3Context _context;

    public EditarReserva()
    {
        _options = new DbContextOptionsBuilder<FP_059_NAGASystems_Prod3Context>()
            .UseInMemoryDatabase(databaseName: "TestDb_EditarReserva")
            .Options;
        _context = new FP_059_NAGASystems_Prod3Context(_options);
    }

    [Fact]
    public async Task Edit_PostValidReservaData_UpdatesReservaAndRedirectsToIndex()
    {
        // Arrange
        var reservaController = new ReservaController(_context);

        // Crear cliente
        var cliente = new Cliente
        {
            DNI = "12345678X",
            Nombre = "Juan",
            Apellido1 = "Pérez",
            Apellido2 = "García",
            Direccion = "Calle Falsa 123",
            Email = "juan@example.com",
            Telefono = "900123456",
            VIP = 1,
            Estado = 1
        };
        _context.Cliente.Add(cliente);
        await _context.SaveChangesAsync();

        // Crear habitación
        var habitacion = new Habitacion
        {
            Numero = 101,
            TipoHabitacion = 1,
            Estado = 1
        };
        _context.Habitacion.Add(habitacion);
        await _context.SaveChangesAsync();

        // Crear tipo de alojamiento
        var tipoAlojamiento = new TipoAlojamiento
        {
            Descripcion = "Apartamento",
            Precio = 100
        };
        _context.TipoAlojamiento.Add(tipoAlojamiento);
        await _context.SaveChangesAsync();

        // Crear tipo de temporada
        var tipoTemporada = new TipoTemporada
        {
            Descripcion = "Temporada alta",
            Coeficiente = 1.5
        };
        _context.TipoTemporada.Add(tipoTemporada);
        await _context.SaveChangesAsync();

        // Crear reserva original
        var reservaOriginal = new Reserva
        {
            DNI = cliente.DNI,
            Habitacion = habitacion.Numero,
            TipoAlojamiento = tipoAlojamiento.Id,
            TipoTemporada = tipoTemporada.Id,
            FechaInicio = DateTime.Today,
            FechaFin = DateTime.Today.AddDays(3),
            Factura = 500,
            Referido = "Referido",
            CheckIn = 1,
            Cancelado = 0,
            Oferta = 1
        };
        _context.Reserva.Add(reservaOriginal);
        await _context.SaveChangesAsync();

        // Modificar la reserva original
        reservaOriginal.Factura = 600;
        reservaOriginal.Referido = "Referido modificado";

        // Act
        var result = await reservaController.Edit(reservaOriginal.Id, reservaOriginal);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectToActionResult.ActionName);

        var updatedReserva = await _context.Reserva.FindAsync(reservaOriginal.Id);
        Assert.Equal(600, updatedReserva.Factura);
        Assert.Equal("Referido modificado", updatedReserva.Referido);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
