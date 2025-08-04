using CommandLine;

using Gay.Silverbranch.Api.Utilities.Backend.CommandLine;

using Gay.Silverbranch.ProjectScaffolder.Bll.Constants;

namespace Gay.Silverbranch.ProjectScaffolder.Api.CommandLine;

/// <summary>
/// Full Backend Command Line Options
/// short options
/// h - host
/// p - https port
/// s - subdirectory
/// H - valid host to request health check
/// c - cache expiration timer
/// v - verbose logging/logging level
///
/// long options
/// ready - the readiness probe health check port
/// liveness - the liveness probe health check port
/// startup - the startup probe health check port
/// health-host - 
/// cache-expire - 
/// dbconn - 
/// http - 
/// port - 
/// host - 
/// subdirectory - 
/// verbose - 
/// </summary>
public class ProjectScaffolderCmdOptions : RoutingBackendCmdOptions
{
    /// <summary>
    /// The root path for the location of all scaffold outputs
    /// </summary>
    [Option('o')]
    public string OutputRootPath { get; set; } = CreationTagConstants.OutputRootFolder;
}