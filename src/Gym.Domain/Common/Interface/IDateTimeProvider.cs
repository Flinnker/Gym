namespace Gym.Domain;

public interface IDateTimeProvider
{
    public DateTime UtcNow { get; }
}
