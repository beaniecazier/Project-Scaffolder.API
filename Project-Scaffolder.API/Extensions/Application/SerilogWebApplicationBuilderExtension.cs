using Project_Scaffolder.API.CommandLine;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Project_Scaffolder.API.Extensions.Application;

public static class SerilogWebApplicationBuilderExtension
{
    public static void SetLogLevelFromOptions(this WebApplicationBuilder builder, CmdOptions options,
        LoggingLevelSwitch levelSwitch)
    {
        if (builder.Environment.IsDevelopment()) return;
        if (!CommandLineApplicationExtension.BeVerbose) return;
        if (options.Verbose < 0 || options.Verbose > 5)
        {
            throw new ArgumentOutOfRangeException("options.Verbose",
                options.Verbose,
                "Invalid Serilog logging level found while trying to set the log level");            
        }

        levelSwitch.MinimumLevel = (LogEventLevel)options.Verbose;
    }
    
    public static void AddLoggingWithSerilog(this WebApplicationBuilder builder, LoggingLevelSwitch levelSwitch)
    {
        Serilog.ILogger logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .MinimumLevel.ControlledBy(levelSwitch)
            .CreateLogger();
        Log.Logger = logger;
        builder.Host.UseSerilog();
    }
}