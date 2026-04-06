using NLog.Extensions.Logging;
using SoapCore;
using System.ServiceModel.Channels;
using WIS.Configuration;
using WIS.Domain.Services;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence;
using WIS.XmlData.WInterface.Helpers;
using WIS.XmlData.WInterface.Services;
using WIS.XmlData.WInterface.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSoapCore();

builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection(DatabaseSettings.Position));

builder.Services.AddScoped<IWisInterface, WisInterface>();
builder.Services.AddScoped<ILoggerProvider, NLogLoggerProvider>();
builder.Services.AddScoped<IDatabaseFactory, DatabaseFactory>();
builder.Services.AddScoped<IDapper, DapperService>();
builder.Services.AddScoped<IDatabaseConfigurationService, DatabaseConfigurationService>();
builder.Services.AddScoped<IXmlDataExternalManager, XmlDataExternalManager>();
builder.Services.AddScoped<IXmlDataQueryManager, XmlDataQueryManager>();
builder.Services.AddScoped<WisInterface>();

builder.Services.AddControllers();

builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.SetMinimumLevel(LogLevel.Trace);
});

var messageVersion = builder.Configuration.GetValue<string>("AppSettings:MessageVersion");
var app = builder.Build();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.UseSoapEndpoint<IWisInterface>(opt =>
    {
        opt.Path = "/WisInterface.asmx";
        opt.SoapSerializer = SoapSerializer.XmlSerializer;
        opt.EncoderOptions = new SoapEncoderOptions[] {
            new SoapEncoderOptions {
                MessageVersion = messageVersion == "Soap11" ? MessageVersion.Soap11 : MessageVersion.Soap12WSAddressing10,
                BindingName = "WisInterfaceSoap",
                PortName = "WisInterfaceSoap",
            },
        };
    });
});

app.MapControllers();

app.Run();
