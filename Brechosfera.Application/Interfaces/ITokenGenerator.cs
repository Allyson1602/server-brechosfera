using Brechosfera.Domain.Entities;

namespace Brechosfera.Application.Interfaces;

public interface ITokenGenerator
{
    string Generate(UserCredential user);
}
