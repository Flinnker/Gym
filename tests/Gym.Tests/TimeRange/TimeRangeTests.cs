using FluentAssertions;
using Gym.Domain.Tests.Unit.Common;
using Xunit.Abstractions;

namespace Gym.Domain.Tests.Unit.TimeRange;

public class TimeRangeTests
{
    private Domain.TimeRange _sut;
    private ITestOutputHelper _outputHelper;

    public TimeRangeTests(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    #region Create
    [Fact]
    public void Create_WhenCreateTimeRangeWithCorrectData_ShouldSuccessfulyCreateTimeRange()
    {
        // Arrange
        var startTime = new TimeOnly(2, 2, 2);
        var endTime = new TimeOnly(3, 3, 3);

        // Act
        var createTimeRangeResult = Domain.TimeRange.Create(
            startTime: startTime,
            endTime: endTime);

        // Assert
        createTimeRangeResult.Value.Should().NotBeNull();
        createTimeRangeResult.Value.Should().BeOfType<Domain.TimeRange>();
    }

    [Fact]
    public void Create_WhenCreateTimeRangeWithIncorrectStartTime_ShouldReturnErrorAboutIncorrectStartTime()
    {
        // Arrange
        var startTime = new TimeOnly();
        var endTime = new TimeOnly(2, 2, 2);

        // Act
        var createTimeRangeResult = Domain.TimeRange.Create(
            startTime: startTime,
            endTime: endTime);

        // Assert
        createTimeRangeResult.FirstError.Should().Be(TimeRangeErrors.NotAvailableSessionBeginTime);
    }

    [Fact]
    public void Create_WhenCreateTimeRangeWithIncorrectEndTime_ShouldReturnErrorAboutIncorrectEndTime()
    {
        // Arrange
        var startTime = new TimeOnly(2, 2, 2);
        var endTime = new TimeOnly();

        // Act
        var createTimeRangeResult = Domain.TimeRange.Create(
            startTime: startTime,
            endTime: endTime);

        // Assert
        createTimeRangeResult.FirstError.Should().Be(TimeRangeErrors.NotAvailableSessionEndTime);
    }

    [Fact]
    public void Create_WhenCreateTimeRangeWithStartTimeMoreEndTime_ShouldReturnErrorAboutStartTimeMoreEndTime()
    {
        // Arrange
        var startTime = new TimeOnly(3, 3, 3);
        var endTime = new TimeOnly(2, 2, 2);

        // Act
        var createTimeRangeResult = Domain.TimeRange.Create(
            startTime: startTime,
            endTime: endTime);

        // Assert
        createTimeRangeResult.FirstError.Should().Be(TimeRangeErrors.EndCanNotBeEarlierThenBegin);
    }

    [Fact]
    public void Create_WhenCreateTimeRangeWithStartTimeEqualEndTime_ShouldReturnErrorAboutZeroDuration()
    {
        // Arrange
        var startTime = new TimeOnly(2, 2, 2);
        var endTime = new TimeOnly(2, 2, 2);

        // Act
        var createTimeRangeResult = Domain.TimeRange.Create(
            startTime: startTime,
            endTime: endTime);

        // Assert
        createTimeRangeResult.FirstError.Should().Be(TimeRangeErrors.DurationCanNotBeZeroMinutes);
    }
    #endregion Create

    [Fact]
    public void GetSessionAlreadyEnd_WhenSessionNotStarted_ShouldReturnFalse()
    {
        // Arrange
        var startTime = new TimeOnly(2, 2, 2);
        var endTime = new TimeOnly(3, 3, 3);
        _sut = Domain.TimeRange.Create(startTime, endTime).Value;
        var startDate = new DateOnly(2000, 1, 1);
        var dateTimeProvider = new DateTimeProviderForTest()
        {
            UtcNow = new DateTime(2000, 1, 1, 2, 2, 1)
        };

        // Act
        var getSessionAlreadyEndResult = _sut.GetSessionAlreadyEnd(startDate, dateTimeProvider);

        // Assert
        getSessionAlreadyEndResult.Should().BeFalse();
    }

    [Fact]
    public void GetSessionAlreadyEnd_WhenSessionAlreadyEnd_ShouldReturnTrue()
    {
        // Arrange
        var startTime = new TimeOnly(2, 2, 2);
        var endTime = new TimeOnly(3, 3, 3);
        _sut = Domain.TimeRange.Create(startTime, endTime).Value;
        var startDate = new DateOnly(2000, 1, 1);
        var dateTimeProvider = new DateTimeProviderForTest()
        {
            UtcNow = new DateTime(2000, 1, 1, 3, 3, 3)
        };

        // Act
        var getSessionAlreadyEndResult = _sut.GetSessionAlreadyEnd(startDate, dateTimeProvider);

        // Assert
        getSessionAlreadyEndResult.Should().BeTrue();
    }

    [Fact]
    public void GetUserLateForCancel_WhenUserNotLateForCancel_ShouldReturnFalse()
    {
        // Arrange
        var sessionInfo = SessionInfoFactory.CreateSessionInfo(
            startDate: new DateOnly(2000, 1, 3),
            startTime: new TimeOnly(9, 0, 0),
            endTime: new TimeOnly(10, 0, 0));
        var dateTimeProvider = new DateTimeProviderForTest()
        {
            UtcNow = new DateTime(2000, 1, 1, 1, 1, 1)
        };
        _sut = sessionInfo.TimeRange;

        // Act
        var getUserLateForCancelResult = _sut.GetUserLateForCancel(sessionInfo.StartDate, dateTimeProvider);

        // Assert
        getUserLateForCancelResult.Should().BeFalse();
    }

    [Fact]
    public void GetUserLateForCancel_WhenUserAlredyLateForCancel_ShouldReturnTrue()
    {
        // Arrange
        var sessionInfo = SessionInfoFactory.CreateSessionInfo(
            startDate: new DateOnly(2000, 1, 3),
            startTime: new TimeOnly(9, 0, 0),
            endTime: new TimeOnly(10, 0, 0));
        var dateTimeProvider = new DateTimeProviderForTest()
        {
            UtcNow = new DateTime(2000, 1, 3, 1, 1, 1)
        };
        _sut = sessionInfo.TimeRange;

        // Act
        var getUserLateForCancelResult = _sut.GetUserLateForCancel(sessionInfo.StartDate, dateTimeProvider);

        // Assert
        getUserLateForCancelResult.Should().BeTrue();
    }

    [Fact]
    public void GetIsTimeRangeOverlapping_WhenTimeRangeNotOverlap_ShouldReturnFalse()
    {
        // Arrange
        var sessionInfoForSut = SessionInfoFactory.CreateSessionInfo(
            startTime: new TimeOnly(9, 0, 0),
            endTime: new TimeOnly(10, 0, 0));
        var sessionInfoForCheckOverlapping = SessionInfoFactory.CreateSessionInfo(
            startTime: new TimeOnly(11, 0, 0),
            endTime: new TimeOnly(12, 0, 0));
        _sut = sessionInfoForSut.TimeRange;

        // Act
        var getIsTimeRangeOverlappingResult = _sut.GetIsTimeRangeOverlapping(sessionInfoForCheckOverlapping.TimeRange);

        // Assert
        getIsTimeRangeOverlappingResult.Should().BeFalse();
    }

    [Fact]
    public void GetIsTimeRangeOverlapping_WhenTimeRangeOverlap_ShouldReturnTrue()
    {
        // Arrange
        var sessionInfo = SessionInfoFactory.CreateSessionInfo(
            startTime: new TimeOnly(9, 0, 0),
            endTime: new TimeOnly(10, 0, 0));
        _sut = sessionInfo.TimeRange;

        // Act
        var getIsTimeRangeOverlappingResult = _sut.GetIsTimeRangeOverlapping(sessionInfo.TimeRange);

        // Assert
        getIsTimeRangeOverlappingResult.Should().BeTrue();
    }
}