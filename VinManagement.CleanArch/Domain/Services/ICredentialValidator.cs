namespace Domain.Services;

public interface ICredentialValidator
{
    Task<bool> IsValidAsync(string username, string password, CancellationToken cancellationToken = default);
}
