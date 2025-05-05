using CommandLine;

namespace Project_Scaffolder.API.CommandLine;

#pragma warning disable CS1591

public class CmdOptions
{
    // Omitting long name, defaults to name of property, ie "--verbose"
    [Option(
        'v',
        "verbose",
        Default = 2,
        HelpText = 
            """
            Prints all messages to standard output. The default behaviour without this setting is to only log out errors

            When passed a value the verbosity is set to that level.  These levels are based on the Serilog Logging
            levels. Provided is this table taken from, https://github.com/serilog/serilog/wiki/Configuration-Basics
                0.Verbose: Verbose is the noisiest level, rarely (if ever) enabled for a production app.
                1.Debug: Debug is used for internal system events that are not necessarily observable from the outside,
                    but useful when determining how something happened.
                2.Information: Information events describe things happening in the system that correspond to its
                    responsibilities and functions. Generally these are the observable actions the system can perform.
                3.Warning: When service is degraded, endangered, or may be behaving outside of its expected parameters,
                    Warning level events are used.
                4.Error: When functionality is unavailable or expectations broken, an Error event is used.
                5.Fatal: The most critical level, Fatal events demand immediate attention.
            """)]
    public int Verbose { get; set; }
    public bool UseVerboseLogging { get; set; } = false;
}

#pragma warning restore CS1591