using CapaModelo;
using FP._059_NAGASystems_Prod3.Controllers;
using FP._059_NAGASystems_Prod3.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

public class EliminarCliente : IDisposable
{
    private FP_059_NAGASystems_Prod3Context _context;

    public EliminarCliente()
    {
        var options = new DbContextOptionsBuilder<FP_059_NAGASystems_Prod3Context>()
            .UseInMemoryDatabase(databaseName: "TestDatabaseForDelete")
            .Options;

        _context = new FP_059_NAGASystems_Prod3Context(options);

        // Insert test data into the in-memory database
        _context.Cliente.Add(new Cliente
        {
            DNI = "87654321X",
            Nombre = "Ana",
            Apellido1 = "López",
            Apellido2 = "Martínez",
            Direccion = "Avenida Real 321",
            Email = "ana@example.com",
            Telefono = "900123457",
            VIP = 1,
            Estado = 1
        });
        _context.SaveChanges();
    }

    [Fact]
    public async Task DeleteConfirmed_ValidClienteId_DeletesClienteAndRedirectsToIndex()
    {
        var controller = new ClienteController(_context);

        // Act
        var result = await controller.DeleteConfirmed("87654321X");

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectToActionResult.ActionName);
        var deletedCliente = await _context.Cliente.FindAsync("87654321X");
        Assert.Null(deletedCliente);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
