using Microsoft.OpenApi.Models;
using Zapchat.Repository.Data;
using Zapchat.Service.Mappings;
using Microsoft.EntityFrameworkCore;
using Zapchat.Api.Configuration;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Asp.Versioning.ApiExplorer;

var builder = WebApplication.CreateBuilder(args);

// Configura��o do DbContext com SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Inje��o de depend�ncia
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerConfig();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.ResolveDependencies();
builder.Services.WebApiConfig(builder.Configuration);
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options => { options.SuppressModelStateInvalidFilter = true; }).AddJsonOptions(x => { x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault; });
var app = builder.Build();
var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

// Executar migra��es apenas em ambiente de produ��o
if (!app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Verifica se o banco de dados existe
        if (!dbContext.Database.CanConnect())
        {
            dbContext.Database.EnsureCreated(); // Cria o banco se n�o existir
        }
        else if (dbContext.Database.GetPendingMigrations().Any())
        {
            dbContext.Database.Migrate(); // Aplica as migra��es pendentes
        }
    }
}


// Configure the HTTP request pipeline.
app.UseSwaggerConfig(provider);
app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();
app.Run();

