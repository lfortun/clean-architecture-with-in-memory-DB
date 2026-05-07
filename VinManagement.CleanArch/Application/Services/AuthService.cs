using Application.DTOs.Auth;
using Application.Services;
using Domain.Common;
using Domain.Services;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly ICredentialValidator _credentialValidator;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public AuthService(ICredentialValidator credentialValidator, IJwtTokenGenerator tokenGenerator)
    {
        _credentialValidator = credentialValidator;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto, CancellationToken cancellationToken = default)
    {
        var isValid = await _credentialValidator.IsValidAsync(dto.Username, dto.Password, cancellationToken);
        if (!isValid)
        {
            throw new BadRequestException("Invalid username or password.");
        }

        var role = dto.Username.ToLowerInvariant() == "admin" ? "Admin" : "User";
        var token = _tokenGenerator.GenerateToken(dto.Username, role);
        var expiresAt = _tokenGenerator.GetExpiryTime();

        return new LoginResponseDto(token, "Bearer", expiresAt);
    }
}
