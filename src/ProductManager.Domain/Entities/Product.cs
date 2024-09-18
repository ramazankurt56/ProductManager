using ProductManager.Domain.Abstractions;

namespace ProductManager.Domain.Entities;

/// <summary>
/// �r�n varl�k s�n�f�.
/// </summary>
public class Product : Entity
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
