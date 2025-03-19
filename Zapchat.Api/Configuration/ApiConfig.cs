using Microsoft.AspNetCore.Authentication.JwtBearer;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Zapchat.Api.Extensions;
using Microsoft.Extensions.Configuration;

namespace Zapchat.Api.Configuration
{
    public static class ApiConfig
    {
        const string _myAllowSpecificOrigins = "myAllowSpecificOrigins";
        public static IServiceCollection WebApiConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
            }).AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });


            services.AddCors(option =>
            {
                option.AddPolicy(name: _myAllowSpecificOrigins,
                    policy =>
                    {
                        policy
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                    });
            });
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;

            });

            var appSettingsSection = configuration.GetSection("Auth").GetSection("dadosAutenticacao");
            services.Configure<AppSettingsAuth>(appSettingsSection);
            var appSettings = appSettingsSection.Get<AppSettingsAuth>()!;

            var secretEncoded = configuration.GetSection("Auth").GetValue<string>("secret")!;
            byte[] base64Bytes = Convert.FromBase64String(secretEncoded);
            string decodedSecrectc = Encoding.UTF8.GetString(base64Bytes);
            var key = Encoding.ASCII.GetBytes(decodedSecrectc);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false

                };
            });


            return services;
        }

        public static IApplicationBuilder UseMvcConfiguration(this IApplicationBuilder app)
        {
            app.UseHttpsRedirection();
            app.UseCors(_myAllowSpecificOrigins);

            return app;
        }
    }
}
