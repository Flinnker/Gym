using Ardalis.SmartEnum;

namespace Gym.Domain;

public class SubscriptionType : SmartEnum<SubscriptionType>
{
    public static readonly SubscriptionType Free = new(
        maxGymCount: 1,
        maxGymRoomCount: 1,
        maxDailySessionCount: 4,
        price: 0,
        name: nameof(Free),
        value: 0);
    public static readonly SubscriptionType Base = new(
        maxGymCount: 1,
        maxGymRoomCount: 3,
        maxDailySessionCount: int.MaxValue,
        price: 299,
        name: nameof(Base),
        value: 1);
    public static readonly SubscriptionType Pro = new(
        maxGymCount: 3,
        maxGymRoomCount: int.MaxValue,
        maxDailySessionCount: int.MaxValue,
        price: 599,
        name: nameof(Pro),
        value: 2);

    public int MaxGymCount { get; private set; }
    public int MaxGymRoomCount { get; private set; }
    public int MaxDailySessionCount { get; private set; }
    public int Price { get; private set; }

    public SubscriptionType(
        int maxGymCount,
        int maxGymRoomCount,
        int maxDailySessionCount,
        int price,
        string name,
        int value) : base(name, value)
    {
        MaxGymCount = maxGymCount;
        MaxGymRoomCount = maxGymRoomCount;
        MaxDailySessionCount = maxDailySessionCount;
        Price = price;
    }
}
