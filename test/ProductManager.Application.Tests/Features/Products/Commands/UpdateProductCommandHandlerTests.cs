using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ProductManager.Application.Abstractions;
using ProductManager.Application.Features.Products.Commands.UpdateProduct;
using ProductManager.Domain.Entities;
using ProductManager.Domain.Repositories;

// Test sýnýfý: UpdateProductCommandHandler için testler
public class UpdateProductCommandHandlerTests
{
    // Mock nesneler: Testlerde kullanýlan sahte (mock) nesneler.
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<UpdateProductCommandHandler>> _loggerMock;
    private readonly UpdateProductCommandHandler _handler;

    // Constructor (yapýcý metod): Test sýnýfý baþlatýlýrken mock nesneler oluþturuluyor
    // ve handler (iþlemci) örneði bu mock nesnelerle çalýþacak þekilde ayarlanýyor.
    public UpdateProductCommandHandlerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<UpdateProductCommandHandler>>();

        // UpdateProductCommandHandler sýnýfýnýn bir örneði oluþturuluyor.
        _handler = new UpdateProductCommandHandler(
            _productRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _loggerMock.Object
        );
    }

    // Test: Eðer ürün bulunamazsa ne olur?
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenProductNotFound()
    {
        // Arrange (Hazýrlýk aþamasý)
        var command = new UpdateProductCommand(Guid.NewGuid(), "UpdatedProduct", 19.99m, "Updated description");

        // Repository'deki GetByIdAsync metodu ayarlanýyor.
        // Bu metod çaðrýldýðýnda null dönecek (yani ürün bulunamýyor).
        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Product)null);  // Ürün bulunamýyor

        // Act (Ýþlemi çalýþtýr)
        // Handler'ýn Handle metodu çaðrýlýyor ve ürün güncelleme iþlemi baþlatýlýyor.
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert (Sonucu kontrol et)
        // Ýþlem baþarýsýz olmalý, çünkü ürün bulunamadý.
        result.IsSuccessful.Should().BeFalse();
        result.ErrorMessages.Should().Contain("Product not found.");

        // Logger'ýn "Product not found" mesajýný kaydedip kaydetmediðini kontrol ediyoruz.
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Product with ID {command.Id} not found.")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);  // Logger'ýn bu uyarýyý sadece bir kere kaydetmesi bekleniyor.

        // Ürün bulunamadýðý için UpdateAsync metodunun çaðrýlmadýðýný doðruluyoruz.
        _productRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Product>()), Times.Never);

        // Deðiþikliklerin veritabanýna kaydedilmediðini doðruluyoruz.
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    // Test: Eðer ürün baþarýyla güncellenirse ne olur?
    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenProductIsUpdatedSuccessfully()
    {
        // Arrange (Hazýrlýk aþamasý)
        var command = new UpdateProductCommand(Guid.NewGuid(), "UpdatedProduct", 19.99m, "Updated description");

        // Mevcut bir ürün oluþturuluyor (eski bilgilerle).
        var product = new Product { Name = "OldProduct", Price = 10.99m, Description = "Old description" };

        // Repository'deki GetByIdAsync metodu ayarlanýyor. Bu metod çaðrýldýðýnda ürün bulunuyor.
        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(product);  // Ürün bulundu

        // Mapper ayarlanýyor. Komuttaki yeni bilgiler ile mevcut ürün güncelleniyor.
        _mapperMock
            .Setup(x => x.Map(command, product));  // Mapper kullanýlarak ürün güncelleniyor

        // Act (Ýþlemi çalýþtýr)
        // Handler'ýn Handle metodu çaðrýlýyor ve ürün güncelleme iþlemi baþlatýlýyor.
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert (Sonucu kontrol et)
        // Ýþlem baþarýlý olmalý, çünkü ürün güncellendi.
        result.IsSuccessful.Should().BeTrue();
        result.Data.Should().Be($"Product with ID {product.Id} updated successfully.");

        // Ürün güncelleme iþleminin çaðrýldýðýný doðruluyoruz.
        _productRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Product>()), Times.Once);

        // Deðiþikliklerin veritabanýna kaydedildiðini doðruluyoruz.
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenSavingProductFails()
    {
        // Arrange
        var command = new UpdateProductCommand(Guid.NewGuid(), "UpdatedProduct", 19.99m, "Updated description");
        var product = new Product { Name = "OldProduct", Price = 10.99m, Description = "Old description" };

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(product);  // Ürün bulundu

        _mapperMock
            .Setup(x => x.Map(command, product));  // Ürün mapper ile güncelleniyor

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));  // Veritabaný hatasý

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.ErrorMessages.Should().Contain("An error occurred while updating the product: Database error");

        _productRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Product>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

}
