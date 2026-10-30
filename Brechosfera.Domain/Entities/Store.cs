using Brechosfera.Domain.Abstractions;
using Brechosfera.Domain.Enums;
using Brechosfera.Domain.Guards;
using Brechosfera.Domain.ValueObjects;

namespace Brechosfera.Domain.Entities;

public class Store : Entity, IAggregateRoot
{
    public const int MaxPhonesLimit = 3;

    private readonly List<StoreReview> _reviews = new();
    private readonly List<Phone> _phones = new();
    private readonly List<ProductType> _productTypes = new();
    private readonly List<SocialMedia> _socialMedia = new();

    public Guid OwnerUserId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public decimal AveragePrice { get; private set; }
    public ELocalType LocalType { get; private set; }
    public int? AverageQuantity { get; private set; }
    public string? Description { get; private set; }
    public Address? Address { get; private set; }

    public IReadOnlyCollection<StoreReview> Reviews => _reviews.AsReadOnly();
    public IReadOnlyCollection<Phone> Phones => _phones.AsReadOnly();
    public IReadOnlyCollection<ProductType> ProductTypes => _productTypes.AsReadOnly();
    public IReadOnlyCollection<SocialMedia> SocialMedia => _socialMedia.AsReadOnly();

    private Store() { }

    public Store(
        Guid ownerUserId,
        string name, 
        decimal averagePrice, 
        ELocalType localType, 
        List<ProductType> productTypes,
        string? description, 
        int? averageQuantity,
        List<SocialMedia>? socialMedia = null,
        IEnumerable<Phone>? phones = null
    )
    {
        Guard.AgainstNullOrWhiteSpace(name, "STORE_NAME_INVALID", "Nome da loja inválida.");
        Guard.AgainstEmptyGuid(ownerUserId, "STORE_OWNER_INVALID", "O usuário responsável pela loja é inválido.");
        Guard.AgainstEmptyList(productTypes, "PRODUCT_TYPE_EMPTY_LIST", "Informe os tipos de produto.");
        Guard.AgainstNegativeOrZero(averageQuantity, "STORE_AVERAGE_QUANTITY_MORE_ZERO", "Quantidade média deve ser maior que zero.");
        Guard.AgainstNegativeOrZero(averagePrice, "STORE_AVERAGE_PRICE_MORE_ZERO", "Preço médio deve ser  maior que zero.");
        Guard.AgainstInvalidEnum(localType, "STORE_LOCAL_TYPE_INVALID", "O tipo de local é inválido.");

        Name = name;
        Description = description;
        OwnerUserId = ownerUserId;
        AverageQuantity = averageQuantity;
        AveragePrice = averagePrice;
        LocalType = localType;
        AddProductTypes(productTypes);

        if (socialMedia is not null)
        {
            AddListSocialMedia(socialMedia);
        }

        if (phones is not null)
        {
            AddListPhone(phones);
        }
    }

    public void UpdateDetails(
        string name,
        decimal averagePrice,
        ELocalType localType,
        int? averageQuantity,
        string? description
    )
    {
        Guard.AgainstNegativeOrZero(averagePrice, "STORE_AVERAGE_PRICE_MORE_ZERO", "Preço médio deve ser  maior que zero.");
        Guard.AgainstNegativeOrZero(averageQuantity, "STORE_AVERAGE_QUANTITY_MORE_ZERO", "Quantidade média deve ser maior que zero.");
        Guard.AgainstInvalidEnum(localType, "STORE_LOCAL_TYPE_INVALID", "O tipo de local é inválido.");
        Guard.AgainstNullOrWhiteSpace(name, "STORE_NAME_INVALID", "Nome da loja inválida.");

        Name = name;
        AveragePrice = averagePrice;
        LocalType = localType;
        AverageQuantity = averageQuantity;
        Description = description;
    }

    public void ReplaceProductTypes(IEnumerable<ProductType> productTypes)
    {
        Guard.AgainstEmptyList(productTypes, "PRODUCT_TYPE_EMPTY_LIST", "Informe os tipos de produto.");

        _productTypes.Clear();
        AddProductTypes(productTypes);
    }

    public void AddProductType(ProductType newProductType)
    {
        Guard.AgainstNull(newProductType, "PRODUCT_TYPE_EMPTY", "O tipo do produto não pode ser nula.");

        if (_productTypes.Any(x => x.Id == newProductType.Id))
            return;

        _productTypes.Add(newProductType);
    }

    public void AddProductTypes(IEnumerable<ProductType> productTypes)
    {
        Guard.AgainstEmptyList(productTypes, "PRODUCT_TYPE_EMPTY_LIST", "Informe os tipos de produto.");

        var existingProductTypeIds = ProductTypes
            .Select(x => x.Id)
            .ToHashSet();

        foreach (ProductType productTypeItem in productTypes)
        {
            if (existingProductTypeIds.Contains(productTypeItem.Id))
                continue;

            AddProductType(productTypeItem);
            existingProductTypeIds.Add(productTypeItem.Id);
        }
    }

    public bool CheckIsOwnedBy(Guid userId)
    {
        return OwnerUserId == userId;
    }

    public void UpdatePrimaryPhone(string phoneNumber)
    {
        Guard.AgainstNullOrWhiteSpace(phoneNumber, "PHONE_EMPTY", "O número celular não pode ser nula.");

        var phoneToPromote = _phones.FirstOrDefault(p => p.Number == phoneNumber);

        Guard.AgainstNull(phoneToPromote, "PHONE_NOT_FOUND", $"O celular {phoneNumber} não foi encontrado.");

        if (phoneToPromote is null || phoneToPromote.IsPrimary)
            return;

        var currentPrimary = _phones.FirstOrDefault(p => p.IsPrimary);

        if (currentPrimary is not null)
        {
            _phones.Remove(currentPrimary);
            _phones.Add(currentPrimary with { IsPrimary = false });
        }

        _phones.Remove(phoneToPromote);
        _phones.Add(phoneToPromote with { IsPrimary = true });
    }

    public void RemoveSocialMedia(string link)
    {
        Guard.AgainstNullOrWhiteSpace(link, "SOCIAL_MEDIA_EMPTY", "A rede social não pode ser nula.");
        Guard.AgainstMinEqualCount(_socialMedia.Count, 1, "SOCIAL_MEDIA_MIN", "Tenha pelo menos uma rede social.");

        var socialMedia = _socialMedia.FirstOrDefault(s => s.Link == link);

        Guard.AgainstNull(
            socialMedia,
            "SOCIAL_MEDIA_NOT_FOUND",
            "A rede social não foi encontrada."
        );

        if (socialMedia is not null)
            _socialMedia.Remove(socialMedia);
    }

    public void AddSocialMedia(SocialMedia socialMedia)
    {
        Guard.AgainstNull(socialMedia, "SOCIAL_MEDIA_EMPTY", "A rede social não pode ser nula.");

        if (_socialMedia.Any(s => s.Link == socialMedia.Link))
            return;

        _socialMedia.Add(socialMedia);
    }

    public void AddListSocialMedia(IEnumerable<SocialMedia> socialMedia)
    {
        Guard.AgainstNull(socialMedia, "SOCIAL_MEDIA_EMPTY_LIST", "A lista de redes sociais não pode ser nula.");

        foreach (SocialMedia socialMediaItem in socialMedia)
        {
            AddSocialMedia(socialMediaItem);
        }
    }

    public void AddListPhone(IEnumerable<Phone> phones)
    {
        Guard.AgainstNull(phones, "PHONE_EMPTY_LIST", "A lista de celulares não pode ser nula.");

        var phonesToAdd = phones.ToList();

        var primaryPhonesCount = _phones.Count(x => x.IsPrimary) + phonesToAdd.Count(x => x.IsPrimary);

        Guard.AgainstMaxCount(primaryPhonesCount, 1, "STORE_MULTIPLE_PRIMARY", "A loja pode possuir apenas um celular principal.");

        foreach (Phone phone in phonesToAdd)
        {
            AddPhone(phone);
        }
    }

    public void RemovePhone(string phoneNumber)
    {
        Guard.AgainstNullOrWhiteSpace(
            phoneNumber,
            "PHONE_EMPTY",
            "O número do celular é obrigatório."
        );

        var phone = _phones.FirstOrDefault(p => p.Number == phoneNumber);

        Guard.AgainstNull(
            phone,
            "PHONE_NOT_FOUND",
            "O celular não foi encontrado."
        );

        if (phone is not null)
        {
            var wasPrimary = phone.IsPrimary;

            _phones.Remove(phone);

            if (wasPrimary && _phones.Count > 0)
               _phones[0].IsPrimary = true;
        }
    }

    public void AddPhone(Phone phone)
    {
        Guard.AgainstNull(phone, "PHONE_EMPTY", "O celular não pode ser nulo.");
        Guard.AgainstMaxCount(_phones.Count + 1, MaxPhonesLimit, "STORE_PHONE_LIMIT_EXCEEDED", $"A loja já atingiu o limite de {MaxPhonesLimit} celulares.");
        Guard.AgainstDuplicate(_phones, p => p.Number == phone.Number, "STORE_PHONE_DUPLICATED", "O celular já existe.");

        if (_phones.Count == 0 || phone.IsPrimary)
            SetPrimaryPhone(phone);

        _phones.Add(phone);
    }

    public void SetPrimaryPhone(Phone phone)
    {
        foreach (var existingPhone in _phones)
        {
            existingPhone.IsPrimary = false;
        }

        phone.IsPrimary = true;
    }

    public void AddReview(StoreReview review)
    {
        Guard.AgainstNull(review, "STORE_REVIEW_EMPTY", "A avaliação não pode ser nula.");

        _reviews.Add(review);
    }

    public void SetAddress(Address address)
    {
        Guard.AgainstNull(address, "STORE_ADDRESS_EMPTY", "A localização não pode ser nula.");

        Address = address;
    }
}
