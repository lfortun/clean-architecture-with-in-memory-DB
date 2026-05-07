namespace Application.DTOs.Auth;

public record LoginResponseDto(
    string Token,
    string TokenType,
    DateTime ExpiresAt);
