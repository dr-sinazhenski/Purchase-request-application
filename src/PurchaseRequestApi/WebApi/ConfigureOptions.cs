using Infrastructure.Database;
using Microsoft.AspNetCore.SignalR;
using Application.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace WebApi
{
    public static class ConfigureOptions
    {
        public static void ConfigureProjectsOptions(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<DbOptions>(
                builder.Configuration.GetSection(nameof(DbOptions)));
            builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));
            var jwtOptions = builder.Configuration.GetSection("JwtOptions").Get<JwtOptions>();
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret))
                };
            });
        }
    }
}
