using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ProductManager.Application.Abstractions;
using ProductManager.Application.Features.Products.Commands.CreateProduct;
using ProductManager.Domain.Entities;
using ProductManager.Domain.Repositories;

public class CreateProductCommandHandlerTests
{
    // Testlerde kullan�lacak sahte (mock) nesneler tan�mlan�yor.
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<CreateProductCommandHandler>> _loggerMock;
    private readonly CreateProductCommandHandler _handler;

    // Constructor (kurucu fonksiyon), testler ba�lamadan �nce her test i�in yeni bir handler ve mock nesneleri olu�turuyor.
    public CreateProductCommandHandlerTests()
    {
        // Mock nesneleri, ger�ek nesnelerin taklidi yap�l�r.
        // Bu sayede ger�ek veri taban� ya da di�er d�� ba��ml�l�klar kullan�lmadan test yap�labilir.
        _productRepositoryMock = new Mock<IProductRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<CreateProductCommandHandler>>();

        // Ger�ek kodda kullan�lan handler (i�lemci) s�n�f�n�n bir �rne�i olu�turuluyor.
        // Mock nesneler handler'a enjekte ediliyor (i�eri aktar�l�yor).
        _handler = new CreateProductCommandHandler(
            _productRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _loggerMock.Object
        );
    }

[Fact]
    public async Task Handle_ShouldReturnFailure_WhenProductWithSameNameExists()
    {
        // Arrange (Haz�rl�k a�amas�)
        // "ExistingProduct" ad�nda bir �r�n eklemeye �al��aca��z.
        var command = new CreateProductCommand("ExistingProduct", 10.99m, "Description");

        // Repository'deki GetByNameAsync metodunu ayarl�yoruz.
        // E�er bu metod "ExistingProduct" ad�nda bir �r�n ararsa, biz ona zaten b�yle bir �r�n oldu�unu s�yl�yoruz.
        _productRepositoryMock
            .Setup(x => x.GetByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(new Product { Name = "ExistingProduct" });

        // Act (Testi �al��t�r)
        // Handler'�n "Handle" metodunu �a��r�yoruz ve bu komutu ona g�nderiyoruz.
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert (Sonu�lar� kontrol et)
        // Sonu�ta i�lem ba�ar�s�z olmal� ��nk� ayn� isimde bir �r�n zaten var.
        result.IsSuccessful.Should().BeFalse();
        result.ErrorMessages.Should().Contain("A product with the name 'ExistingProduct' already exists.");

        // Logger'da bu durumun uyar� olarak loglan�p loglanmad���n� kontrol ediyoruz.
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("A product with the name")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);

        // Yeni �r�n eklemeye �al���lmamal� ��nk� zaten ayn� isimde bir �r�n var.
        _productRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Product>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }


    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenProductIsCreatedSuccessfully()
    {
        // Arrange (Haz�rl�k a�amas�)
        var command = new CreateProductCommand("NewProduct", 10.99m, "Description");
        var product = new Product { Name = "NewProduct" };

        // Veritaban�nda ayn� isimde bir �r�n olmad���n� belirtiyoruz.
        _productRepositoryMock
            .Setup(x => x.GetByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((Product)null); // Yani b�yle bir �r�n yok.

        // Mapper komut ile �r�n� d�n��t�rs�n.
        _mapperMock
            .Setup(x => x.Map<Product>(command))
            .Returns(product);

        // Act (Testi �al��t�r)
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert (Sonu�lar� kontrol et)
        // ��lem ba�ar�l� olmal� ve do�ru mesaj d�nmeli.
        result.IsSuccessful.Should().BeTrue();
        result.Data.Should().Be($"Product 'NewProduct' with ID {product.Id} created successfully.");

        // Uyar� loglanmamal�.
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Never);

        // �r�n ekleme metodunun �a�r�ld���ndan ve veritaban�na kaydedildi�inden emin ol.
        _productRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Product>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }


    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenSavingProductFails()
    {
        // Arrange (Haz�rl�k a�amas�)
        var command = new CreateProductCommand("NewProduct", 10.99m, "Description");
        var product = new Product { Name = "NewProduct" };

        // Ayn� isimde �r�n olmad���n� belirtiyoruz.
        _productRepositoryMock
            .Setup(x => x.GetByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((Product)null);

        // Komutu �r�ne d�n��t�r.
        _mapperMock
            .Setup(x => x.Map<Product>(command))
            .Returns(product);

        // Veri taban�na kaydetme i�lemi s�ras�nda hata olu�tu�unu sim�le ediyoruz.
        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act (Testi �al��t�r)
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert (Sonu�lar� kontrol et)
        // ��lem ba�ar�s�z olmal� ve hata mesaj� d�nmeli.
        result.IsSuccessful.Should().BeFalse();
        result.ErrorMessages.Should().Contain("An error occurred while creating the product: Database error");

        // �r�n eklenmeye �al���ld���n� do�ruluyoruz.
        _productRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Product>()), Times.Once);

        // Veritaban�na kaydetme i�leminin bir kez �a�r�ld���n� ama ba�ar�s�z oldu�unu kontrol ediyoruz.
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        // Logger'�n bu hata s�ras�nda bir hata logu yazd���n� kontrol ediyoruz.
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"An error occurred while saving the product {product.Name} to the database.")),
                It.IsAny<Exception>(), // Burada bir Exception bekliyoruz.
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }


}
