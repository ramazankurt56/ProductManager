namespace ProductManager.Application.Models;

/// <summary>
/// Ürün güncelleme iþlemleri için kullanýlan veri transfer nesnesi.
/// </summary>
public sealed class UpdateProductDto
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
