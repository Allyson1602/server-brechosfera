using Brechosfera.Application.Interfaces;
using Brechosfera.Application.Notifications;
using Brechosfera.Application.Requests;
using Brechosfera.Application.Services;
using Brechosfera.Domain.Abstractions;
using Brechosfera.Domain.Entities;
using Brechosfera.Domain.Repositories;
using Brechosfera.Tests.Common.Factories;
using NSubstitute;

namespace Brechosfera.Application.Tests;

public class EventServiceTests
{
    private readonly IEventRepository _repository = Substitute.For<IEventRepository>();
    private readonly IStoreRepository _storeRepository = Substitute.For<IStoreRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private readonly Notifier _notifier = new();
    private readonly EventService _service;

    public EventServiceTests()
    {
        _service = new EventService(
            unitOfWork: _unitOfWork,
            notifier: _notifier,
            repository: _repository,
            storeRepository: _storeRepository,
            currentUser: _currentUser
        );

        _unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(true);
    }

    private Store ArrangeStore(Guid ownerId)
    {
        var store = StoreFactory.Create(ownerUserId: ownerId);

        _storeRepository.GetByIdAsync(store.Id, Arg.Any<CancellationToken>()).Returns(store);

        return store;
    }

    private Event ArrangeEvent(Guid storeId)
    {
        var storeEvent = EventFactory.Create(responsibleStoreId: storeId);

        _repository.GetByIdAsync(storeEvent.Id, Arg.Any<CancellationToken>()).Returns(storeEvent);

        return storeEvent;
    }

    private static UpdateEventRequest NewUpdateRequest() => new(
        Banner: "novobanner",
        Name: "Bazar de Verão",
        Description: "Nova descrição.",
        OrganizerName: "Bruno Lima",
        Address: new AddressRequest(
            Cep: "20040020",
            Street: "Avenida Rio Branco",
            Neighborhood: "Centro",
            City: "Rio de Janeiro",
            StateCode: "RJ",
            State: "Rio de Janeiro",
            Complement: null
        ),
        StartDate: EventFactory.BaseDate.AddDays(7),
        EndDate: EventFactory.BaseDate.AddDays(7).AddHours(6)
    );

    private static CreateEventRequest NewCreateRequest(Guid? responsibleId = null) => new(
        Banner: "novobanner",
        Name: "Bazar de Verão",
        Description: "Nova descrição.",
        OrganizerName: "Bruno Lima",
        Address: new AddressRequest(
            Cep: "20040020",
            Street: "Avenida Rio Branco",
            Neighborhood: "Centro",
            City: "Rio de Janeiro",
            StateCode: "RJ",
            State: "Rio de Janeiro",
            Complement: null
        ),
        StartDate: EventFactory.BaseDate.AddDays(7),
        EndDate: EventFactory.BaseDate.AddDays(7).AddHours(6),
        ResponsibleStoreId: responsibleId ?? Guid.NewGuid()
    );

    [Fact]
    public async Task UpdateEventAsync_WhenUserOwnsStore_UpdatesEvent()
    {
        var ownerId = Guid.NewGuid();
        var store = ArrangeStore(ownerId);
        var storeEvent = ArrangeEvent(store.Id);

        _currentUser.UserId.Returns(ownerId);

        var result = await _service.UpdateEventAsync(storeEvent.Id, NewUpdateRequest(), CancellationToken.None);

        Assert.False(_notifier.HasNotifications());
        Assert.Equal("Bazar de Verão", result?.Name);
        Assert.Equal("Bazar de Verão", storeEvent.Name);
    }

    [Fact]
    public async Task DeleteEventAsync_WhenUserOwnsStore_DeletesEvent()
    {
        var userId = Guid.NewGuid();
        var store = ArrangeStore(userId);
        var storeEvent = ArrangeEvent(store.Id);

        _currentUser.UserId.Returns(userId);

        var result = await _service.DeleteEventAsync(storeEvent.Id, CancellationToken.None);

        Assert.True(result);
        Assert.NotNull(storeEvent.DeletedAt);

        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateEventAsync_WhenUserOwnsStore_CreatesEvent()
    {
        var ownerId = Guid.NewGuid();
        var store = ArrangeStore(ownerId);
        var createRequest = NewCreateRequest(store.Id);

        _currentUser.UserId.Returns(ownerId);

        var result = await _service.CreateEventAsync(createRequest, CancellationToken.None);

        Assert.False(_notifier.HasNotifications());
        Assert.Equal("Bazar de Verão", result?.Name);
        Assert.Equal(store.Id, result?.ResponsibleStoreId);
        Assert.Equal("20040020", result?.Address.Cep);
    }

    [Fact]
    public async Task CreateEventAsync_WhenUserDoesNotOwnStore_DoesNotCreateEvent()
    {
        var store = ArrangeStore(Guid.NewGuid());
        var createRequest = NewCreateRequest(store.Id);

        _currentUser.UserId.Returns(Guid.NewGuid());

        var result = await _service.CreateEventAsync(createRequest, CancellationToken.None);

        Assert.Contains(_notifier.GetNotifications(), n => n.Code == "EventAccessDenied");
        Assert.Null(result);

        await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateEventAsync_WhenStoreDoesNotExist_DoesNotCreateEvent()
    {
        var createRequest = NewCreateRequest();

        var result = await _service.CreateEventAsync(createRequest, CancellationToken.None);

        Assert.Contains(_notifier.GetNotifications(), n => n.Code == "ResponsibleStoreId");
        Assert.Null(result);

        await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateEventAsync_WhenUserDoesNotOwnStore_DoesNotUpdateEvent()
    {
        var store = ArrangeStore(Guid.NewGuid());
        var storeEvent = ArrangeEvent(store.Id);
        var originalName = storeEvent.Name;

        _currentUser.UserId.Returns(Guid.NewGuid());

        var result = await _service.UpdateEventAsync(storeEvent.Id, NewUpdateRequest(), CancellationToken.None);
        
        Assert.Contains(_notifier.GetNotifications(), n => n.Code == "EventAccessDenied");
        Assert.Null(result);
        Assert.Equal("Bazar de Primavera", storeEvent.Name);

        await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteEventAsync_WhenUserDoesNotOwnStore_DoesNotDeleteEvent()
    {
        var store = ArrangeStore(Guid.NewGuid());
        var storeEvent = ArrangeEvent(store.Id);

        _currentUser.UserId.Returns(Guid.NewGuid());

        var result = await _service.DeleteEventAsync(storeEvent.Id, CancellationToken.None);

        Assert.Contains(_notifier.GetNotifications(), n => n.Code == "EventAccessDenied");
        Assert.Null(storeEvent.DeletedAt);
    }
}
