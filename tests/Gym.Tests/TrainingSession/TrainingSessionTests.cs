using Gym.Domain;
using FluentAssertions;
using Xunit.Abstractions;
using Gym.Domain.Tests.Unit.TrainingSession;

namespace Gym.Tests.TrainingSession;

public class TrainingSessionTests
{
    private Domain.TrainingSession _sut;
    private ITestOutputHelper _outputHelper;

    public TrainingSessionTests(ITestOutputHelper outputHelper) 
    {
        _outputHelper = outputHelper;
    }

    [Fact]
    public void ReserveSpot_WhenAddManyParticipant_ShouldReturnError()
    {
        // Arrange
        const int SessionSize = 1;
        _sut = TrainingSessionFactory.CreateTrainingSession(
            sessionSize: SessionSize,
            startTime: new TimeOnly(10, 0, 0),
            endTime: new TimeOnly(11, 0, 0),
            startSessionDate: new DateOnly(2000, 1, 2));

        var participants = Enumerable.Range(0, SessionSize + 1).Select(item => Guid.NewGuid());

        // Act
        var results = participants.Select(participant => _sut.ReserveSpot(participant));

        // Assert
        results.Take(..^1).Should().OnlyContain(result => !result.IsError);
        results.Last().FirstError.Should().Be(TrainingSessionErrors.NotHaveAvailableSpot);
    }

    [Fact]
    public void ReserveSpot_WhenAddSomeParticipant_ShouldReturnError()
    {
        // Arrange
        const int SessionSize = 2;
        _sut = TrainingSessionFactory.CreateTrainingSession(
            sessionSize: SessionSize,
            startTime: new TimeOnly(10, 0, 0),
            endTime: new TimeOnly(11, 0, 0),
            startSessionDate: new DateOnly(2000, 1, 2));
        var participantId = Guid.NewGuid();

        // Act
        var firstReserveSpotResult = _sut.ReserveSpot(participantId);
        var secondReserveSpotResult = _sut.ReserveSpot(participantId);

        // Assert
        firstReserveSpotResult.IsError.Should().BeFalse();
        secondReserveSpotResult.FirstError.Should().Be(TrainingSessionErrors.UserAlreadyReserveSpot);
    }

    [Fact]
    public void CancelReservation_WhenCancelExistParticipant_ShouldCancelReservation()
    {
        // Arrange
        const int SessionSize = 1;
        _sut = TrainingSessionFactory.CreateTrainingSession(
            sessionSize: SessionSize,
            startTime: new TimeOnly(10, 0, 0),
            endTime: new TimeOnly(11, 0, 0),
            startSessionDate: new DateOnly(2000, 1, 2));
        var timeProvider = new DateTimeProviderForTest()
        {
            UtcNow = new DateTime(2000, 1, 1, 6, 0, 0)
        };
        var participantId = Guid.NewGuid();

        // Act
        _sut.ReserveSpot(participantId);
        var cancelReservationResult = _sut.CancelReservation(participantId, timeProvider);

        // Assert
        cancelReservationResult.IsError.Should().BeFalse();
    }

    [Fact]
    public void CancelReservation_WhenCancelExistParticipantAndParticipantLate_ShouldReturnError()
    {
        // Arrange
        const int SessionSize = 1;
        _sut = TrainingSessionFactory.CreateTrainingSession(
            sessionSize: SessionSize,
            startTime: new TimeOnly(10, 0, 0),
            endTime: new TimeOnly(11, 0, 0),
            startSessionDate: new DateOnly(2000, 1, 2));
        var participantId = Guid.NewGuid();
        var firstTimeProvider = new DateTimeProviderForTest()
        {
            UtcNow = new DateTime(2000, 1, 2, 6, 0, 0)
        };
        var secondTimeProvider = new DateTimeProviderForTest()
        {
            UtcNow = new DateTime(2000, 1, 2, 16, 0, 0)
        };

        // Act
        var reserveSpotResult = _sut.ReserveSpot(participantId);
        var firstCancelReservationResult = _sut.CancelReservation(participantId, firstTimeProvider);
        var secondCancelReservationResult = _sut.CancelReservation(participantId, secondTimeProvider);

        // Assert
        reserveSpotResult.IsError.Should().BeFalse();
        firstCancelReservationResult.FirstError.Should().Be(TrainingSessionErrors.YouLate);
        secondCancelReservationResult.FirstError.Should().Be(TrainingSessionErrors.SessionAlreadyEnd);
    }

    [Fact]
    public void CancelReservation_WhenCancelNotExistParticipant_ShouldReturnError()
    {
        // Arrange
        const int SessionSize = 1;
        _sut = TrainingSessionFactory.CreateTrainingSession(
            sessionSize: SessionSize,
            startTime: new TimeOnly(10, 0, 0),
            endTime: new TimeOnly(11, 0, 0),
            startSessionDate: new DateOnly(2000, 1, 2));
        var timeProvider = new DateTimeProviderForTest()
        {
            UtcNow = new DateTime(2000, 1, 1, 6, 0, 0)
        };
        var participantId = Guid.NewGuid();

        // Act
        var reserveSpotResult = _sut.ReserveSpot(participantId);
        var cancelReservationResult = _sut.CancelReservation(participantId, timeProvider);

        // Assert
        reserveSpotResult.IsError.Should().BeFalse();
        cancelReservationResult.IsError.Should().BeFalse();
    }

    public class DateTimeProviderForTest : IDateTimeProvider
    {
        private DateTime _utcNow = DateTime.UtcNow;

        public DateTime UtcNow
        {
            get
            {
                return _utcNow;
            }
            set 
            {
                _utcNow = value;
            }
        }
    }
}