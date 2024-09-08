using ErrorOr;
using FluentAssertions;
using Xunit.Abstractions;

namespace Gym.Domain.Tests.Unit.GymRoom;

public class GymRoomTests
{
    private Domain.GymRoom _sut;
    private ITestOutputHelper _outputHelper;

    public GymRoomTests(ITestOutputHelper outputHelper)
    {
        _sut = new Domain.GymRoom(SubscriptionType.Free);
        _outputHelper = outputHelper;
    }

    #region ReserveTrainingSessionInShedule
    [Theory]
    [MemberData(
        nameof(GymRoomTestsData.GetDataForSuccessfulyReserveTime),
        MemberType = typeof(GymRoomTestsData))]
    public void ReserveTimeInShedule_WhenUserReserveNotOverlappingTrainingSession_ShouldSuccessfullyReserveTimeInShedule(
        SubscriptionType subscriptionType, 
        DateOnly startDate, 
        Domain.TimeRange timeRange)
    {
        // Arrange
        _sut = new Domain.GymRoom(subscriptionType);

        // Act
        var reserveTrainingSessionResult = _sut.ReserveTimeInShedule(startDate, timeRange);

        // Assert
        reserveTrainingSessionResult.IsError.Should().BeFalse();
        reserveTrainingSessionResult.Value.Should().Be(Result.Success);
    }

    [Theory]
    [MemberData(
        nameof(GymRoomTestsData.GetDataForReserveTimeWithOverlap),
        MemberType = typeof(GymRoomTestsData))]
    public void ReserveTimeInShedule_WhenUserReserveOverlappingTrainingSession_ShouldReturnErrorAboutTimeAlreadyTaken(
        SubscriptionType subscriptionType,
        List<(DateOnly startDate, Domain.TimeRange timeRange)> timesForReserveWithoutOverlap,
        List<(DateOnly startDate, Domain.TimeRange timeRange)> timesForReserveWithOverlap)
    {
        // Arrange
        _sut = new Domain.GymRoom(subscriptionType);
        var reserveTimeWithoutOverlapResults = timesForReserveWithoutOverlap.Select(time => 
            _sut.ReserveTimeInShedule(time.startDate, time.timeRange)).ToList();

        // Act
        var reserveTimeWithOverlapResults = timesForReserveWithOverlap.Select(time =>
            _sut.ReserveTimeInShedule(time.startDate, time.timeRange)).ToList();

        // Assert
        reserveTimeWithoutOverlapResults.Should().AllSatisfy(reserveTimeWithoutOverlapResult => 
            reserveTimeWithoutOverlapResult.IsError.Should().BeFalse());
        reserveTimeWithoutOverlapResults.Should().AllSatisfy(reserveTimeWithoutOverlapResult =>
            reserveTimeWithoutOverlapResult.Value.Should().Be(Result.Success));
        reserveTimeWithOverlapResults.Should().AllSatisfy(reserveTimeWithOverlapResult =>
            reserveTimeWithOverlapResult.IsError.Should().BeTrue());
        reserveTimeWithOverlapResults.Should().AllSatisfy(reserveTimeWithOverlapResult =>
            reserveTimeWithOverlapResult.FirstError.Should().Be(SheduleError.OverlappReservedTrainingSession));
    }
    #endregion ReserveTrainingSessionInShedule

    #region CancelTrainingSessionInShedule
    [Theory]
    [MemberData(
        nameof(GymRoomTestsData.GetDataForSuccessfullyUnreserveTimeFromShedule),
        MemberType = typeof(GymRoomTestsData))]
    public void UnreserveTimeInShedule_WhenUserUnreserveTakenTimeFromShedule_ShouldSuccessfullyUnreserveTime(
        SubscriptionType subscriptionType,
        DateOnly startDate,
        Domain.TimeRange timeRange)
    {
        // Arrange
        _sut = new Domain.GymRoom(subscriptionType);
        var reserveTimeInSheduleResult = _sut.ReserveTimeInShedule(startDate, timeRange);

        // Act
        var unreserveTimeFromSheduleResult = _sut.UnreserveTimeInShedule(startDate, timeRange);

        // Assert
        reserveTimeInSheduleResult.IsError.Should().BeFalse();
        reserveTimeInSheduleResult.Value.Should().Be(Result.Success);
        unreserveTimeFromSheduleResult.IsError.Should().BeFalse();
        unreserveTimeFromSheduleResult.Value.Should().Be(Result.Success);
    }

    [Theory]
    [MemberData(
        nameof(GymRoomTestsData.GetDataForUnreserveNotExistTimeFromShedule),
        MemberType = typeof(GymRoomTestsData))]
    public void UnreserveTimeInShedule_WhenUserUnreserveNotTakenTimeFromShedule_ShouldReturnErrorAboutTakenTimeNotFound(
        SubscriptionType subscriptionType,
        List<(DateOnly startDate, Domain.TimeRange timeRange)> timesForUnreserveTimeInShedule)
    {
        // Arrange
        _sut = new Domain.GymRoom(subscriptionType);

        // Act
        var unreserveTimeInSheduleResults = timesForUnreserveTimeInShedule.Select(timeForUnreserveTimeInShedule =>
            _sut.UnreserveTimeInShedule(timeForUnreserveTimeInShedule.startDate, timeForUnreserveTimeInShedule.timeRange)).ToList();

        // Assert
        unreserveTimeInSheduleResults.Should().AllSatisfy(unreserveTimeInSheduleResult =>
            unreserveTimeInSheduleResult.IsError.Should().BeTrue());
        unreserveTimeInSheduleResults.Should().AllSatisfy(unreserveTimeInSheduleResult =>
            unreserveTimeInSheduleResult.FirstError.Should().Be(SheduleError.TrainingSessionNotFound));
    }
    #endregion CancelTrainingSessionInShedule

    #region AddTrainingSession
    [Theory]
    [MemberData(
        nameof(GymRoomTestsData.GetDataForSuccessfullyAddTrainingSession),
        MemberType = typeof(GymRoomTestsData))]
    public void AddTrainingSession_WhenUserAddTrainingSessionLessThenMaxSessionCount_ShouldSuccessfullyAddTrainingSession(
        SubscriptionType subscriptionType,
        List<Guid> trainingSessionIds)
    {
        // Arrange
        _sut = new Domain.GymRoom(subscriptionType);

        // Act
        var addTrainingSessionResults = trainingSessionIds.Select(trainingSessionIds => 
            _sut.AddTrainingSession(trainingSessionIds)).ToList();

        // Assert
        addTrainingSessionResults.Should().AllSatisfy(addTrainingSessionResult =>
            addTrainingSessionResult.IsError.Should().BeFalse());
        addTrainingSessionResults.Should().AllSatisfy(addTrainingSessionResult =>
            addTrainingSessionResult.Value.Should().Be(Result.Success));
    }

    [Theory]
    [MemberData(
        nameof(GymRoomTestsData.GetDataForAddTrainingSessionBetterThenMaxSessionCount),
        MemberType = typeof(GymRoomTestsData))]
    public void AddTrainingSession_WhenUserAddTrainingSessionBetterThenMaxSessionCount_ShouldReturnErrorAboutRoomCanNotHaveMoreTrainingSession(
        SubscriptionType subscriptionType,
        List<Guid> trainingSessionIdsForAddWithouError,
        List<Guid> trainingSessionIdsForAddWithError)
    {
        // Arrange
        _sut = new Domain.GymRoom(subscriptionType);
        var addTrainingSessionWithoutErrorResults = trainingSessionIdsForAddWithouError.Select(trainingSessionIds =>
            _sut.AddTrainingSession(trainingSessionIds)).ToList();

        // Act
        var addTrainingSessionWithErrorResults = trainingSessionIdsForAddWithError.Select(trainingSessionIds =>
            _sut.AddTrainingSession(trainingSessionIds)).ToList();

        // Assert
        addTrainingSessionWithoutErrorResults.Should().AllSatisfy(addTrainingSessionResult =>
            addTrainingSessionResult.IsError.Should().BeFalse());
        addTrainingSessionWithoutErrorResults.Should().AllSatisfy(addTrainingSessionResult =>
            addTrainingSessionResult.Value.Should().Be(Result.Success));
        addTrainingSessionWithErrorResults.Should().AllSatisfy(addTrainingSessionResult =>
            addTrainingSessionResult.IsError.Should().BeTrue());
        addTrainingSessionWithErrorResults.Should().AllSatisfy(addTrainingSessionResult =>
            addTrainingSessionResult.FirstError.Should().Be(TrainingSessionErrors.CannotHaveMoreSessionsThanSubscritpionAllows));
    }
    #endregion AddTrainingSession

    #region RemoveTrainingSession
    [Theory]
    [MemberData(
        nameof(GymRoomTestsData.GetDataForRemoveExistTraiingSession),
        MemberType = typeof(GymRoomTestsData))]
    public void RemoveTrainingSession_WhenUserRemoveExistTrainingSession_ShouldSuccessfullyRemoveTrainingSession(
        SubscriptionType subscriptionType,
        List<Guid> trainingSessionIds)
    {
        // Arrange
        _sut = new Domain.GymRoom(subscriptionType);
        var addTrainingSessionResults = trainingSessionIds.Select(trainingSessionIds =>
            _sut.AddTrainingSession(trainingSessionIds)).ToList();

        // Act
        var removeTrainingSessionResults = trainingSessionIds.Select(trainingSessionIds =>
            _sut.RemoveTrainingSession(trainingSessionIds)).ToList();

        // Assert
        addTrainingSessionResults.Should().AllSatisfy(addTrainingSessionResult =>
            addTrainingSessionResult.IsError.Should().BeFalse());
        addTrainingSessionResults.Should().AllSatisfy(addTrainingSessionResult =>
            addTrainingSessionResult.Value.Should().Be(Result.Success));
        removeTrainingSessionResults.Should().AllSatisfy(removeTrainingSessionResult =>
            removeTrainingSessionResult.IsError.Should().BeFalse());
        removeTrainingSessionResults.Should().AllSatisfy(removeTrainingSessionResult =>
            removeTrainingSessionResult.Value.Should().Be(Result.Success));
    }

    [Theory]
    [MemberData(
        nameof(GymRoomTestsData.GetDataForRemoveNotExistTrainingSession),
        MemberType = typeof(GymRoomTestsData))]
    public void RemoveTrainingSession_WhenUserRemoveNotExistTrainingSession_ShouldReturnErrorAboutSessionNotExistOrSessionCollectionIsEmpty(
        SubscriptionType subscriptionType,
        List<Guid> trainingSessionIdsForAdd,
        List<(Guid trainingSessionId, ErrorOr<Success> expectedResult)> trainingSessionIdsForRemove)
    {
        // Arrange
        _sut = new Domain.GymRoom(subscriptionType);
        var addTrainingSessionResults = trainingSessionIdsForAdd.Select(trainingSessionIdForAdd =>
            _sut.AddTrainingSession(trainingSessionIdForAdd)).ToList();

        // Act
        var removeTrainingSessionResults = trainingSessionIdsForRemove.Select(trainingSessionIdForRemove => (
                isError: trainingSessionIdForRemove.expectedResult != Result.Success,
                expectedResult: trainingSessionIdForRemove.expectedResult,
                actualResult: _sut.RemoveTrainingSession(trainingSessionIdForRemove.trainingSessionId)
            )).ToList();

        // Assert
        addTrainingSessionResults.Should().AllSatisfy(addTrainingSessionResult =>
            addTrainingSessionResult.IsError.Should().BeFalse());
        addTrainingSessionResults.Should().AllSatisfy(addTrainingSessionResult =>
            addTrainingSessionResult.Value.Should().Be(Result.Success));
        CheckResults(removeTrainingSessionResults);
    }
    #endregion RemoveTrainingSession

    #region GetDailySessionCount
    [Theory]
    [MemberData(
        nameof(GymRoomTestsData.GetDataForCheckGetTrainingSessionCount),
        MemberType = typeof(GymRoomTestsData))]
    public void GetDailySessionCount_WhenUserGetDailySessionCount_ShouldSuccessfullyReturnCurrentTrainingSessionCount(
        SubscriptionType subscriptionType,
        List<Guid> trainingSessionIdsForAdd,
        List<(Guid trainingSessionId, ErrorOr<Success> expectedResult)> trainingSessionIdsForRemove,
        int expectedCount)
    {
        // Arrange
        _sut = new Domain.GymRoom(subscriptionType);
        var addTrainingSessionResults = trainingSessionIdsForAdd.Select(trainingSessionIdForAdd => 
            _sut.AddTrainingSession(trainingSessionIdForAdd)).ToList();
        var removeTrainingSessionResults = trainingSessionIdsForRemove.Select(trainingSessionIdForRemove => (
            isError: trainingSessionIdForRemove.expectedResult != Result.Success,
            expectedResult: trainingSessionIdForRemove.expectedResult,
            actualResult: _sut.RemoveTrainingSession(trainingSessionIdForRemove.trainingSessionId))).ToList();

        // Act
        var actualCount = _sut.GetDailySessionCount();

        // Assert
        addTrainingSessionResults.Should().AllSatisfy(addTrainingSessionResult => addTrainingSessionResult.IsError.Should().BeFalse());
        addTrainingSessionResults.Should().AllSatisfy(addTrainingSessionResult => addTrainingSessionResult.Value.Should().Be(Result.Success));
        CheckResults(removeTrainingSessionResults);
        actualCount.Should().Be(expectedCount);
    }
    #endregion GetDailySessionCount

    private void CheckResults(IEnumerable<(bool isError, ErrorOr<Success> expectedResult, ErrorOr<Success> actualResult)> results)
    {
        results.Should().AllSatisfy(removeTrainerResult =>
            removeTrainerResult.actualResult.IsError.Should().Be(removeTrainerResult.isError));
        results.Where(removeTrainerResult => removeTrainerResult.isError).Should().AllSatisfy(removeTrainerResult =>
            removeTrainerResult.actualResult.FirstError.Should().BeEquivalentTo(removeTrainerResult.expectedResult.FirstError));
        results.Where(removeTrainerResult => !removeTrainerResult.isError).Should().AllSatisfy(removeTrainerResult =>
            removeTrainerResult.actualResult.Value.Should().BeEquivalentTo(removeTrainerResult.expectedResult.Value));
    }
}