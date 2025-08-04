using Asp.Versioning;
using FluentValidation.Results;
using LanguageExt.Common;
using Microsoft.AspNetCore.OutputCaching;
using System.Security.Claims;

using Gay.Silverbranch.Api.Bll.Services.Interface.V1;
using Gay.Silverbranch.Api.Models.Enum.V1;
using Gay.Silverbranch.Api.Utilities.Backend.Logging.Endpoints.V1;
using Gay.Silverbranch.Api.Utilities.Backend.Middleware;
using Gay.Silverbranch.Api.Utilities.Common.Endpoints.Interfaces;
using Gay.Silverbranch.Api.Utilities.Common.Versioning;
using Gay.Silverbranch.Utilities.Security.Constants;

using Gay.Silverbranch.ProjectScaffolder.Api.Filters.V1.Put;
using Gay.Silverbranch.ProjectScaffolder.Api.Mappings.V1;
using Gay.Silverbranch.ProjectScaffolder.Bll.Services.Interface.Model.V1;
using Gay.Silverbranch.ProjectScaffolder.Contracts.Endpoints.V1;
using Gay.Silverbranch.ProjectScaffolder.Contracts.Requests.V1.Put;
using Gay.Silverbranch.ProjectScaffolder.Models.Entities.V1;

namespace Gay.Silverbranch.ProjectScaffolder.Api.Endpoints.V1.Put;

/// <summary>
/// The collection of Endpoints for the ApiMetaInfo Model in API pertaining to the PUT action
/// </summary>
[ApiVersion(1.0)]
public class PutApiMetaInfoModelEndpoint : IEndpoints
{
    private const string ContentType = "application/json";
    private const string ModelTypeName = "ApiMetaInfo";

    /// <summary>
    /// Register specific services need for the ApiMetaInfo Model PUT endpoint to the DI container
    /// </summary>
    /// <param name="services">The service collection for dependency injection</param>
    /// <param name="configuration">The currently loaded configurations, secrets and environment variables</param>
    public static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        PutEndpointLoggingTemplates.LogAddServices(modelName: ModelTypeName);
    }

    /// <summary>
    /// Map all ApiMetaInfo Model Endpoints with correct settings
    /// </summary>
    /// <param name="app"></param>
    public static void DefineEndpoints(IEndpointRouteBuilder app)
    {
        PutEndpointLoggingTemplates.LogDefined(modelName: ModelTypeName);
        var singleEndpoint = app
            .MapPut(
                ApiMetaInfoModelEndpoints.PutEndpoint,
                PutApiMetaInfoModelAsync)
            .WithName(ApiMetaInfoModelEndpoints.PutEndpointName)
            .Accepts<PutApiMetaInfoModelRequest>(ContentType)
            .Produces(StatusCodes.Status200OK)
            .Produces<IEnumerable<ValidationFailure>>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithApiVersionSet(ApiVersioning.VersionSet!)
            .HasApiVersion(1.0)
            .WithTags(ApiMetaInfoModelEndpoints.Tag)
            .AddEndpointFilter<PutApiMetaInfoModelRequestValidationFilter>();
        
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            singleEndpoint.AllowAnonymous();
        }
        else
        {
            singleEndpoint.RequireAuthorization(AuthConstants.TrustedMemberPolicyName);
        }
    }

    /// <summary>
    /// Search for and update a specific ApiMetaInfo Model result from the database
    /// </summary>
    /// <param name="http">the http context</param>
    /// <param name="id">Id of model to update</param>
    /// <param name="changes">The collection of changes to be applied to the model</param>
    /// <param name="service">The service class the serves this Endpoint for database operations</param>
    /// <param name="outputCacheStore">Access to the Output Cache</param>
    /// <param name="linker">The web linker</param>
    /// <param name="token">Cancellation token</param>
    /// <returns>The updated copy of the ApiMetaInfo Model</returns>
    /// <response code="200">Update successful</response>
    /// <response code="400">Something went wrong or the database does not exist</response>
    /// <response code="404">ID was not found in the database</response>
    /// <response code="500">Something went wrong or the database does not exist</response>
    private static async Task<IResult> PutApiMetaInfoModelAsync(
        HttpContext http,
        string id,
        PutApiMetaInfoModelRequest changes,
        IApiMetaInfoModelService service,
        IOutputCacheStore outputCacheStore,
        LinkGenerator linker,
        CancellationToken token)
    {
        var userNameId = http.Items[UsernameIdentifierMiddleware.Key]!.ToString()!;
        PutEndpointLoggingTemplates.LogCalled(
            modelName: ModelTypeName,
            callingUserId: userNameId);
        
        var ownership = (eModelOwnershipScope)http.Items[OwnershipTypeMiddleware.Key]!;
		var queryResultModels =
			(http.Items[PutApiMetaInfoModelRequestValidationFilter.ModelsKey] as IEnumerable<ModelMetaInfoModel>)!
			.ToList();
        
        var model = changes.MapToModelFromPutRequest(
            id: id, 
            username: userNameId,
			queryResultModels);

        var result = await service.UpdateAsync(
            user: userNameId,
            model: model,
            isTrusted: ownership == eModelOwnershipScope.All,
            token: token);
        if (!result.IsFail)
            await outputCacheStore.EvictByTagAsync(
                tag: ApiMetaInfoModelEndpoints.Tag,
                cancellationToken: token);
        
        return result.Match(
            Succ: count => HandlePostModelEndpointSuccess(
                context: http,
                linker: linker,
                model: model,
                count: count,
                userId: userNameId),
            Fail: error => HandlePutModelEndpointFail(error, id, userNameId)
        );
    }

    private static IResult HandlePostModelEndpointSuccess(
        HttpContext context,
        LinkGenerator linker,
        ApiMetaInfoModel model,
        int count,
        string userId)
    {
        PutEndpointLoggingTemplates.LogEndpointSuccess(
            modelName: model.GetType().Name,
            modelId: model.CommonIdentity,
            callingUserId: userId);
        var locationUri = linker.GetUriByName(
            httpContext: context,
            endpointName: ApiMetaInfoModelEndpoints.GetByIdEndpointName,
            values: new { id = model.CommonIdentity });
        return Results.Created(locationUri, count);
    }

    private static IResult HandlePutModelEndpointFail(
        in Error error,
        string modelId,
        string userId)
    {
        var ex = error.ToException();

        if (ex.GetType() == typeof(NullReferenceException))
        {
            PutEndpointLoggingTemplates.LogEndpointFailureNullRef(
                exception:ex,
                modelName:nameof(ApiMetaInfoModel),
                modelId:modelId,
                callingUserId:userId);
            return Results.NotFound();
        }
        PutEndpointLoggingTemplates.LogEndpointFailureServerError(
            exception:ex,
            modelName:nameof(ApiMetaInfoModel),
            modelId:modelId,
            callingUserId:userId);
        return Results.Problem(
            detail: ex.ToString(),
            statusCode: StatusCodes.Status500InternalServerError);
    }
}