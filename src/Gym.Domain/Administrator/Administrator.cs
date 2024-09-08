using ErrorOr;

namespace Gym.Domain;

public class Administrator : Entity
{
    private Guid? _subscriptionId;

    public Administrator(Guid? subscription = null, Guid? id = null) : base(id)
    {
        _subscriptionId = subscription;
    }

    public static Administrator CreateAdministrator()
    {
        return new Administrator();
    }

    public ErrorOr<Success> SetSubscription(Guid newSubscriptionId) 
    {
        if (_subscriptionId == newSubscriptionId) 
        {
            return AdministratorErrors.NewSubscriptionEqualOldSubscription;
        }

        _subscriptionId = newSubscriptionId;

        return Result.Success;
    }
}
