using ErrorOr;
using Gym.Domain;

namespace Gym.Tests.Subscription;

public class SubscriptionTests
{
    [Fact]
    public void AddGym_WhenMoreThanSubscriptionAllows_ShouldFail()
    {
        // Arrange
        var subscription = SubscriptionFactory.CreateSubscription(SubscriptionType.Base);

        var gyms = Enumerable.Range(0, subscription.GetMaxGyms() + 1)
            .Select(_ => Domain.Gym.CreateGym(SubscriptionType.Base))
            .ToList();

        // Act
        var gymResults = new List<ErrorOr<Success>>();

        foreach(var gym in gyms)
        {
            gymResults.Add(subscription.AddGym(gym));
        }

        // Assert
        var allButLastGumResults = gymResults.Take(..^1);

        foreach (var allButLastGumResult in allButLastGumResults)
        {
            Assert.Equal(allButLastGumResult, Result.Success);
        }

        var lastResult = gymResults.Last();

        Assert.Equal(lastResult.FirstError, GymErrors.CannotHaveMoreGymsThanSubscritpionAllows);

    }
}
