using ErrorOr;

namespace Gym.Domain;

public class SubscriptionErrors
{
    public static readonly Error SubscriptionNotHaveThisGym = Error.Validation(
        code: "Participant_001", "SubscriptionNotHaveThisGym");
}
