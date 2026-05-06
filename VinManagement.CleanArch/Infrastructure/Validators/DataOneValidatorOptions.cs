namespace Infrastructure.Validators;

public class DataOneValidatorOptions
{
    public const string SectionName = "VinValidators:DataOne";

    public string ProviderName { get; set; } = string.Empty;
    public bool Enabled { get; set; }
}
