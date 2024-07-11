namespace Gym.Domain;

public static class SubscriptionFactory
{
    public static Subscription CreateSubscription(SubscriptionType subscriptionType)
    {
        return new Subscription(
            type: subscriptionType);
    }
}