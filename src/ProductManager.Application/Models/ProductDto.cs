namespace ProductManager.Application.Models;

/// <summary>
/// Ürün bilgilerini taþýyan veri transfer nesnesi (DTO).
/// </summary>
public sealed class ProductDto
{
    /// <summary>
    /// Ürünün benzersiz kimliði.
    /// </summary>
    public Guid Id { get; set; }

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
