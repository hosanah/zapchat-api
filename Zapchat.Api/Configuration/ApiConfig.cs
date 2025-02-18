using Microsoft.AspNetCore.Authentication.JwtBearer;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Zapchat.Api.Extensions;

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


            var key = Encoding.ASCII.GetBytes("f0d7d11612e7739e337c73d6f77e990169c57994382df339c9825a9f9bb76ff0bf1cdf395276e6f415c5127f5a56e49f2afa0de5b7f3538681f80e49dea47fcc21db0213a3c9a8d4494d79f92fb11548fcd5a8c22c005ac300bf80484adc77f5");

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
