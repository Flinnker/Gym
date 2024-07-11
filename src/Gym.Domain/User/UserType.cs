using Ardalis.SmartEnum;

namespace Gym.Domain;

public class UserType : SmartEnum<UserType>
{
    public static readonly UserType User = new(nameof(User), 0);
    public static readonly UserType Participant = new(nameof(Participant), 1);
    public static readonly UserType Trainer = new(nameof(Trainer), 2);
    public static readonly UserType Administrator = new(nameof(Administrator), 3);


    public UserType(string name, int value) : base(name, value)
    {
    }
}