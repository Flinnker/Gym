using ErrorOr;

namespace Gym.Domain;

public class AdministratorErrors
{
    public static readonly Error UserAlreadyHaveSubscription = Error.Validation(
        code: "Administrator_001", "UserAlreadyHaveSubscription");

    public static readonly Error UserNotHaveSubscription = Error.Validation(
        code: "Administrator_002", "UserNotHaveSubscription");

    public static readonly Error UserNotHaveRemovingSubscription = Error.Validation(
        code: "Administrator_003", "UserNotHaveRemovingSubscription");

    public static readonly Error NewSubscriptionTypeNotEnoughForCurrentNeeds = Error.Validation(
        code: "Administrator_004", "NewSubscriptionTypeNotEnoughForCurrentNeeds");

    public static readonly Error NewSubscriptionEqualOldSubscription = Error.Validation(
        code: "Administrator_005", "NewSubscriptionEqualOldSubscription");
}
