using ErrorOr;
using FluentAssertions;
using Gym.Domain;

namespace Gym.Tests.Subscription;

public class SubscriptionTests
{
    [Fact]
    public void AddGym_WhenMoreThanSubscriptionAllows_ShouldReturnErrorAboutSubscriptionCanNotAddNewGymBecauseCurrentGymCountBetterOrEqualMaxGymCount()
    {
        // Arrange
        var subscription = SubscriptionFactory.CreateSubscription(SubscriptionType.Base);
        var gyms = Enumerable.Range(0, subscription.GetMaxGyms() + 1)
            .Select(_ => Domain.Gym.CreateGym(SubscriptionType.Base))
            .ToList();

        // Act
        var gymResults = gyms.Select(gym => subscription.AddGym(gym.Value.Id)).ToList();

        // Assert
        gymResults.Take(..^1).Should().AllSatisfy(allButLastGumResult => allButLastGumResult.Value.Should().Be(Result.Success));
        gymResults.Last().FirstError.Should().Be(GymErrors.CannotHaveMoreGymsThanSubscritpionAllows);
    }

    [Fact]
    public void RemoveGym_WhenRemoveNotExistGym_ShouldReturnErrorAboutGymNotExistOrGymCollectionIsEmpty()
    {
        // Arrange
        var subscription = SubscriptionFactory.CreateSubscription(SubscriptionType.Base);
        var gymId = Guid.NewGuid();

        // Act
        var removeGymWhenGymCollectionIsEmptyResult = subscription.RemoveGym(gymId);
        var addGymResult = subscription.AddGym(gymId);
        var removeNotExistGymResult = subscription.RemoveGym(Guid.NewGuid());

        // Assert
        removeGymWhenGymCollectionIsEmptyResult.FirstError.Should().Be(GymErrors.SubscriptionNotHaveGyms);
        addGymResult.Value.Should().Be(Result.Success);
        removeNotExistGymResult.FirstError.Should().Be(SubscriptionErrors.SubscriptionNotHaveThisGym);
    }

    [Fact]
    public void GetGymCount_WhenMoreThanSubscriptionAllows_ShouldFail()
    {
        // Arrange
        var subscription = SubscriptionFactory.CreateSubscription(SubscriptionType.Base);
        var gyms = Enumerable.Range(0, subscription.GetMaxGyms())
            .Select(_ => Domain.Gym.CreateGym(SubscriptionType.Base))
            .ToList();
        var addGymResults = gyms.Select(gym => subscription.AddGym(gym.Value.Id)).ToList();

        // Act
        var actualGymCount = subscription.GetGymCount();

        // Assert
        addGymResults.Should().AllSatisfy(addGymResult => addGymResult.Value.Should().Be(Result.Success));
        actualGymCount.Should().Be(subscription.GetMaxGyms());
    }
}
