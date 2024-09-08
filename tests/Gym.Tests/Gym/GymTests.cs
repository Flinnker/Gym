using ErrorOr;
using FluentAssertions;
using Xunit.Abstractions;

namespace Gym.Domain.Tests.Unit.Gym;

public class GymTests
{
    private Domain.Gym _sut;
    private ITestOutputHelper _outputHelper;

    public GymTests(ITestOutputHelper outputHelper) 
    {
        _sut = Domain.Gym.CreateGym(SubscriptionType.Free).Value;
        _outputHelper = outputHelper;
    }

    #region CreateGym
    [Theory]
    [MemberData(
        nameof(GymTestsData.GetDataForSuccessfullyCreateGymTest),
        MemberType = typeof(GymTestsData))]
    public void CreateGym_WhereSubscriptionTypeIsNotNull_ShouldSuccessfullyReturnGymInstance(SubscriptionType type)
    {
        // Act
        var createGymResult = Domain.Gym.CreateGym(type);

        // Assert
        createGymResult.IsError.Should().BeFalse();
        createGymResult.Value.Should().NotBeNull();
        createGymResult.Value.Should().BeOfType(typeof(Domain.Gym));
    }

    [Theory]
    [MemberData(
        nameof(GymTestsData.GetDataForFailureCreateGymTest),
        MemberType = typeof(GymTestsData))]
    public void CreateGym_WhereSubscriptionTypeIsNull_ShouldReturnErrorAboutNullSubscriptionType(SubscriptionType type)
    {
        // Act
        var createGymResult = Domain.Gym.CreateGym(type);

        // Assert
        createGymResult.IsError.Should().BeTrue();
        createGymResult.FirstError.Should().Be(GymErrors.SubscriptionIsNull);
    }
    #endregion CreateGym

    #region AddTrainer
    [Theory]
    [MemberData(
        nameof(GymTestsData.GetDataForSuccessfullyAddTrainerTest),
        MemberType = typeof(GymTestsData))]
    public void AddTrainer_WhenUserTryAddNewTrainers_ShouldSuccessfullyAddTrainers(
        SubscriptionType subscriptionType,
        IEnumerable<Guid> trainerIds)
    {
        // Arrange
        _sut = new Domain.Gym(subscriptionType);

        // Act
        var addTrainerResults = trainerIds.Select(trainerId => _sut.AddTrainer(trainerId)).ToList();

        // Assert
        addTrainerResults.Should().AllSatisfy(addTrainerResult => addTrainerResult.IsError.Should().BeFalse());
        addTrainerResults.Should().AllSatisfy(addTrainerResult => addTrainerResult.Value.Should().Be(Result.Success));
    }

    [Theory]
    [MemberData(
        nameof(GymTestsData.GetDataForAddAlreadyExistTrainerTest),
        MemberType = typeof(GymTestsData))]
    public void AddTrainer_WhenUserTryAddTrainerWhichAlreadyExistInGym_ShoulErrorAboutTrainerAlreadyExist(
        SubscriptionType subscriptionType,
        List<(Guid trainerId, ErrorOr<Success> expectedResult)> trainerIdsAndExpectedResults)
    {
        // Arrange
        _sut = new Domain.Gym(subscriptionType);

        // Act
        var addTrainerResults = trainerIdsAndExpectedResults
            .Select(trainerIdAndExpectedResult => (
                isError: trainerIdAndExpectedResult.expectedResult != Result.Success,
                expectedResult: trainerIdAndExpectedResult.expectedResult,
                actualResult: _sut.AddTrainer(trainerIdAndExpectedResult.trainerId))).ToList();

        // Assert
        CheckResults(addTrainerResults);
    }
    #endregion AddTrainer

    #region RemoveTrainer
    [Theory]
    [MemberData(
        nameof(GymTestsData.GetDataForRemoveExistTrainerTest),
        MemberType = typeof(GymTestsData))]
    public void RemoveTrainer_WhenUserTryRemoveOnlyExistTrainer_ShouldSuccessfullyRemoveTrainer(
        SubscriptionType subscriptionType,
        List<Guid> trainerIdsForAdd,
        List<Guid> trainerIdsForRemove)
    {
        // Arrange
        _sut = new Domain.Gym(subscriptionType);
        var addTrainerResults = trainerIdsForAdd.Select(trainerId => _sut.AddTrainer(trainerId)).ToList();

        // Act
        var removeTrainerResults = trainerIdsForRemove.Select(trainerId => _sut.RemoveTrainer(trainerId)).ToList();

        // Assert
        addTrainerResults.Should().AllSatisfy(addTrainerResult => addTrainerResult.IsError.Should().BeFalse());
        addTrainerResults.Should().AllSatisfy(addTrainerResult => addTrainerResult.Value.Should().Be(Result.Success));
        removeTrainerResults.Should().AllSatisfy(addTrainerResult => addTrainerResult.IsError.Should().BeFalse());
        removeTrainerResults.Should().AllSatisfy(addTrainerResult => addTrainerResult.Value.Should().Be(Result.Success));
    }

    [Theory]
    [MemberData(
        nameof(GymTestsData.GetDataForRemoveNotExistTrainerTest),
        MemberType = typeof(GymTestsData))]
    public void RemoveTrainer_WhenUserTryRemoveNotExistTrainer_ShouldReturnErrorAboutTrainerAlreadyNotExitInGym(
        SubscriptionType subscriptionType,
        List<Guid> trainerIdsForAdd,
        List<(Guid trainerId, ErrorOr<Success> expectedResult)> trainerIdsForRemoveWithExpectedResults)
    {
        // Arrange
        _sut = new Domain.Gym(subscriptionType);
        var addTrainerResults = trainerIdsForAdd.Select(trainerId => _sut.AddTrainer(trainerId)).ToList();

        // Act
        var removeTrainerResults = trainerIdsForRemoveWithExpectedResults
            .Select(trainerIdWithResult => (
                isError: trainerIdWithResult.expectedResult != Result.Success,
                expectedResult: trainerIdWithResult.expectedResult,
                actualResult: _sut.RemoveTrainer(trainerIdWithResult.trainerId)
            )).ToList();

        // Assert
        addTrainerResults.Should().AllSatisfy(addTrainerResult => addTrainerResult.IsError.Should().BeFalse());
        addTrainerResults.Should().AllSatisfy(addTrainerResult => addTrainerResult.Value.Should().Be(Result.Success));
        CheckResults(removeTrainerResults);
    }
    #endregion RemoveTrainer

    #region AddGymRoom
    [Theory]
    [MemberData(
        nameof(GymTestsData.GetDataForSuccessfullyAddGymRoomsTest),
        MemberType = typeof(GymTestsData))]
    public void AddGymRoom_WhenUserAddNotExistRooms_ShouldSuccessfullyAddRooms(
        SubscriptionType subscriptionType, 
        List<Guid> roomIds)
    {
        // Arrange
        _sut = new Domain.Gym(subscriptionType);

        // Act
        var addRoomResults = roomIds.Select(roomId => _sut.AddGymRoom(roomId)).ToList();

        // Assert
        addRoomResults.Should().AllSatisfy(addRoomResult => addRoomResult.IsError.Should().BeFalse());
        addRoomResults.Should().AllSatisfy(addRoomResult => addRoomResult.Value.Should().Be(Result.Success));
    }

    [Theory]
    [MemberData(
        nameof(GymTestsData.GetDataForAddGymRoomsBetterThenMaxRoomCountTest),
        MemberType = typeof(GymTestsData))]
    public void AddGymRoom_WhenUserAddRoomsBetterThenMaxRoomCount_ShouldReturnErrorAboutGymCannotHaveMoreGymRooms(
        SubscriptionType subscriptionType,
        List<(Guid roomId, ErrorOr<Success> expectedResult)> roomIdsWithExpectedResults)
    {
        // Arrange
        _sut = new Domain.Gym(subscriptionType);

        // Act
        var addRoomResults = roomIdsWithExpectedResults.Select(roomIdWithExpectedResult => (
            isError: roomIdWithExpectedResult.expectedResult != Result.Success,
            expectedResult: roomIdWithExpectedResult.expectedResult,
            actualResult: _sut.AddGymRoom(roomIdWithExpectedResult.roomId)
        )).ToList();

        // Assert
        CheckResults(addRoomResults);
    }

    [Theory]
    [MemberData(
        nameof(GymTestsData.GetDataForAddExistGymRoomTest),
        MemberType = typeof(GymTestsData))]
    public void AddGymRoom_WhenUserAddExistRooms_ShouldReturnErrorAboutRoomIsExist(
        SubscriptionType subscriptionType,
        List<(Guid roomId, ErrorOr<Success> expectedResult)> roomIdsWithExpectedResults)
    {
        // Arrange
        _sut = new Domain.Gym(subscriptionType);

        // Act
        var addRoomResults = roomIdsWithExpectedResults.Select(roomIdWithExpectedResult => (
            isError: roomIdWithExpectedResult.expectedResult != Result.Success,
            expectedResult: roomIdWithExpectedResult.expectedResult,
            actualResult: _sut.AddGymRoom(roomIdWithExpectedResult.roomId)
        )).ToList();

        // Assert
        CheckResults(addRoomResults);
    }
    #endregion AddGymRoom

    #region RemoveGymRoom
    [Theory]
    [MemberData(
        nameof(GymTestsData.GetDataForRemoveExistGymRoomsTest),
        MemberType = typeof(GymTestsData))]
    public void RemoveGymRoom_WhenUserTryRemoveExistRoom_ShouldSuccessfullyRemoveRoom(
        SubscriptionType subscriptionType,
        List<Guid> roomIdsForAdd,
        List<Guid> roomIdsForRemove)
    {
        // Arrange
        _sut = new Domain.Gym(subscriptionType);
        var addRoomResults = roomIdsForAdd.Select(roomId => _sut.AddGymRoom(roomId)).ToList();

        // Act
        var removeRoomResults = roomIdsForRemove.Select(roomIdForRemove => 
            _sut.RemoveGymRoom(roomIdForRemove)).ToList();

        // Assert
        addRoomResults.Should().AllSatisfy(addRoomResult => addRoomResult.IsError.Should().BeFalse());
        addRoomResults.Should().AllSatisfy(addRoomResult => addRoomResult.Value.Should().Be(Result.Success));
        removeRoomResults.Should().AllSatisfy(removeRoomResult => removeRoomResult.IsError.Should().BeFalse());
        removeRoomResults.Should().AllSatisfy(removeRoomResult => removeRoomResult.Value.Should().Be(Result.Success));
    }

    [Theory]
    [MemberData(
        nameof(GymTestsData.GetDataForRemoveNotExistGymRoomsTest),
        MemberType = typeof(GymTestsData))]
    public void RemoveGymRoom_WhenUserTryRemoveNotExistRoom_ShouldReturnErrorAboutRoomNotExistOrGymNotHaveRooms(
        SubscriptionType subscriptionType,
        List<Guid> roomIdsForAdd,
        List<(Guid roomId, ErrorOr<Success> expectedResult)> roomIdsForRemoveWithExpectedResults)
    {
        // Arrange
        _sut = new Domain.Gym(subscriptionType);
        var addRoomResults = roomIdsForAdd.Select(roomId => _sut.AddGymRoom(roomId)).ToList();

        // Act
        var removeRoomResults = roomIdsForRemoveWithExpectedResults.Select(roomIdForRemoveWithExpectedResult => (
            isError: roomIdForRemoveWithExpectedResult.expectedResult != Result.Success,
            expectedError: roomIdForRemoveWithExpectedResult.expectedResult,
            actualResult: _sut.RemoveGymRoom(roomIdForRemoveWithExpectedResult.roomId)
        )).ToList();

        // Assert
        addRoomResults.Should().AllSatisfy(addRoomResult => addRoomResult.IsError.Should().BeFalse());
        addRoomResults.Should().AllSatisfy(addRoomResult => addRoomResult.Value.Should().Be(Result.Success));
        CheckResults(removeRoomResults);
    }
    #endregion RemoveGymRoom

    #region GetGymRoomCount
    [Theory]
    [MemberData(
        nameof(GymTestsData.GetDataCheckGymRoomCountTest),
        MemberType = typeof(GymTestsData))]
    public void GetGymRoomCount(
        SubscriptionType subscriptionType,
        List<Guid> roomIdsForAdd,
        List<Guid> roomIdsForRemove,
        int expectedRoomCount)
    {
        // Arrange
        _sut = new Domain.Gym(subscriptionType);
        roomIdsForAdd.Select(roomIdForAdd => _sut.AddGymRoom(roomIdForAdd)).ToList();
        roomIdsForRemove.Select(roomIdForRemove => _sut.RemoveGymRoom(roomIdForRemove)).ToList();

        // Act
        var actualRoomCount = _sut.GetGymRoomCount();

        // Assert
        actualRoomCount.Should().Be(expectedRoomCount);
    }
    #endregion GetGymRoomCount

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