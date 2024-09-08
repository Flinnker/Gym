using ErrorOr;

namespace Gym.Domain.Tests.Unit.GymRoom;

public class GymRoomTestsData
{
    private static SubscriptionType _defaultSubscriptionType = new SubscriptionType(
        maxGymCount: 1,
        maxGymRoomCount: 10,
        maxDailySessionCount: 5,
        price: 1,
        name: "TestSubscription_1",
        value: 1);

    #region ReserveTimeInShedule
    public static IEnumerable<object[]> GetDataForSuccessfulyReserveTime()
    {
        yield return new object[] {
            _defaultSubscriptionType, 
            new DateOnly(2000, 1, 1), 
            new Domain.TimeRange(
                startTime: new TimeOnly(10, 0, 0), 
                endTime: new TimeOnly(11, 0, 0)) };
    }

    public static IEnumerable<object[]> GetDataForReserveTimeWithOverlap()
    {
        yield return new object[] {
            _defaultSubscriptionType,
            new List<(DateOnly startDate, Domain.TimeRange timeRange)>()
            {
                (
                    startDate: new DateOnly(2000, 1, 1),
                    timeRange: new Domain.TimeRange(
                        startTime: new TimeOnly(10, 0, 0),
                        endTime: new TimeOnly(11, 0, 0))
                ),
                (
                    startDate: new DateOnly(2000, 1, 1),
                    timeRange: new Domain.TimeRange(
                        startTime: new TimeOnly(12, 0, 0),
                        endTime: new TimeOnly(13, 0, 0))
                )
            },
            new List<(DateOnly startDate, Domain.TimeRange timeRange)>()
            {
                (
                    startDate: new DateOnly(2000, 1, 1),
                    timeRange: new Domain.TimeRange(
                        startTime: new TimeOnly(10, 59, 0),
                        endTime: new TimeOnly(11, 59, 0))
                ),
                (
                    startDate: new DateOnly(2000, 1, 1),
                    timeRange: new Domain.TimeRange(
                        startTime: new TimeOnly(11, 1, 0),
                        endTime: new TimeOnly(12, 1, 0))
                )
            }
        };
    }
    #endregion ReserveTimeInShedule

    #region UnreserveTimeInShedule
    public static IEnumerable<object[]> GetDataForSuccessfullyUnreserveTimeFromShedule()
    {
        yield return new object[] {
            _defaultSubscriptionType,
            new DateOnly(2000, 1, 1),
            new Domain.TimeRange(
                startTime: new TimeOnly(10, 0, 0),
                endTime: new TimeOnly(11, 0, 0)) };
    }

    public static IEnumerable<object[]> GetDataForUnreserveNotExistTimeFromShedule()
    {
        yield return new object[] {
            _defaultSubscriptionType,
            new List<(DateOnly startDate, Domain.TimeRange timeRange)>()
            {
                (
                    startDate: new DateOnly(2000, 1, 1),
                    timeRange: new Domain.TimeRange(
                        startTime: new TimeOnly(10, 59, 0),
                        endTime: new TimeOnly(11, 59, 0))
                )
            }
        };
    }
    #endregion UnreserveTimeInShedule

    #region AddTrainingSession
    public static IEnumerable<object[]> GetDataForSuccessfullyAddTrainingSession()
    {
        yield return new object[] {
            _defaultSubscriptionType,
            Enumerable.Range(0, 5).Select(_ => Guid.NewGuid()).ToList()
        };
    }

    public static IEnumerable<object[]> GetDataForAddTrainingSessionBetterThenMaxSessionCount()
    {
        yield return new object[] {
            new SubscriptionType(
                maxGymCount: 1,
                maxGymRoomCount: 10,
                maxDailySessionCount: 3,
                price: 1,
                name: "TestSubscription_1",
                value: 1),
            Enumerable.Range(0, 3).Select(_ => Guid.NewGuid()).ToList(),
            Enumerable.Range(0, 3).Select(_ => Guid.NewGuid()).ToList()
        };
    }
    #endregion AddTrainingSession

    #region RemoveTrainingSession
    public static IEnumerable<object[]> GetDataForRemoveExistTraiingSession()
    {
        yield return new object[] {
            _defaultSubscriptionType,
            Enumerable.Range(0, 5).Select(_ => Guid.NewGuid()).ToList()
        };
    }

    public static IEnumerable<object[]> GetDataForRemoveNotExistTrainingSession()
    {
        var trainingSessionIds = Enumerable.Range(0, 2).Select(_ => Guid.NewGuid()).ToList();

        yield return new object[] {
            _defaultSubscriptionType,
            trainingSessionIds,
            new List<(Guid trainingSessionId, ErrorOr<Success> expectedResult)>() 
            {
                (trainingSessionId: trainingSessionIds[0], expectedResult: Result.Success),
                (trainingSessionId: Guid.NewGuid(), expectedResult: GymRoomsErrors.trainingSessionAlreadyNotExist),
                (trainingSessionId: trainingSessionIds[1], expectedResult: Result.Success),
                (trainingSessionId: Guid.NewGuid(), expectedResult: TrainingSessionErrors.SubscriptionNotHaveSessions)
            }
        };
    }
    #endregion RemoveTrainingSession

    #region GetDailySessionCount
    public static IEnumerable<object[]> GetDataForCheckGetTrainingSessionCount()
    {
        var trainingSessionIds = Enumerable.Range(0, 3).Select(_ => Guid.NewGuid()).ToList();

        yield return new object[] {
            _defaultSubscriptionType,
            trainingSessionIds,
            new List<(Guid trainingSessionId, ErrorOr<Success> expectedResult)>()
            {
                (trainingSessionId: trainingSessionIds[0], expectedResult: Result.Success),
                (trainingSessionId: Guid.NewGuid(), expectedResult: GymRoomsErrors.trainingSessionAlreadyNotExist),
                (trainingSessionId: Guid.NewGuid(), expectedResult: GymRoomsErrors.trainingSessionAlreadyNotExist)
            },
            2
        };
        yield return new object[] {
            _defaultSubscriptionType,
            trainingSessionIds,
            new List<(Guid trainingSessionId, ErrorOr<Success> expectedResult)>()
            {
                (trainingSessionId: trainingSessionIds[0], expectedResult: Result.Success),
                (trainingSessionId: trainingSessionIds[0], expectedResult: GymRoomsErrors.trainingSessionAlreadyNotExist),
                (trainingSessionId: trainingSessionIds[1], expectedResult: Result.Success)
            },
            1
        };
    }
    #endregion GetDailySessionCount
}