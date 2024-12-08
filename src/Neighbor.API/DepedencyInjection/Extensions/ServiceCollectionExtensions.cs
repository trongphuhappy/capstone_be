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
        var accessToken = "1VNNyAnz2r67DZTJGQQ-qveWKseDRZsLHei9O02XeP9kpbiSBeQx-Bqm0c9cdtIn3DZdrwzPIMfVSVN1Z6dhJakqbjQPAtZ-sm3FzwPSvnsRsuqIWxOQ-KgD1MvJqpa48cWCUBOtXBy5f6hk-zU6qIyIjCeeruZxDFq39MhlHOs";

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
               ValidateIssuer = true,
               ValidateAudience = true,
               ValidateLifetime = true,
               ValidateIssuerSigningKey = true,
               ValidIssuer = authenticationSetting.Issuer ?? "Jwt",
               ValidAudience = authenticationSetting.Audience ?? "Jwt",
               IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSetting.AccessSecretToken ?? accessToken)),
               ClockSkew = TimeSpan.Zero
           };

           options.Events = new JwtBearerEvents
           {
               OnAuthenticationFailed = context =>
               {
                   Console.WriteLine($"Authentication failed: {context.Exception.Message}");
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
