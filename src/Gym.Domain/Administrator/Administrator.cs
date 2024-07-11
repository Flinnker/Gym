using ErrorOr;

namespace Gym.Domain;

public class Administrator : Entity
{
    private Subscription? _subscription;

    public Administrator(Subscription? subscription = null, Guid? id = null) : base(id)
    {
        _subscription = subscription;
    }

    public static Administrator CreateAdministrator()
    {
        return new Administrator();
    }

    public ErrorOr<Success> SetSubscription(Subscription newSubscription) 
    {
        if (_subscription is not null && _subscription.Id == newSubscription.Id) 
        {
            return AdministratorErrors.NewSubscriptionEqualOldSubscription;
        }

        _subscription = newSubscription;

        return Result.Success;
    }
}
