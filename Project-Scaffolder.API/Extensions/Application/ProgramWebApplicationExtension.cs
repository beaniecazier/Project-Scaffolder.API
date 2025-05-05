using Serilog.Core;
using Serilog.Events;

namespace Project_Scaffolder.API.Extensions.Application;

public static class ProgramWebApplicationExtension
{
    public static WebApplication AddApplicationServicesAndConfigurations(
        this WebApplicationBuilder? builder,
        string[] args)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(args);
        
        var config = builder!.Configuration;

        var logLevelSwitch = new LoggingLevelSwitch(LogEventLevel.Error);
        if (builder.Environment.IsDevelopment()) logLevelSwitch.MinimumLevel = LogEventLevel.Debug;
        builder.AddLoggingWithSerilog(logLevelSwitch);
        
        var options = CommandLineApplicationExtension.ParseCommandLine(args);
        builder.SetLogLevelFromOptions(options, logLevelSwitch);
        builder.Services.AddSingleton(options);
        
        return builder.Build();
    }
}