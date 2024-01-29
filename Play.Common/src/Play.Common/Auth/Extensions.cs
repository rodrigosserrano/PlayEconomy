using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Play.Common.Settings;

namespace Play.Common.Auth;

public static class Extensions
{
    public static IServiceCollection AddAuth(this IServiceCollection services)
    {
        // var authSettings = configuration.GetSection(nameof(AuthSettings)).Get<AuthSettings>()!;

        var authSettings = new AuthSettings
        {
            Key = "0pqVdXBxfBpkTZuVoPh1bjoIiOyVx1Uj",
            Audience = [
                "https://localhost:7103",
                "https://localhost:7194"
            ],
            ExpirationTimeSeconds = 3600,
            Issuer = "https://localhost:7103"
        };

        // JWT
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = authSettings.Issuer,
                ValidAudiences = authSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(authSettings.Key)),
            };

            // TODO
            // options.Events = new JwtBearerEvents
            // {
            //     OnAuthenticationFailed = context =>
            //     {
            //         whenFailure();
            //         return Task.CompletedTask;
            //     }
            // };
        });

        // Resolve DI 
        services.AddScoped(_ => new Auth());

        return services;
    }
}