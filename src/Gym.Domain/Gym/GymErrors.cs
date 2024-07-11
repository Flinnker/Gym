using ErrorOr;

namespace Gym.Domain;

public class GymErrors
{
    public static readonly Error CannotHaveMoreGymsThanSubscritpionAllows = Error.Validation(
        code: "Subscription_001", "CannotHaveMoreGymsThanSubscritpionAllows");

    public static readonly Error SubscriptionNotHaveGyms = Error.Validation(
        code: "Subscription_002", "SubscriptionNotHaveGyms");
}
