using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ProductManager.Application.Abstractions;
using ProductManager.Application.Features.Products.Commands.DeleteProduct;
using ProductManager.Domain.Entities;
using ProductManager.Domain.Repositories;

public class DeleteProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<DeleteProductCommandHandler>> _loggerMock;
    private readonly DeleteProductCommandHandler _handler;

    public DeleteProductCommandHandlerTests()
    {
        // Mock nesneler oluþturuluyor
        _productRepositoryMock = new Mock<IProductRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<DeleteProductCommandHandler>>();

        // Testlerde kullanmak üzere DeleteProductCommandHandler örneði oluþturuluyor
        _handler = new DeleteProductCommandHandler(
            _productRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenProductNotFound()
    {


        // Arrange (Hazýrlýk aþamasý)
        // Silinmek istenen ürünün Guid türündeki ID'si
        var productId = Guid.NewGuid();
        var command = new DeleteProductCommand { Id = productId };

        // Ürün bulunamadýðý senaryosunu simüle ediyoruz
        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Product)null);  // Ürün bulunamadýðýnda null döner

        // Act (Testi çalýþtýr)
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert (Sonuçlarý kontrol et)
        // Ürün bulunamadýðý için iþlem baþarýsýz olmalý ve hata mesajý içermeli
        result.IsSuccessful.Should().BeFalse();
        result.ErrorMessages.Should().Contain("Product not found.");

        // Logger'ýn bu durumu uyarý olarak loglayýp loglamadýðýný kontrol ediyoruz
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Product with ID {productId} not found.")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);

        // Silme iþlemi yapýlmadýðý için DeleteAsync çaðrýlmamalý
        _productRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<Guid>()), Times.Never);

        // Deðiþikliklerin veritabanýna kaydedilmediðini doðruluyoruz
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenProductIsDeletedSuccessfully()
    {
        // Hazýrlýk (Arrange)
        var command = new DeleteProductCommand { Id = Guid.NewGuid() };

        var product = new Product
        {
            Name = "TestProduct"
            // Id dahili olarak oluþturulur ve ayarlanamaz
        };

        // Oluþturulan Id'yi al
        var generatedProductId = product.Id;

        // GetByIdAsync metodunu ürünü döndürecek þekilde ayarla
        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(product);

        // Ýþlemi gerçekleþtir (Act)
        var result = await _handler.Handle(command, CancellationToken.None);

        // Sonuçlarý doðrula (Assert)
        result.IsSuccessful.Should().BeTrue();
        result.Data.Should().Be($"Product with ID {generatedProductId} deleted successfully.");

        // DeleteAsync metodunun ürünün Id'si ile çaðrýldýðýný doðrula
        _productRepositoryMock.Verify(x => x.DeleteAsync(generatedProductId), Times.Once);

        // SaveChangesAsync metodunun bir kez çaðrýldýðýný doðrula
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }


}
