namespace Brechosfera.Application.Requests;

public sealed record RegisterRequest(string Email, string Username, string Password);
