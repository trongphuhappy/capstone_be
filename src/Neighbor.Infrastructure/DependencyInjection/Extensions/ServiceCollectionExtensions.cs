using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Neighbor.Contract.Abstractions.Services;
using Neighbor.Contract.Settings;
using Neighbor.Infrastructure.Services;
using StackExchange.Redis;

namespace Neighbor.Infrastructure.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddConfigurationRedis
        (this IServiceCollection services, IConfiguration configuration)
    {
        var redisSettings = new RedisSetting();
        configuration.GetSection(RedisSetting.SectionName).Bind(redisSettings);
        services.AddSingleton<RedisSetting>();
        if (!redisSettings.Enabled) return;
        services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisSettings.ConnectionString));
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisSettings.ConnectionString;
        });
        services.AddSingleton<IResponseCacheService, ResponseCacheService>();
    }

    public static void AddConfigurationService(this IServiceCollection services)
    {
        services.AddSingleton<IEmailService, EmailService>()
                .AddTransient<ITokenGeneratorService, TokenGeneratorService>()
                .AddTransient<IPasswordHashService, PasswordHashService>()
                .AddTransient<IMediaService, MediaService>();
    }

    public static void AddConfigurationAppSetting
        (this IServiceCollection services, IConfiguration configuration)
    {
        services
            .Configure<AuthenticationSetting>(configuration.GetSection(AuthenticationSetting.SectionName))
            .Configure<EmailSetting>(configuration.GetSection(EmailSetting.SectionName))
            .Configure<ClientSetting>(configuration.GetSection(ClientSetting.SectionName))
            .Configure<UserSetting>(configuration.GetSection(UserSetting.SectionName))
            .Configure<CloudinarySetting>(configuration.GetSection(CloudinarySetting.SectionName));
    }
}