namespace Brechosfera.Application.Requests;

public sealed record RegisterUserRequest(string Username, string Email, string Password);
