
using AutoMapper;
using ProductManager.Application.Features.Products.Commands.CreateProduct;
using ProductManager.Application.Features.Products.Commands.UpdateProduct;
using ProductManager.Application.Models;
using ProductManager.Domain.Entities;

namespace ProductManager.Application.Profiles;

/// <summary>
/// AutoMapper profil sýnýfý, nesneler arasý dönüþümleri tanýmlar.
/// </summary>
public class ProductProfile : Profile
{
    /// <summary>
    /// Yeni bir örnek oluþturur ve eþleme yapýlandýrmalarýný tanýmlar.
    /// </summary>
    public ProductProfile()
    {
        // CreateProductCommand'dan Product'a dönüþüm
        CreateMap<CreateProductCommand, Product>();

        // UpdateProductCommand'dan Product'a dönüþüm
        CreateMap<UpdateProductCommand, Product>();

        // Product'tan ProductDto'ya dönüþüm
        CreateMap<Product, ProductDto>();
    }
}