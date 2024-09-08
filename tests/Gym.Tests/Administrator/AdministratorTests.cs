using ErrorOr;
using FluentAssertions;

namespace Gym.Domain.Tests.Unit.Administrator;

public class AdministratorTests
{
    private Domain.Administrator _sut;

    [Fact]
    public void CreateAdministrator_CreateAdministartor_ShouldReturnAdministartorInstance()
    {
        // Act
        var createAdministartorResult = Domain.Administrator.CreateAdministrator();

        // Assert
        createAdministartorResult.Should().NotBeNull();
        createAdministartorResult.Should().BeOfType<Domain.Administrator>();
    }

    [Fact]
    public void SetSubscription_WhenSetRightSubscription_ShouldSuccessfullSetSubscription()
    {
        // Arrange
        var subscriptionId = Guid.NewGuid();
        _sut = Domain.Administrator.CreateAdministrator();

        // Act
        var setSubscriptionResult = _sut.SetSubscription(subscriptionId);

        // Assert
        setSubscriptionResult.Value.Should().Be(Result.Success);
    }

    [Fact]
    public void SetSubscription_WhenSetTwoOrMoreIdenticalSubscription_ShouldReturnErrorAboutIdenticalSubscription()
    {
        // Arrange
        var subscriptionId = Guid.NewGuid();
        _sut = Domain.Administrator.CreateAdministrator();

        // Act
        var firstSetSubscriptionResult = _sut.SetSubscription(subscriptionId);
        var secondSetSubscriptionResult = _sut.SetSubscription(subscriptionId);

        // Assert
        firstSetSubscriptionResult.Value.Should().Be(Result.Success);
        secondSetSubscriptionResult.FirstError.Should().Be(AdministratorErrors.NewSubscriptionEqualOldSubscription);
    }
}
