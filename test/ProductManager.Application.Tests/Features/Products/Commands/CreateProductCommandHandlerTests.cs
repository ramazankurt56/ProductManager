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
    // Testlerde kullanýlacak sahte (mock) nesneler tanýmlanýyor.
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<CreateProductCommandHandler>> _loggerMock;
    private readonly CreateProductCommandHandler _handler;

    // Constructor (kurucu fonksiyon), testler baþlamadan önce her test için yeni bir handler ve mock nesneleri oluþturuyor.
    public CreateProductCommandHandlerTests()
    {
        // Mock nesneleri, gerçek nesnelerin taklidi yapýlýr.
        // Bu sayede gerçek veri tabaný ya da diðer dýþ baðýmlýlýklar kullanýlmadan test yapýlabilir.
        _productRepositoryMock = new Mock<IProductRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<CreateProductCommandHandler>>();

        // Gerçek kodda kullanýlan handler (iþlemci) sýnýfýnýn bir örneði oluþturuluyor.
        // Mock nesneler handler'a enjekte ediliyor (içeri aktarýlýyor).
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
        // Arrange (Hazýrlýk aþamasý)
        // "ExistingProduct" adýnda bir ürün eklemeye çalýþacaðýz.
        var command = new CreateProductCommand("ExistingProduct", 10.99m, "Description");

        // Repository'deki GetByNameAsync metodunu ayarlýyoruz.
        // Eðer bu metod "ExistingProduct" adýnda bir ürün ararsa, biz ona zaten böyle bir ürün olduðunu söylüyoruz.
        _productRepositoryMock
            .Setup(x => x.GetByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(new Product { Name = "ExistingProduct" });

        // Act (Testi çalýþtýr)
        // Handler'ýn "Handle" metodunu çaðýrýyoruz ve bu komutu ona gönderiyoruz.
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert (Sonuçlarý kontrol et)
        // Sonuçta iþlem baþarýsýz olmalý çünkü ayný isimde bir ürün zaten var.
        result.IsSuccessful.Should().BeFalse();
        result.ErrorMessages.Should().Contain("A product with the name 'ExistingProduct' already exists.");

        // Logger'da bu durumun uyarý olarak loglanýp loglanmadýðýný kontrol ediyoruz.
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("A product with the name")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);

        // Yeni ürün eklemeye çalýþýlmamalý çünkü zaten ayný isimde bir ürün var.
        _productRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Product>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }


    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenProductIsCreatedSuccessfully()
    {
        // Arrange (Hazýrlýk aþamasý)
        var command = new CreateProductCommand("NewProduct", 10.99m, "Description");
        var product = new Product { Name = "NewProduct" };

        // Veritabanýnda ayný isimde bir ürün olmadýðýný belirtiyoruz.
        _productRepositoryMock
            .Setup(x => x.GetByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((Product)null); // Yani böyle bir ürün yok.

        // Mapper komut ile ürünü dönüþtürsün.
        _mapperMock
            .Setup(x => x.Map<Product>(command))
            .Returns(product);

        // Act (Testi çalýþtýr)
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert (Sonuçlarý kontrol et)
        // Ýþlem baþarýlý olmalý ve doðru mesaj dönmeli.
        result.IsSuccessful.Should().BeTrue();
        result.Data.Should().Be($"Product 'NewProduct' with ID {product.Id} created successfully.");

        // Uyarý loglanmamalý.
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Never);

        // Ürün ekleme metodunun çaðrýldýðýndan ve veritabanýna kaydedildiðinden emin ol.
        _productRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Product>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }


    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenSavingProductFails()
    {
        // Arrange (Hazýrlýk aþamasý)
        var command = new CreateProductCommand("NewProduct", 10.99m, "Description");
        var product = new Product { Name = "NewProduct" };

        // Ayný isimde ürün olmadýðýný belirtiyoruz.
        _productRepositoryMock
            .Setup(x => x.GetByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((Product)null);

        // Komutu ürüne dönüþtür.
        _mapperMock
            .Setup(x => x.Map<Product>(command))
            .Returns(product);

        // Veri tabanýna kaydetme iþlemi sýrasýnda hata oluþtuðunu simüle ediyoruz.
        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act (Testi çalýþtýr)
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert (Sonuçlarý kontrol et)
        // Ýþlem baþarýsýz olmalý ve hata mesajý dönmeli.
        result.IsSuccessful.Should().BeFalse();
        result.ErrorMessages.Should().Contain("An error occurred while creating the product: Database error");

        // Ürün eklenmeye çalýþýldýðýný doðruluyoruz.
        _productRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Product>()), Times.Once);

        // Veritabanýna kaydetme iþleminin bir kez çaðrýldýðýný ama baþarýsýz olduðunu kontrol ediyoruz.
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        // Logger'ýn bu hata sýrasýnda bir hata logu yazdýðýný kontrol ediyoruz.
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
