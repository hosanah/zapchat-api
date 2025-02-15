using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using FluentValidation;
using Zapchat.Domain.DTOs;
using Zapchat.Domain.Interfaces.ContasPagar;
using Zapchat.Domain.Interfaces;
using Zapchat.Repository.Repositories;
using Zapchat.Service.Services.ContasPagar;
using Zapchat.Service.Services;
using Zapchat.Service.Validations;
using static Zapchat.Api.Configuration.SwagguerConfig;
using Zapchat.Domain.Interfaces.Clientes;
using Zapchat.Service.Services.Clientes;
using Zapchat.Domain.Interfaces.Messages;
using Zapchat.Domain.Notifications;

namespace Zapchat.Api.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection ResolveDependencies(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddScoped<INotificator, Notificator>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IGrupoWhatsAppRepository, GrupoWhatsAppRepository>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IGrupoWhatsAppService, GrupoWhatsAppService>();
            services.AddScoped<IValidator<UsuarioDto>, UsuarioValidator>();
            services.AddScoped<IContasPagarService, ContasPagarService>();
            services.AddScoped<IClientesService, ClientesService>();
            services.AddScoped<IUtilsService, UtilsService>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            return services;
        }

    }
}
