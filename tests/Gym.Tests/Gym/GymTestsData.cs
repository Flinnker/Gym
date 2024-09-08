using ErrorOr;

namespace Gym.Domain.Tests.Unit.Gym;

public class GymTestsData
{
    private static SubscriptionType _defaultSubscriptionType = new SubscriptionType(
        maxGymCount: 1,
        maxGymRoomCount: 10,
        maxDailySessionCount: 1,
        price: 1,
        name: "TestSubscription_1",
        value: 1);

    #region CreateGym
    public static IEnumerable<object[]> GetDataForSuccessfullyCreateGymTest()
    {
        yield return new object[] { SubscriptionType.Free };
        yield return new object[] { SubscriptionType.Base };
        yield return new object[] { SubscriptionType.Pro };
        yield return new object[] { _defaultSubscriptionType };
    }

    public static IEnumerable<object[]> GetDataForFailureCreateGymTest()
    {
        yield return new object[] {
            null 
        };
    }
    #endregion CreateGym

    #region AddTrainer
    public static IEnumerable<object[]> GetDataForSuccessfullyAddTrainerTest()
    {
        yield return new object[] {
            _defaultSubscriptionType,
            Enumerable.Range(0, 3).Select(_ => Guid.NewGuid())
        };
    }

    public static IEnumerable<object[]> GetDataForAddAlreadyExistTrainerTest()
    {
        var firstTrainerId = Guid.NewGuid();
        var secondTrainerId = Guid.NewGuid();

        yield return new object[] {
            _defaultSubscriptionType,
            new List<(Guid trainerId, ErrorOr<Success> expectedResult)>()
            {
                ( trainerId: firstTrainerId, expectedResult: Result.Success),
                ( trainerId: secondTrainerId, expectedResult: Result.Success),
                ( trainerId: firstTrainerId, expectedResult: GymErrors.TrainerAlreadyExistInGym),
                ( trainerId: secondTrainerId, expectedResult: GymErrors.TrainerAlreadyExistInGym),
                ( trainerId: firstTrainerId, expectedResult: GymErrors.TrainerAlreadyExistInGym)
            } 
        };
        yield return new object[] {
            _defaultSubscriptionType,
            new List<(Guid trainerId, ErrorOr<Success> expectedResult)>()
            {
                ( trainerId: firstTrainerId, expectedResult: Result.Success),
                ( trainerId: firstTrainerId, expectedResult: GymErrors.TrainerAlreadyExistInGym),
                ( trainerId: secondTrainerId, expectedResult: Result.Success),
                ( trainerId: secondTrainerId, expectedResult: GymErrors.TrainerAlreadyExistInGym)
            }
        };
    }
    #endregion AddTrainer

    #region RemoveTrainer
    public static IEnumerable<object[]> GetDataForRemoveExistTrainerTest()
    {
        var trainerIds = Enumerable.Range(0, 3).Select(_ => Guid.NewGuid()).ToList();

        yield return new object[] {
            _defaultSubscriptionType,
            trainerIds,
            trainerIds
        };
    }

    public static IEnumerable<object[]> GetDataForRemoveNotExistTrainerTest()
    {
        var trainerIds = Enumerable.Range(0, 3).Select(_ => Guid.NewGuid()).ToList();

        yield return new object[] {
            _defaultSubscriptionType,
            trainerIds,
            new List<(Guid trainerId, ErrorOr<Success> expectedResult)>()
            {
                ( trainerId: trainerIds[0], expectedResult: Result.Success),
                ( trainerId: Guid.NewGuid(), expectedResult: GymErrors.TrainerAlreadyNotExistInGym),
                ( trainerId: Guid.NewGuid(), expectedResult: GymErrors.TrainerAlreadyNotExistInGym),
                ( trainerId: trainerIds[0], expectedResult: GymErrors.TrainerAlreadyNotExistInGym),
            }
        };
    }
    #endregion RemoveTrainer

    #region AddGymRoom
    public static IEnumerable<object[]> GetDataForSuccessfullyAddGymRoomsTest()
    {
        var trainerIds = Enumerable.Range(0, 3).Select(_ => Guid.NewGuid()).ToList();

        yield return new object[] {
            _defaultSubscriptionType,
            trainerIds
        };
        yield return new object[] {
            new SubscriptionType(
                maxGymCount: 1,
                maxGymRoomCount: 3,
                maxDailySessionCount: 1,
                price: 1,
                name: "TestSubscription_1",
                value: 1),
            trainerIds
        };
    }

    public static IEnumerable<object[]> GetDataForAddGymRoomsBetterThenMaxRoomCountTest()
    {
        yield return new object[] {
            new SubscriptionType(
                maxGymCount: 1,
                maxGymRoomCount: 2,
                maxDailySessionCount: 1,
                price: 1,
                name: "TestSubscription_1",
                value: 1),
            new List<(Guid roomId, ErrorOr<Success> expectedResult)>()
            {
                ( roomId: Guid.NewGuid(), expectedResult: Result.Success),
                ( roomId: Guid.NewGuid(), expectedResult: Result.Success),
                ( roomId: Guid.NewGuid(), expectedResult: GymErrors.CannotHaveMoreGymRoomsThanSubscritpionAllows)
            }
        };
    }

    public static IEnumerable<object[]> GetDataForAddExistGymRoomTest()
    {
        var trainerIds = Enumerable.Range(0, 2).Select(_ => Guid.NewGuid()).ToList();

        yield return new object[] {
            _defaultSubscriptionType,
            new List<(Guid roomId, ErrorOr<Success> expectedResult)>()
            {
                ( roomId: trainerIds[0], expectedResult: Result.Success),
                ( roomId: trainerIds[1], expectedResult: Result.Success),
                ( roomId: trainerIds[0], expectedResult: GymErrors.RoomAlreadyContainsInGym),
                ( roomId: trainerIds[0], expectedResult: GymErrors.RoomAlreadyContainsInGym),
                ( roomId: trainerIds[1], expectedResult: GymErrors.RoomAlreadyContainsInGym)
            }
        };
    }
    #endregion AddGymRoom

    #region RemoveGymRoom
    public static IEnumerable<object[]> GetDataForRemoveExistGymRoomsTest()
    {
        var trainerIds = Enumerable.Range(0, 3).Select(_ => Guid.NewGuid()).ToList();

        yield return new object[] {
            _defaultSubscriptionType,
            trainerIds,
            trainerIds
        };
    }

    public static IEnumerable<object[]> GetDataForRemoveNotExistGymRoomsTest()
    {
        var trainerIds = Enumerable.Range(0, 2).Select(_ => Guid.NewGuid()).ToList();

        yield return new object[] {
            _defaultSubscriptionType,
            trainerIds,
            new List<(Guid roomId, ErrorOr<Success> expectedResult)>()
            {
                ( roomId: Guid.NewGuid(), expectedResult: GymErrors.RoomAlreadyNotExistInGym),
                ( roomId: trainerIds[0], expectedResult: Result.Success),
                ( roomId: trainerIds[1], expectedResult: Result.Success),
                ( roomId: Guid.NewGuid(), expectedResult: GymErrors.SubscriptionNotHaveGymRooms),
            }
        };
    }
    #endregion RemoveGymRoom

    #region GetGymRoomCount
    public static IEnumerable<object[]> GetDataCheckGymRoomCountTest()
    {
        var trainerIds = Enumerable.Range(0, 5).Select(_ => Guid.NewGuid()).ToList();

        yield return new object[] {
            _defaultSubscriptionType,
            trainerIds,
            trainerIds.Take(2).Append(Guid.NewGuid()).ToList(),
            3
        };
        yield return new object[] {
            _defaultSubscriptionType,
            trainerIds,
            trainerIds.Skip(2).Take(2).ToList(),
            3
        };
        yield return new object[] {
            _defaultSubscriptionType,
            trainerIds,
            trainerIds,
            0
        };
    }
    #endregion GetGymRoomCount
}