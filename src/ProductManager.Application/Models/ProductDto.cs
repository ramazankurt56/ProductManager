namespace ProductManager.Application.Models;

/// <summary>
/// �r�n bilgilerini ta��yan veri transfer nesnesi (DTO).
/// </summary>
public sealed class ProductDto
{
    /// <summary>
    /// �r�n�n benzersiz kimli�i.
    /// </summary>
    public Guid Id { get; set; }

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
