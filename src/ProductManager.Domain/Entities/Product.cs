using ProductManager.Domain.Abstractions;

namespace ProductManager.Domain.Entities;

/// <summary>
/// Ürün varlýk sýnýfý.
/// </summary>
public class Product : Entity
{
    /// <summary>
    /// Ürünün adý.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Ürünün fiyatý.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Ürünün açýklamasý.
    /// </summary>
    public string Description { get; set; } = string.Empty;
}
