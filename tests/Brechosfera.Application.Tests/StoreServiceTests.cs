using Brechosfera.Application.Common;
using Brechosfera.Application.Interfaces;
using Brechosfera.Application.Mapper;
using Brechosfera.Application.Messages;
using Brechosfera.Application.Notifications;
using Brechosfera.Application.Requests;
using Brechosfera.Application.Services;
using Brechosfera.Domain.Abstractions;
using Brechosfera.Domain.Entities;
using Brechosfera.Domain.Enums;
using Brechosfera.Domain.Repositories;
using Brechosfera.Tests.Common.Factories;
using Microsoft.Extensions.Caching.Distributed;
using NSubstitute;
using System.Text.Json;

namespace Brechosfera.Application.Tests;

public class StoreServiceTests
{
    private readonly IStoreRepository _storeRepository = Substitute.For<IStoreRepository>();
    private readonly IEventRepository _eventRepository = Substitute.For<IEventRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IIdentityService _identity = Substitute.For<IIdentityService>();
    private readonly IMessagePublisher _publisher = Substitute.For<IMessagePublisher>();
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private readonly IDistributedCache _cache = Substitute.For<IDistributedCache>();
    private readonly Notifier _notifier = new();
    private readonly StoreService _service;

    public StoreServiceTests()
    {
        _service = new StoreService(
            publisher: _publisher,
            unitOfWork: _unitOfWork,
            notifier: _notifier,
            identity: _identity,
            repository: _storeRepository,
            productTypeRepository: Substitute.For<IProductTypeRepository>(),
            currentUser: _currentUser,
            storeDeletion: new StoreDeletion(_eventRepository),
            cache: _cache
        );

        _unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(true);
    }

    public static TheoryData<string, Func<StoreService, Guid, Task>> StoreChanges => new()
    {
        { "AddPhone",               (s, id) => s.AddPhoneAsync(id, new PhoneRequest("11999990001", false), CancellationToken.None) },
        { "RemovePhone",            (s, id) => s.RemovePhoneAsync(id, new RemovePhoneRequest("11999990001"), CancellationToken.None) },
        { "AddSocialMedia",         (s, id) => s.AddSocialMediaAsync(id, new SocialMediaRequest("@brecho", ESocialMediaPlatform.Instagram), CancellationToken.None) },
        { "RemoveSocialMedia",      (s, id) => s.RemoveSocialMediaAsync(id, new RemoveSocialMediaRequest("@brecho"), CancellationToken.None) },
        { "UpdateAddress",          (s, id) => s.UpdateAddressAsync(id, new AddressRequest("01001000", "Praça da Sé", "Sé", "São Paulo", "SP", "São Paulo", null), CancellationToken.None) },
        { "UpdateProductTypes",     (s, id) => s.UpdateProductTypesAsync(id, new ReplaceStoreProductTypesRequest([Guid.NewGuid()]), CancellationToken.None) }, 
        { "UpdateStore",            (s, id) => s.UpdateStoreAsync(id, new UpdateStoreRequest("Brechó Teste", 50m, ELocalType.Physical, null, null), CancellationToken.None) }
    };

    [Theory]
    [MemberData(nameof(StoreChanges))]
    public async Task StoreChange_WhenUserIsNotOwner_NotifiesAccessDenied(
        string operation, Func<StoreService, Guid, Task> act)
    {
        var store = StoreFactory.Create(ownerUserId: Guid.NewGuid());

        _storeRepository.GetStoreByIdAsync(store.Id, Arg.Any<CancellationToken>()).Returns(store);
        _storeRepository.GetByIdAsync(store.Id, Arg.Any<CancellationToken>()).Returns(store);
        _currentUser.UserId.Returns(Guid.NewGuid());

        await act(_service, store.Id);

        Assert.True(
            _notifier.GetNotifications().Any(n => n.Code == "STORE_ACCESS_DENIED"),
            $"{operation} não bloqueou um usuário que não é dono da loja.");

        await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }

    [Theory]
    [MemberData(nameof(StoreChanges))]
    public async Task StoreChange_WhenStoreDoesNotExist_NotifiesStoreNotFound(
        string operation, Func<StoreService, Guid, Task> act)
    {
        await act(_service, Guid.NewGuid());

        Assert.True(
            _notifier.GetNotifications().Any(n => n.Code == "STORE_NOT_FOUND"),
            $"{operation} não notificou loja inexistente.");
    }

    [Fact]
    public async Task GetStoreByIdAsync_WhenRedisCacheHit_GetsStoryByRedis()
    {
        var store = StoreFactory.Create();
        var storeSerialized = JsonSerializer.Serialize(store.ToResponse());
        var storeSerializedBytes = System.Text.Encoding.UTF8.GetBytes(storeSerialized);

        _storeRepository.GetStoreByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(store);
        _cache.GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(storeSerializedBytes);

        var result = await _service.GetStoreByIdAsync(store.Id, CancellationToken.None);

        await _storeRepository.DidNotReceiveWithAnyArgs().GetStoreByIdAsync(
            Arg.Any<Guid>(), Arg.Any<CancellationToken>());

        await _cache.DidNotReceiveWithAnyArgs().SetStringAsync(
            "storeId",
            "store_serialized",
            Arg.Any<DistributedCacheEntryOptions>(),
            Arg.Any<CancellationToken>());

        Assert.Equal(store.Name, result?.Name);
    }

    [Fact]
    public async Task GetStoreByIdAsync_WhenRedisCacheMiss_SetToRedis()
    {
        var store = StoreFactory.Create();
        var cacheKey = $"store:{store.Id}";

        _storeRepository
            .GetStoreByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(store);

        var result = await _service.GetStoreByIdAsync(store.Id, CancellationToken.None);

        await _cache.ReceivedWithAnyArgs().SetStringAsync(
            cacheKey,
            "store_serialized",
            Arg.Any<DistributedCacheEntryOptions>(),
            Arg.Any<CancellationToken>());

        Assert.Equal(store.Name, result?.Name);
    }

    [Fact]
    public async Task DeleteStoreAsync_WhenDeleteStore_RemoveCacheStoreRedis()
    {
        var userId = Guid.NewGuid();
        var store = StoreFactory.Create(userId);

        _storeRepository
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(store);
        _currentUser.UserId.Returns(userId);

        var result = await _service.DeleteStoreAsync(store.Id, CancellationToken.None);

        await _cache.Received(1).RemoveAsync(
            Arg.Is<string>(g => g == $"store:{store.Id}"),
            Arg.Any<CancellationToken>());

        Assert.True(result);
    }

    [Fact]
    public async Task CreateStoreAsync_WhenNameAlreadyExists_DoesNotCreateStore()
    {
        _storeRepository.ExistsByNameAsync("Brechó Aurora", Arg.Any<Guid?>(), Arg.Any<CancellationToken>()).Returns(true);

        var request = new CreateStoreRequest(
            Name: " Brechó Aurora ",
            AveragePrice: 50m,
            LocalType: ELocalType.Physical,
            ProductTypeKeys: ["camisetas"],
            AverageQuantity: null,
            Description: null,
            Address: null,
            SocialMedia: null,
            Phones: null
        );

        var result = await _service.CreateStoreAsync(request, CancellationToken.None);

        Assert.Null(result);
        Assert.Contains(_notifier.GetNotifications(), n => n.Code == "Name");
        _storeRepository.DidNotReceive().Add(Arg.Any<Store>());
    }

    [Fact]
    public async Task DeleteStoreAsync_WhenUserIsOwner_DeletesStoreAndNotifiesOwner()
    {
        var ownerId = Guid.NewGuid();
        var store = StoreFactory.Create(ownerUserId: ownerId);
        var owner = new AuthUser(ownerId, "ana", "ana@email.com", []);

        _storeRepository.GetByIdAsync(store.Id, Arg.Any<CancellationToken>()).Returns(store);
        _currentUser.UserId.Returns(ownerId);
        _identity.GetByIdAsync(ownerId).Returns(owner);
        _eventRepository.ListByStoreIdsAsync(Arg.Any<IReadOnlyCollection<Guid>>(), Arg.Any<CancellationToken>()).Returns(new List<Event>());

        var result = await _service.DeleteStoreAsync(store.Id, CancellationToken.None);

        Assert.True(result);
        Assert.NotNull(store.DeletedAt);
        Assert.False(_notifier.HasNotifications());

        await _publisher.Received(1).PublishAsync(
            Queues.StoreDeleted,
            Arg.Is<StoreDeletedMessage>(m => m.Email == "ana@email.com" && m.StoreName == store.Name),
            Arg.Any<CancellationToken>()
        );
    }

    [Fact]
    public async Task DeleteStoreAsync_WhenUserIsNotOwner_DoesNotDeleteNorSendEmail()
    {
        var store = StoreFactory.Create(ownerUserId: Guid.NewGuid());
        _storeRepository.GetByIdAsync(store.Id, Arg.Any<CancellationToken>()).Returns(store);
        _currentUser.UserId.Returns(Guid.NewGuid());

        var result = await _service.DeleteStoreAsync(store.Id, CancellationToken.None);

        Assert.False(result);
        Assert.Null(store.DeletedAt);
        Assert.True(_notifier.HasNotifications());

        await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
        await _publisher.DidNotReceiveWithAnyArgs().PublishAsync<StoreDeletedMessage>(default!, default!, default!);
    }
}
