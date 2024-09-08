namespace Gym.Domain.Tests.Unit.Common;

public class DateTimeProviderForTest : IDateTimeProvider
{
    private DateTime _utcNow = DateTime.UtcNow;

    public DateTime UtcNow
    {
        get
        {
            return _utcNow;
        }
        set
        {
            _utcNow = value;
        }
    }
}
