namespace ProductManager.Domain.Abstractions;

/// <summary>
/// Tüm varlýklarýn temel sýnýfý.
/// </summary>
public class Entity
{
    /// <summary>
    /// Varlýðýn benzersiz kimliði.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Belirtilen kimlikle yeni bir varlýk oluþturur.
    /// </summary>
    /// <param name="id">Varlýðýn kimliði.</param>
    public Entity(Guid id)
    {
        Id = id;
    }

    /// <summary>
    /// Yeni bir varlýk oluþturur ve benzersiz bir kimlik atar.
    /// </summary>
    public Entity()
    {
        Id = Guid.NewGuid();
    }
}
