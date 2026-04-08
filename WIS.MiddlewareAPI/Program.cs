using Custom.Domain.DataModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.IO;
using System.Reflection;
using WIS.Configuration;
using WIS.Data;
using WIS.Domain.DataModel;
using WIS.Domain.Services;
using WIS.Domain.Services.Configuracion;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence;
using WIS.Security;
using WIS.MiddlewareAPI.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title       = "Middleware API",
        Version     = "v1",
        Description = "API de entrada para integracion con ERP del cliente. " +
                      "Recibe datos en formato del cliente, los traduce al formato WIS"
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

builder.Services.AddSingleton<IDatabaseConfigurationService, DatabaseConfigurationService>();
builder.Services.AddSingleton<IDatabaseFactory, DatabaseFactory>();
builder.Services.AddSingleton<IDapper, DapperService>();
builder.Services.AddSingleton<IFactoryService, FactoryService>();
builder.Services.AddScoped<IIdentityService, MiddlewareIdentityService>();
builder.Services.AddScoped<IUnitOfWorkFactory, UnitOfWorkCustomFactory>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection(DatabaseSettings.Position));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Middleware API v1");
    c.RoutePrefix = string.Empty; 
});

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.Run();
