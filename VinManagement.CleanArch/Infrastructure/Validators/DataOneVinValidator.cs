using Domain.Services;
using Microsoft.Extensions.Options;

namespace Infrastructure.Validators;

public class DataOneVinValidator : IVinValidator
{
    private readonly DataOneValidatorOptions _options;

    public DataOneVinValidator(IOptions<DataOneValidatorOptions> options)
    {
        _options = options.Value;
    }

    public string ProviderName => _options.ProviderName;

    public async Task<bool> IsValidAsync(string vin, CancellationToken cancellationToken = default)
    {
        await Task.Delay(100, cancellationToken);
        return vin.Length == 17;
    }
}
