using CommandLine;
using Project_Scaffolder.API.CommandLine;
using Serilog;

namespace Project_Scaffolder.API.Extensions.Application;

#pragma warning disable CS1591

public static class CommandLineApplicationExtension
{
    public static bool BeVerbose = false;
    
    public static CmdOptions ParseCommandLine(string[] args)
    {
        if (args.Contains("-v") || args.Contains("--verbose")) BeVerbose = true;
        
        var argsList = args.Where(x => !x.Contains("--applicationName") &&
                                    !x.Contains("--environment") &&
                                    !x.Contains("--contentRoot"));
        
        var results = Parser.Default.ParseArguments<CmdOptions>(argsList)
            .WithParsed<CmdOptions>(RunOptions)
            .WithNotParsed(HandleParseError);

        return results.Value;
    }

    static void RunOptions(CmdOptions opts)
    {
    }

    static void HandleParseError(IEnumerable<Error> errs)
    {
        foreach (var error in errs) Log.Error("", error);
        throw new Exception("One or more errors occurred.\n" +
                            "The provided cli arguments are not valid.\n" +
                            "Terminating program.");
    }
}

#pragma warning restore CS1591