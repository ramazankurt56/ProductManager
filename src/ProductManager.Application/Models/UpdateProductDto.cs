namespace ProductManager.Application.Models;

/// <summary>
/// �r�n g�ncelleme i�lemleri i�in kullan�lan veri transfer nesnesi.
/// </summary>
public sealed class UpdateProductDto
{
    /// <summary>
    /// �r�n�n ad�.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// �r�n�n fiyat�.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// �r�n�n a��klamas�.
    /// </summary>
    public string Description { get; set; } = string.Empty;
}
