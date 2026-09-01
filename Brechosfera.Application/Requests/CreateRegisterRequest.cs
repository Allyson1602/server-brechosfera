namespace Brechosfera.Application.Requests;

public sealed record CreateRegisterRequest(string Email, string Username, string Password);
