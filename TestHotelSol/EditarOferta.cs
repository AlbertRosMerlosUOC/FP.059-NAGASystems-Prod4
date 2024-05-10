using CapaModelo;
using FP._059_NAGASystems_Prod3;
using FP._059_NAGASystems_Prod3.Controllers;
using FP._059_NAGASystems_Prod3.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

public class ModificarOferta : IDisposable
{
    private readonly DbContextOptions<FP_059_NAGASystems_Prod3Context> _options;

    public ModificarOferta()
    {
        _options = new DbContextOptionsBuilder<FP_059_NAGASystems_Prod3Context>()
            .UseInMemoryDatabase(databaseName: "TestDatabaseForEditOffer")
            .Options;
    }

    [Fact]
    public async Task Edit_PostValidOfferData_UpdatesOfferAndRedirectsToIndex()
    {
        // Arrange
        using (var setupContext = new FP_059_NAGASystems_Prod3Context(_options))
        {
            // Crear una oferta original
            var originalOffer = new Oferta
            {
                Id = 1,
                Descripcion = "Oferta especial",
                Coeficiente = 5
            };

            setupContext.Oferta.Add(originalOffer);
            await setupContext.SaveChangesAsync();
        }

        // Act and Assert
        using (var testContext = new FP_059_NAGASystems_Prod3Context(_options))
        {
            var controller = new OfertaController(testContext);

            // Oferta modificada
            var modifiedOffer = new Oferta
            {
                Id = 1,
                Descripcion = "Oferta especial modificada",
                Coeficiente = 10
            };

            // Act
            var result = await controller.Edit(modifiedOffer.Id, modifiedOffer);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);

            var updatedOffer = await testContext.Oferta.FindAsync(modifiedOffer.Id);
            Assert.Equal("Oferta especial modificada", updatedOffer.Descripcion);
        }
    }

    public void Dispose()
    {
        using (var context = new FP_059_NAGASystems_Prod3Context(_options))
        {
            context.Database.EnsureDeleted();
        }
    }
}