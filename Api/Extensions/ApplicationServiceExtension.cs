using Api.Helper;
using Core.Interfaces;
using Infrastructure.GenericRepository;

namespace Api.Extensions;
public static class ApplicationServiceExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IImageService, ImageService>();
        services.AddAutoMapper(typeof(AutoMpperProfiles).Assembly);
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
}