namespace Gym.Domain;

public abstract class Entity
{
    public Guid Id { get; protected set; }

    public Entity(Guid? id = null) 
    {
        Id = id ?? Guid.NewGuid();
    }
}
