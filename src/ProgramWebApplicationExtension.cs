using Asp.Versioning;
using Serilog;
using Serilog.Core;
using Serilog.Events;

using Gay.Silverbranch.Api.Bll.Extensions.ServiceCollection;
using Gay.Silverbranch.Api.Utilities.Backend.Extensions.ServiceCollection;
using Gay.Silverbranch.Api.Utilities.Backend.Swagger;
using Gay.Silverbranch.Api.Utilities.Common.CommandLine.Interface;
using Gay.Silverbranch.Api.Utilities.Common.Endpoints;
using Gay.Silverbranch.Api.Utilities.Common.Endpoints.Extensions;
using Gay.Silverbranch.Api.Utilities.Common.Extensions.ServiceCollection;
using Gay.Silverbranch.Utilities.General.BackgroundServices;
using Gay.Silverbranch.Utilities.General.CommandLine;
using Gay.Silverbranch.Utilities.General.Logging;
using Gay.Silverbranch.Utilities.Security.Extensions;

using Gay.Silverbranch.ProjectScaffolder.Api.CommandLine;
using Gay.Silverbranch.ProjectScaffolder.Bll.Constants;
using Gay.Silverbranch.ProjectScaffolder.Bll.Database.V1;

namespace Gay.Silverbranch.ProjectScaffolder.Api;

/// <summary>
/// 
/// </summary>
public static class ProgramWebApplicationExtension
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="defaultApiVersion"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static WebApplication AddApplicationServicesAndConfigurations(
        this WebApplicationBuilder? builder,
        ApiVersion defaultApiVersion,
        string[] args)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(args);
        
        var config = builder!.Configuration;

        var logLevelSwitch = new LoggingLevelSwitch(LogEventLevel.Error);
        if (builder.Environment.IsDevelopment()) logLevelSwitch.MinimumLevel = LogEventLevel.Information;
        builder.AddLoggingWithSerilog(logLevelSwitch);
        builder.Host.UseSerilog();
        
        var opts = CommandLineApplicationExtension.ParseCommandLine<ProjectScaffolderCmdOptions>(args,
            opts =>
            {
                OutputCacheServiceExtension.OutputCacheExpirationInMinutes = opts.CacheExpirationTimeInMinutes;
                //StartupBackgroundService.FakedStartupDurationInSeconds = results.Value.FakedStartupDurationInSeconds;
                ConfigureSwaggerOptions.ContactName = opts.ContactName;
                ConfigureSwaggerOptions.ContactUrl = opts.ContactUrl;
                ConfigureSwaggerOptions.ContactEmail = opts.ContactEmail;
                ConfigureSwaggerOptions.TermsOfServiceUrl = opts.TermsOfServiceUrl;
                StaticRoutingDefinitions.BaseSubdirectory = string.IsNullOrWhiteSpace(opts.Subdirectory) ? 
                    "" :
                    opts.Subdirectory;
                CreationTagConstants.OutputRootFolder = opts.OutputRootPath;
            });
        
        builder.SetLogLevelFromOptions(opts, logLevelSwitch);
        builder.Services.AddSingleton<IHealthCheckCmdOptions>(opts);
        
        //builder.Services.AddJsonConfigurationOptions();

        //builder.Services.AddSecurity(config);
        builder.Services.AddKeycloakAuthApi(config);

        builder.Services.AddApiVersioningSettings(defaultApiVersion);

        builder.Services.AddOutputAndResponseCacheing();

        builder.Services.ConfigureAndAddSwagger(config);

        builder.Services.AddApplication();

        string dbConn = "Scaffolder:ConnectionString";
        //string dbConn = "";
        builder.Services.AddDatabase<ProjectScaffolderDbContext>(config, dbConn, verbose:true);

        builder.Services.AddHealthCheckServicesBackend<ProjectScaffolderDbContext>();
        builder.Services.AddApplicationEndpoints<IApiMarker>(config);
        builder.Services.AddHostedService<StartupBackgroundService>();
        
        return builder.Build();
    }
}