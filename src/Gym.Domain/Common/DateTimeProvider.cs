namespace Gym.Domain;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow { 
        get 
        {
            return DateTime.UtcNow;
        } 
    }
}
