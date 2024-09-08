using ErrorOr;
using FluentAssertions;
using Gym.Domain.Tests.Unit.Common;
using Xunit.Abstractions;

namespace Gym.Domain.Tests.Unit.Shedule;

public class SheduleTests
{
    private Domain.Shedule _sut;
    private ITestOutputHelper _outputHelper;

    public SheduleTests(ITestOutputHelper outputHelper)
    {
        _sut = new Domain.Shedule();
        _outputHelper = outputHelper;
    }

    [Fact]
    public void CreateShedule_WhenUserCreateShedule_ShopuldSuccessfullyCreateSheduleInstance()
    {
        // Act
        var createSheduleResult = Domain.Shedule.CreateShedule();

        // Assert
        createSheduleResult.Should().NotBeNull();
        createSheduleResult.Should().BeOfType(typeof(Domain.Shedule));
    }

    [Fact]
    public void AddTrainingSessionAtShedule_WhenAddOverlapTrainingSession_ShouldReturnErrorAboutOverlappingTrainingSession()
    {
        // Arrange
        var sessionInfoForAddWithoutOverlap = SessionInfoFactory.CreateSessionInfo(
            startTime: new TimeOnly(9, 0, 0),
            endTime: new TimeOnly(10, 0, 0));
        var sessionInfoForAddWithOverlap = SessionInfoFactory.CreateSessionInfo(
            startTime: new TimeOnly(9, 0, 0),
            endTime: new TimeOnly(10, 0, 0));

        // Act
        var addTrainingSessionWithoutOverlapResult = _sut.AddTrainingSessionAtShedule(
            startDate: sessionInfoForAddWithoutOverlap.StartDate,
            addingTimeRange: sessionInfoForAddWithoutOverlap.TimeRange);
        var addTrainingSessionWithOverlapResult = _sut.AddTrainingSessionAtShedule(
            startDate: sessionInfoForAddWithOverlap.StartDate,
            addingTimeRange: sessionInfoForAddWithOverlap.TimeRange);

        // Assert
        addTrainingSessionWithoutOverlapResult.Value.Should().Be(Result.Success);
        addTrainingSessionWithOverlapResult.FirstError.Should().Be(SheduleError.OverlappReservedTrainingSession);
    }

    [Fact]
    public void RemoveTrainingSessionFromShedule_WhenRemoveExistTrainingSession_ShouldSuccessfullyRemoveSessionFromShedule()
    {
        // Arrange
        var sessionInfo = SessionInfoFactory.CreateSessionInfo(
            startTime: new TimeOnly(9, 0, 0),
            endTime: new TimeOnly(10, 0, 0));
        var addTrainingSessionResult = _sut.AddTrainingSessionAtShedule(
            startDate: sessionInfo.StartDate,
            addingTimeRange: sessionInfo.TimeRange);

        // Act
        var removeTrainingSessionFromSheduleResult = _sut.RemoveTrainingSessionFromShedule(
            startDate: sessionInfo.StartDate,
            addingTimeRange: sessionInfo.TimeRange);

        // Assert
        addTrainingSessionResult.Value.Should().Be(Result.Success);
        removeTrainingSessionFromSheduleResult.Value.Should().Be(Result.Success);
    }

    [Fact]
    public void RemoveTrainingSessionFromShedule_WhenRemoveNotExistTrainingSession_ShouldReturnErrorAboutTrainingSessionNotFound()
    {
        // Arrange
        var sessionInfo = SessionInfoFactory.CreateSessionInfo(
            startTime: new TimeOnly(9, 0, 0),
            endTime: new TimeOnly(10, 0, 0));

        // Act
        var removeTrainingSessionFromSheduleResult = _sut.RemoveTrainingSessionFromShedule(
            startDate: sessionInfo.StartDate,
            addingTimeRange: sessionInfo.TimeRange);

        // Assert
        removeTrainingSessionFromSheduleResult.FirstError.Should().Be(SheduleError.TrainingSessionNotFound);
    }

    [Fact]
    public void GetTimeRangeIsFree_WhenCheckFreeTimeRange_ShouldReturnThatTimeRangeIsFree()
    {
        // Arrange
        var sessionInfo = SessionInfoFactory.CreateSessionInfo(
            startTime: new TimeOnly(9, 0, 0),
            endTime: new TimeOnly(10, 0, 0));

        // Act
        var getTimeRangeIsFreeResult = _sut.GetTimeRangeIsFree(
            startDate: sessionInfo.StartDate,
            checkingTimeRange: sessionInfo.TimeRange);

        // Assert
        getTimeRangeIsFreeResult.Should().BeTrue();
    }

    [Fact]
    public void GetTimeRangeIsFree_WhenCheckNotFreeTimeRange_ShouldReturnThatTimeRangeIsNotFree()
    {
        // Arrange
        var sessionInfo = SessionInfoFactory.CreateSessionInfo(
            startTime: new TimeOnly(9, 0, 0),
            endTime: new TimeOnly(10, 0, 0));
        var addTrainingSessionResult = _sut.AddTrainingSessionAtShedule(
            startDate: sessionInfo.StartDate,
            addingTimeRange: sessionInfo.TimeRange);

        // Act
        var getTimeRangeIsFreeResult = _sut.GetTimeRangeIsFree(
            startDate: sessionInfo.StartDate,
            checkingTimeRange: sessionInfo.TimeRange);

        // Assert
        addTrainingSessionResult.Value.Should().Be(Result.Success);
        getTimeRangeIsFreeResult.Should().BeFalse();
    }
}
