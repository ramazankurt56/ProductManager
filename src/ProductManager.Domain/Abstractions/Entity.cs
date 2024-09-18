namespace ProductManager.Domain.Abstractions;

/// <summary>
/// T�m varl�klar�n temel s�n�f�.
/// </summary>
public class Entity
{
    /// <summary>
    /// Varl���n benzersiz kimli�i.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Belirtilen kimlikle yeni bir varl�k olu�turur.
    /// </summary>
    /// <param name="id">Varl���n kimli�i.</param>
    public Entity(Guid id)
    {
        Id = id;
    }

    /// <summary>
    /// Yeni bir varl�k olu�turur ve benzersiz bir kimlik atar.
    /// </summary>
    public Entity()
    {
        Id = Guid.NewGuid();
    }
}
