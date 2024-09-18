using Lunavex.Result;
using MediatR;
using ProductManager.Application.Models;
using System;

namespace ProductManager.Application.Features.Products.Queries.GetAllProducts;

/// <summary>
/// T�m �r�nleri almak i�in kullan�lan sorgu.
/// </summary>
public sealed record GetAllProductQuery() : IRequest<Result<List<ProductDto>>>;
