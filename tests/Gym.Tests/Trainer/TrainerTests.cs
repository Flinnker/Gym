using ErrorOr;
using Gym.Domain;
using FluentAssertions;
using Xunit.Abstractions;
using Gym.Domain.Tests.Unit.Common;

namespace Gym.Tests.Trainer;

public class TrainerTests
{
    private Domain.Trainer _sut;

    public TrainerTests(ITestOutputHelper outputHelper) 
    {
        _sut = Domain.Trainer.CreateTrainer();
        outputHelper.WriteLine("Recreate trainer");
    }

    [Fact]
    public void AddSessionAtShedule_WhenBusyScheduleWithoutIntersection_ShouldAddSessions()
    {
        // Arrange
        var sessions = new List<SessionInfo>() 
        {
            SessionInfoFactory.CreateSessionInfo(
                startTime: new TimeOnly(9, 0, 0),
                endTime: new TimeOnly(10, 0, 0)),
            SessionInfoFactory.CreateSessionInfo(
                startTime: new TimeOnly(11, 0, 0),
                endTime: new TimeOnly(12, 0, 0)),
            SessionInfoFactory.CreateSessionInfo(
                startTime: new TimeOnly(10, 0, 0),
                endTime: new TimeOnly(11, 0, 0))
        };

        // Act
        var results = sessions.Select(session => _sut.AddSessionAtShedule(
            addingSessionId: session.SessionId,
            startDate: session.StartDate,
            timeRange: session.TimeRange)).ToList();

        // Assert
        results.Should().OnlyContain(result => !result.IsError);
    }

    [Fact]
    public void AddSessionAtShedule_WhenBusyScheduleWithIntersection_ShouldReturnError()
    {
        // Arrange
        var sessions = new List<SessionInfo>()
        {
            SessionInfoFactory.CreateSessionInfo(
                startTime: new TimeOnly(9, 0, 0),
                endTime: new TimeOnly(10, 0, 0)),
            SessionInfoFactory.CreateSessionInfo(
                startTime: new TimeOnly(11, 0, 0),
                endTime: new TimeOnly(12, 0, 0)),
            SessionInfoFactory.CreateSessionInfo(
                startTime: new TimeOnly(9, 59, 0),
                endTime: new TimeOnly(11, 0, 0)),
            SessionInfoFactory.CreateSessionInfo(
                startTime: new TimeOnly(10, 00, 0),
                endTime: new TimeOnly(11, 1, 0))
        };

        // Act
        var results = sessions.Select(session => _sut.AddSessionAtShedule(
            addingSessionId: session.SessionId,
            startDate: session.StartDate,
            timeRange: session.TimeRange)).ToList();

        // Assert
        results.Take(..^2).Should().OnlyContain(result => !result.IsError);
        results[^1].FirstError.Should().Be(SheduleError.OverlappReservedTrainingSession);
        results[^2].FirstError.Should().Be(SheduleError.OverlappReservedTrainingSession);
    }

    [Fact]
    public void AddSessionAtShedule_WhenAddSameSession_ShouldReturnTrainerAlreadyHaveThisSessionError()
    {
        // Arrange
        var session = SessionInfoFactory.CreateSessionInfo(
            startTime: new TimeOnly(9, 0, 0),
            endTime: new TimeOnly(10, 0, 0));

        // Act
        var results = new List<ErrorOr<Success>>();
        
        results.Add(_sut.AddSessionAtShedule(
            addingSessionId: session.SessionId,
            startDate: session.StartDate,
            timeRange: session.TimeRange));
        results.Add(_sut.AddSessionAtShedule(
            addingSessionId: session.SessionId,
            startDate: session.StartDate,
            timeRange: session.TimeRange));

        // Assert
        results.First().IsError.Should().Be(false);
        results.Last().FirstError.Should().Be(TrainerErrors.TrainerAlreadyHaveThisSession);
    }

    [Fact]
    public void RemoveSessionAtShedule_WhenSessionExist_ShouldRemoveSession()
    {
        // Arrange
        var session = SessionInfoFactory.CreateSessionInfo(
            startTime: new TimeOnly(9, 0, 0),
            endTime: new TimeOnly(10, 0, 0));

        // Act
        _sut.AddSessionAtShedule(
            addingSessionId: session.SessionId,
            startDate: session.StartDate,
            timeRange: session.TimeRange);

        var removeResult = _sut.RemoveSessionAtShedule(
            removingSessionId: session.SessionId,
            startDate: session.StartDate,
            timeRange: session.TimeRange);

        // Assert
        removeResult.IsError.Should().Be(false);
    }

    [Fact]
    public void RemoveSessionAtShedule_WhenSessionNotExist_ShouldReturnError()
    {
        // Arrange
        var session = SessionInfoFactory.CreateSessionInfo(
            startTime: new TimeOnly(9, 0, 0),
            endTime: new TimeOnly(10, 0, 0));

        // Act
        var removeResult = _sut.RemoveSessionAtShedule(
            removingSessionId: session.SessionId,
            startDate: session.StartDate,
            timeRange: session.TimeRange);

        // Assert
        removeResult.FirstError.Should().Be(TrainerErrors.TrainerAlreadyNotHaveThisSession);
    }

    [Fact]
    public void GetTrainerIsFree_WhenTrainerIsFree_ShouldReturnTrue()
    {
        // Arrange
        var startDate = new DateOnly();
        var timeRange = new TimeRange(
            startTime: new TimeOnly(9, 0, 0),
            endTime: new TimeOnly(10, 0, 0));

        // Act
        var isFree = _sut.GetTrainerIsFree(startDate, timeRange);

        // Assert
        isFree.Should().Be(true);
    }

    [Fact]
    public void GetTrainerIsFree_WhenTrainerIsNotFree_ShouldReturnFalse()
    {
        // Arrange
        var startDate = new DateOnly();
        var timeRange = new TimeRange(
            startTime: new TimeOnly(9, 0, 0),
            endTime: new TimeOnly(10, 0, 0));

        var session = new SessionInfo()
        {
            SessionId = Guid.NewGuid(),
            StartDate = startDate,
            TimeRange = timeRange
        };

        // Act
        _sut.AddSessionAtShedule(
            addingSessionId: session.SessionId,
            startDate: session.StartDate,
            timeRange: session.TimeRange);

        var isFree = _sut.GetTrainerIsFree(startDate, timeRange);

        // Assert
        isFree.Should().Be(false);
    }
}