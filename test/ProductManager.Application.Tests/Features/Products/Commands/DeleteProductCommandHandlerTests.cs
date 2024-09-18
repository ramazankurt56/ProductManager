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
        // Mock nesneler olu�turuluyor
        _productRepositoryMock = new Mock<IProductRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<DeleteProductCommandHandler>>();

        // Testlerde kullanmak �zere DeleteProductCommandHandler �rne�i olu�turuluyor
        _handler = new DeleteProductCommandHandler(
            _productRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenProductNotFound()
    {


        // Arrange (Haz�rl�k a�amas�)
        // Silinmek istenen �r�n�n Guid t�r�ndeki ID'si
        var productId = Guid.NewGuid();
        var command = new DeleteProductCommand { Id = productId };

        // �r�n bulunamad��� senaryosunu sim�le ediyoruz
        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Product)null);  // �r�n bulunamad���nda null d�ner

        // Act (Testi �al��t�r)
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert (Sonu�lar� kontrol et)
        // �r�n bulunamad��� i�in i�lem ba�ar�s�z olmal� ve hata mesaj� i�ermeli
        result.IsSuccessful.Should().BeFalse();
        result.ErrorMessages.Should().Contain("Product not found.");

        // Logger'�n bu durumu uyar� olarak loglay�p loglamad���n� kontrol ediyoruz
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Product with ID {productId} not found.")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);

        // Silme i�lemi yap�lmad��� i�in DeleteAsync �a�r�lmamal�
        _productRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<Guid>()), Times.Never);

        // De�i�ikliklerin veritaban�na kaydedilmedi�ini do�ruluyoruz
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenProductIsDeletedSuccessfully()
    {
        // Haz�rl�k (Arrange)
        var command = new DeleteProductCommand { Id = Guid.NewGuid() };

        var product = new Product
        {
            Name = "TestProduct"
            // Id dahili olarak olu�turulur ve ayarlanamaz
        };

        // Olu�turulan Id'yi al
        var generatedProductId = product.Id;

        // GetByIdAsync metodunu �r�n� d�nd�recek �ekilde ayarla
        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(product);

        // ��lemi ger�ekle�tir (Act)
        var result = await _handler.Handle(command, CancellationToken.None);

        // Sonu�lar� do�rula (Assert)
        result.IsSuccessful.Should().BeTrue();
        result.Data.Should().Be($"Product with ID {generatedProductId} deleted successfully.");

        // DeleteAsync metodunun �r�n�n Id'si ile �a�r�ld���n� do�rula
        _productRepositoryMock.Verify(x => x.DeleteAsync(generatedProductId), Times.Once);

        // SaveChangesAsync metodunun bir kez �a�r�ld���n� do�rula
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }


}
