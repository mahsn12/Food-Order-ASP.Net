namespace FoodDelivery.API.Models.Auth;

public record RegisterRequest(string FullName, string Email, string Password, string? PhoneNumber);
public record LoginRequest(string Email, string Password);
public record ForgotPasswordRequest(string Email);
public record ResetPasswordRequest(string Email, string Token, string NewPassword);
public record ExternalLoginRequest(string Provider, string ProviderToken, string Email, string FullName);
public record AuthResponse(string UserId, string FullName, string Email, string Token);
public record ProfileResponse(string UserId, string FullName, string Email, string? PhoneNumber, DateTime CreatedAt);
