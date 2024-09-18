using Lunavex.Result;
using MediatR;
using ProductManager.Application.Models;
using System;

namespace ProductManager.Application.Features.Products.Queries.GetAllProducts;

/// <summary>
/// Tüm ürünleri almak için kullanýlan sorgu.
/// </summary>
public sealed record GetAllProductQuery() : IRequest<Result<List<ProductDto>>>;
