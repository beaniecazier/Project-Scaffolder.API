using FluentValidation.Results;
using Serilog;

using Gay.Silverbranch.Api.Models.Enum.V1;
using Gay.Silverbranch.Api.Utilities.Backend.Middleware;
using Gay.Silverbranch.Api.Utilities.Common.Endpoints.Interfaces;
using Gay.Silverbranch.Api.Utilities.Common.Versioning;

using Gay.Silverbranch.ProjectScaffolder.Bll.Services.Interface.Model.V1;
using Gay.Silverbranch.ProjectScaffolder.Contracts.Endpoints.V1;
namespace Gay.Silverbranch.ProjectScaffolder.Api.Endpoints.V1.Post;

/// <summary>
/// 
/// </summary>
public class PostScaffoldApiEndpoint : IEndpoints
{
    private const string ContentType = "application/json";
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
    }
    
    /// <summary>
    /// Map Create Api from meta info endpoint with correct settings
    /// </summary>
    /// <param name="app"></param>
    /// <exception cref="NotImplementedException"></exception>
    public static void DefineEndpoints(IEndpointRouteBuilder app)
    {
        Log.Information("Now mapping Create Api From Metainfo Action Endpoint");
        var singleEndpoint = app
            .MapPost(ApiMetaInfoModelEndpoints.PostScaffoldEndpoint, ScaffoldApiFromMetaInfoAction)
            .WithName(ApiMetaInfoModelEndpoints.ScaffoldEndpointName)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<IEnumerable<ValidationFailure>>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithApiVersionSet(ApiVersioning.VersionSet!)
            //.CacheOutput(ApiMetaInfoModelEndpoints.Tag)
            .WithTags(ApiMetaInfoModelEndpoints.Tag);

        singleEndpoint.AllowAnonymous();
        // singleEndpoint.RequireAuthorization(AuthConstants.AdminUserPolicyName);
    }

    private static async Task<IResult> ScaffoldApiFromMetaInfoAction(
        HttpContext http,
        string id,
        IApiMetaInfoModelService service,
        //IOutputCacheStore outputCacheStore,
        //LinkGenerator linker,
        CancellationToken token = default)
    {
        string userNameId = http.Items[UsernameIdentifierMiddleware.Key]!.ToString()!;
        var isTrusted = (eModelOwnershipScope)http.Items[OwnershipTypeMiddleware.Key]! == eModelOwnershipScope.All;
        Log.Information("");
        
        var entry = (await service.GetByIdAsync(id, isTrusted, token))
            .ThrowIfFail();
        
        var result = (await service.ScaffoldApi(entry, userNameId, token))
            .ThrowIfFail();

        return Results.NoContent();
    }
}