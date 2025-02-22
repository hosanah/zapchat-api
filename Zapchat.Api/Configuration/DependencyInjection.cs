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
using Zapchat.Domain.Interfaces.FluxoCaixa;
using Zapchat.Service.Services.FluxoCaixa;
using Zapchat.Domain.Interfaces.ContasReceber;
using Zapchat.Service.Services.ContasReceber;
using Zapchat.Service.Services.Categoria;
using Zapchat.Domain.Interfaces.Categoria;

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
            services.AddScoped<IFluxoCaixaService, FluxoCaixaService>();
            services.AddScoped<IClientesService, ClientesService>();
            services.AddScoped<IUtilsService, UtilsService>();
            services.AddScoped<IContasReceberService, ContasReceberService>();
            services.AddScoped<ICategoriaService, CategoriaService>();
            services.AddScoped<IAdmsGrupoRepository, AdmsGrupoRepository>();
            services.AddScoped<IParametroSistemaRepository, ParametroSistemaRepository>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            return services;
        }

    }
}
