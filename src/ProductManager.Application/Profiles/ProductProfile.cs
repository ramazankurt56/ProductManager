
using AutoMapper;
using ProductManager.Application.Features.Products.Commands.CreateProduct;
using ProductManager.Application.Features.Products.Commands.UpdateProduct;
using ProductManager.Application.Models;
using ProductManager.Domain.Entities;

namespace ProductManager.Application.Profiles;

/// <summary>
/// AutoMapper profil s�n�f�, nesneler aras� d�n���mleri tan�mlar.
/// </summary>
public class ProductProfile : Profile
{
    /// <summary>
    /// Yeni bir �rnek olu�turur ve e�leme yap�land�rmalar�n� tan�mlar.
    /// </summary>
    public ProductProfile()
    {
        // CreateProductCommand'dan Product'a d�n���m
        CreateMap<CreateProductCommand, Product>();

        // UpdateProductCommand'dan Product'a d�n���m
        CreateMap<UpdateProductCommand, Product>();

        // Product'tan ProductDto'ya d�n���m
        CreateMap<Product, ProductDto>();
    }
}