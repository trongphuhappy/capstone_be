using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Neighbor.Contract.Settings;
using System.Text;

namespace Neighbor.API.DepedencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthenticationAndAuthorization(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSignalR()
          .AddJsonProtocol(options =>
          {
              options.PayloadSerializerOptions.PropertyNamingPolicy = null;
          });

        var authenticationSetting = new AuthenticationSetting();
        configuration.GetSection(AuthenticationSetting.SectionName).Bind(authenticationSetting);


        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
       .AddJwtBearer(options =>
       {
           options.SaveToken = true;
           options.TokenValidationParameters = new TokenValidationParameters
           {
               ValidateIssuer = false,
               ValidateAudience = false,
               ValidateLifetime = true,
               ValidateIssuerSigningKey = true,
               ValidIssuer = authenticationSetting.Issuer,
               ValidAudience = authenticationSetting.Audience,
               IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSetting.AccessSecretToken)),
               ClockSkew = TimeSpan.Zero
           };

           options.Events = new JwtBearerEvents
           {
               OnAuthenticationFailed = context =>
               {
                   if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                   {
                       context.Response.Headers.Add("IS-TOKEN-EXPIRED", "true");
                   }
                   return Task.CompletedTask;
               },
           };
       });

        services.AddAuthorization(options =>
        {
        });

        return services;
    }
}
