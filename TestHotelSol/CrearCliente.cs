using Xunit;
using Microsoft.AspNetCore.Mvc;
using FP._059_NAGASystems_Prod3.Controllers;
using FP._059_NAGASystems_Prod3.Data;
using CapaModelo;
using Microsoft.EntityFrameworkCore;
using System;

public class ClienteControllerTests : IDisposable
{
    private readonly DbContextOptions<FP_059_NAGASystems_Prod3Context> _options;
    private readonly FP_059_NAGASystems_Prod3Context _context;

    public ClienteControllerTests()
    {
        _options = new DbContextOptionsBuilder<FP_059_NAGASystems_Prod3Context>()
            .UseInMemoryDatabase(databaseName: "TestDb_CrearCliente")
            .Options;
        _context = new FP_059_NAGASystems_Prod3Context(_options);
    }

    [Fact]
    public async Task Create_ValidClienteData_RedirectsToIndex()
    {
        // Arrange
        var clienteController = new ClienteController(_context);
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

        // Act
        var result = await clienteController.Create(cliente);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectToActionResult.ActionName);

        // Verificar que el cliente se haya añadido a la base de datos
        var clienteInDb = await _context.Cliente.FirstOrDefaultAsync(c => c.DNI == cliente.DNI);
        Assert.NotNull(clienteInDb);
        Assert.Equal("Juan", clienteInDb.Nombre);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

