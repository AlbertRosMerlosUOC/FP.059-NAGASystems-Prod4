using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using FP._059_NAGASystems_Prod3.Controllers;
using FP._059_NAGASystems_Prod3.Data;
using CapaModelo;
using FP._059_NAGASystems_Prod3;




public class OfertaControllerTests : IDisposable
{
    private readonly DbContextOptions<FP_059_NAGASystems_Prod3Context> _options;

    public OfertaControllerTests()
    {
        _options = new DbContextOptionsBuilder<FP_059_NAGASystems_Prod3Context>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        using (var context = new FP_059_NAGASystems_Prod3Context(_options))
        {
            context.Database.EnsureCreated();
        }
    }

    public void Dispose()
    {
        using (var context = new FP_059_NAGASystems_Prod3Context(_options))
        {
            context.Database.EnsureDeleted();
        }
    }

    [Fact]
    public async Task Create_PostValidOferta_RedirectsToIndex()
    {
        // Arrange
        using (var context = new FP_059_NAGASystems_Prod3Context(_options))
        {
            var controller = new OfertaController(context);
            var oferta = new Oferta
            {
                Descripcion = "Oferta de prueba",
                Coeficiente = 0.8 // Ajusta el coeficiente según sea necesario
            };

            // Act
            var result = await controller.Create(oferta);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);

            // Verifica que la oferta se haya agregado a la base de datos en memoria
            Assert.Equal(1, await context.Oferta.CountAsync());
            var ofertaInDb = await context.Oferta.FirstOrDefaultAsync(o => o.Descripcion == "Oferta de prueba");
            Assert.NotNull(ofertaInDb);
            Assert.Equal("Oferta de prueba", ofertaInDb.Descripcion);
            // Asegúrate de que el coeficiente se haya guardado correctamente
            Assert.Equal(0.8, ofertaInDb.Coeficiente);
        }
    }
}
