using ErrorOr;
using FluentAssertions;
using Gym.Domain.Tests.Unit.Common;
using Xunit.Abstractions;

namespace Gym.Domain.Tests.Unit.Participant;

public class ParticipantTests
{
    private Domain.Participant _sut;
    private ITestOutputHelper _outputHelper;

    public ParticipantTests(ITestOutputHelper outputHelper)
    {
        _sut = new Domain.Participant();
        _outputHelper = outputHelper;
    }

    [Fact]
    public void CreateParticipant_WhenCreateParticipant_ShouldReturnParticipantInstance()
    {
        // Act
        var createParticipantResult = Domain.Participant.CreateParticipant();

        // Assert
        createParticipantResult.Should().NotBeNull();
        createParticipantResult.Should().BeOfType(typeof(Domain.Participant));
    }

    [Fact]
    public void GetParticipantTrainingSessionsId_WhenTrainingSessionIsNull_ShouldReturnZeroTraningSession()
    {
        // Act
        var getParticipantTrainingSessionResult = _sut.GetParticipantTrainingSessionsId();

        // Assert
        getParticipantTrainingSessionResult.Should().NotBeNull();
        getParticipantTrainingSessionResult.Should().HaveCount(0);
    }

    [Fact]
    public void ReserveTrainingSession_WhenReserveOverlapTrainingSession_ShoulReturnErrorAboutTrainingSessionHaveOverlap()
    {
        // Arrange
        var sessionTimeForSuccesfullyReserveTime = SessionInfoFactory.CreateSessionInfo(
            startTime: new TimeOnly(9, 0, 0),
            endTime: new TimeOnly(10, 0, 0));
        var sessionTimeForReserveTimeWithOverlap = SessionInfoFactory.CreateSessionInfo(
            startTime: new TimeOnly(9, 0, 0),
            endTime: new TimeOnly(10, 0, 0));

        // Act
        var reserveTrainingSessionWithoutOverlapResult = _sut.ReserveTrainingSession(
            startDate: sessionTimeForSuccesfullyReserveTime.StartDate, 
            timeRange: sessionTimeForSuccesfullyReserveTime.TimeRange);
        var reserveTrainingSessionWithOverlapResult = _sut.ReserveTrainingSession(
            startDate: sessionTimeForSuccesfullyReserveTime.StartDate,
            timeRange: sessionTimeForSuccesfullyReserveTime.TimeRange);

        // Assert
        reserveTrainingSessionWithoutOverlapResult.Value.Should().Be(Result.Success);
        reserveTrainingSessionWithOverlapResult.FirstError.Should().Be(ParticipantErrors.TrainingSessionOverlappingReservedSession);
    }

    [Fact]
    public void CancelTrainingSession_WhenUserCancelExistTrainingSession_ShouldSuccessfullyRemoveTrainingSession()
    {
        // Arrange
        var sessionInfo = SessionInfoFactory.CreateSessionInfo(
            startTime: new TimeOnly(9, 0, 0),
            endTime: new TimeOnly(10, 0, 0));
        var reserveTrainingSessionResult = _sut.ReserveTrainingSession(
            startDate: sessionInfo.StartDate,
            timeRange: sessionInfo.TimeRange);

        // Act
        var cancelTrainingSessionResult = _sut.CancelTrainingSession(
            startDate: sessionInfo.StartDate,
            timeRange: sessionInfo.TimeRange);

        // Assert
        reserveTrainingSessionResult.Value.Should().Be(Result.Success);
        cancelTrainingSessionResult.Value.Should().Be(Result.Success);
    }

    [Fact]
    public void CancelTrainingSession_WhenUserCancelNotExistTrainingSession_ShouldReturnErrorAboutSessionNotFound()
    {
        // Arrange
        var sessionInfoForReserve = SessionInfoFactory.CreateSessionInfo(
            startTime: new TimeOnly(9, 0, 0),
            endTime: new TimeOnly(10, 0, 0));
        var sessionInfoForRemove = SessionInfoFactory.CreateSessionInfo(
            startTime: new TimeOnly(10, 0, 0),
            endTime: new TimeOnly(11, 0, 0));
        var reserveTrainingSessionResult = _sut.ReserveTrainingSession(
            startDate: sessionInfoForReserve.StartDate,
            timeRange: sessionInfoForReserve.TimeRange);

        // Act
        var cancelTrainingSessionResult = _sut.CancelTrainingSession(
            startDate: sessionInfoForRemove.StartDate,
            timeRange: sessionInfoForRemove.TimeRange);

        // Assert
        reserveTrainingSessionResult.Value.Should().Be(Result.Success);
        cancelTrainingSessionResult.FirstError.Should().Be(SheduleError.TrainingSessionNotFound);
    }

    [Fact]
    public void CancelTrainingSession_WhenTrainingSessionCollectionIsEmpty_ShouldReturnErrorAboutTrainingSessionNotFound()
    {
        // Arrange
        var sessionInfo = SessionInfoFactory.CreateSessionInfo(
            startTime: new TimeOnly(9, 0, 0),
            endTime: new TimeOnly(10, 0, 0));

        // Act
        var cancelTrainingSessionResult = _sut.CancelTrainingSession(
            startDate: sessionInfo.StartDate,
            timeRange: sessionInfo.TimeRange);

        // Assert
        cancelTrainingSessionResult.FirstError.Should().Be(SheduleError.TrainingSessionNotFound);
    }
}
