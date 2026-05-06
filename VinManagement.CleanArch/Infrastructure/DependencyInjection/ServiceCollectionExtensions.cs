using Application.Services;
using Domain.Repositories;
using Domain.Services;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("VinManagementDb"));

        services.AddScoped<IVinRepository, VinRepository>();
        services.AddScoped<IVinService, VinService>();

        services.Configure<DataOneValidatorOptions>(
            configuration.GetSection(DataOneValidatorOptions.SectionName));
        services.AddScoped<IVinValidator, DataOneVinValidator>();

        return services;
    }
}
