using Asp.Versioning;
using LanguageExt.Common;
using Microsoft.AspNetCore.OutputCaching;
using System.Security.Claims;

using Gay.Silverbranch.Api.Utilities.Backend.Logging.Endpoints.V1;
using Gay.Silverbranch.Api.Utilities.Backend.Middleware;
using Gay.Silverbranch.Api.Utilities.Common.Endpoints.Interfaces;
using Gay.Silverbranch.Api.Utilities.Common.Versioning;
using Gay.Silverbranch.Utilities.Security.Constants;

using Gay.Silverbranch.ProjectScaffolder.Api.Mappings.V1;
using Gay.Silverbranch.ProjectScaffolder.Bll.Services.Interface.Model.V1;
using Gay.Silverbranch.ProjectScaffolder.Contracts.Endpoints.V1;
using Gay.Silverbranch.ProjectScaffolder.Models.Entities.V1;

namespace Gay.Silverbranch.ProjectScaffolder.Api.Endpoints.V1.Delete;

/// <summary>
/// The collection of Endpoints for the ModelMetaInfo Model in API pertaining to the DELETE action
/// </summary>
[ApiVersion(1.0)]
public class DeleteModelMetaInfoModelEndpoint : IEndpoints
{
    private const string ContentType = "application/json";
    private const string ModelTypeName = "ModelMetaInfo";
    
    /// <summary>
    /// Register specific services need for the ModelMetaInfo Model DELETE endpoint to the DI container
    /// </summary>
    /// <param name="services">The service collection for dependency injection</param>
    /// <param name="configuration">The currently loaded configurations, secrets and environment variables</param>
    public static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        DeleteEndpointLoggingTemplates.LogAddServices(modelName: ModelTypeName);
    }

    /// <summary>
    /// Map all ModelMetaInfo Model DELETE Endpoint with correct settings
    /// </summary>
    /// <param name="app"></param>
    public static void DefineEndpoints(IEndpointRouteBuilder app)
    {
        DeleteEndpointLoggingTemplates.LogDefined(modelName: ModelTypeName);
        var singleEndpoint = app.MapDelete(
                ModelMetaInfoModelEndpoints.DeleteEndpoint,
                DeleteModelMetaInfoModelAsync)
            .WithName(ModelMetaInfoModelEndpoints.DeleteEndpointName)
            .Accepts<string>(ContentType)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithApiVersionSet(ApiVersioning.VersionSet!)
            .HasApiVersion(1.0)
            .WithTags(ModelMetaInfoModelEndpoints.Tag);
            
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            singleEndpoint.AllowAnonymous();
        }
        else
        {
            singleEndpoint.RequireAuthorization(AuthConstants.AdminUserPolicyName);
        }
    }

    /// <summary>
    /// Delete an ModelMetaInfo Model by its ID along with all of its history
    /// </summary>
    /// <param name="http">The http client context</param>
    /// <param name="id">The ID of the ModelMetaInfo Model to delete form the database</param>
    /// <param name="service">The service class the serves this Endpoint for database operations</param>
    /// <param name="outputCacheStore">Access to the Output Cache</param>
    /// <param name="token">Cancellation token</param>
    /// <returns>Returns no content on a successful delete</returns>
    /// <response code="200">Delete worked, returns the last surviving copy</response>
    /// <response code="404">ID was not found in the database</response>
    /// <response code="500">Something went wrong or the database does not exist</response>
    private static async Task<IResult> DeleteModelMetaInfoModelAsync(
        HttpContext http,
        string id,
        IModelMetaInfoModelService service,
        IOutputCacheStore outputCacheStore,
        CancellationToken token)
    {
        string userNameId = http.Items[UsernameIdentifierMiddleware.Key]!.ToString()!;
        DeleteEndpointLoggingTemplates.LogCalled(
            modelName: ModelTypeName,
            callingUserId: userNameId);
        
        var entry = await service.DeleteAsync(
            user: userNameId,
            id: id,
            token: token);
        if (!entry.IsFail)
            await outputCacheStore.EvictByTagAsync(
                tag: ModelMetaInfoModelEndpoints.Tag,
                cancellationToken: token);
        return entry.Match(
            Succ: model => HandleDeleteModelEndpointSuccess(model, userNameId),
            Fail: error => HandleDeleteModelEndpointFail(error, id, userNameId));
    }

    private static IResult HandleDeleteModelEndpointSuccess(
        ModelMetaInfoModel model,
        string userId)
    {
        DeleteEndpointLoggingTemplates.LogEndpointSuccess(
            modelName:model.GetType().Name,
            modelId:model.CommonIdentity,
            callingUserId:userId);
        return Results.Ok(model.MapToResponseFromModel());
    }

    private static IResult HandleDeleteModelEndpointFail(
        in Error error,
        string modelId,
        string userId)
    {
        var ex = error.ToException();

        if (ex.GetType() == typeof(NullReferenceException))
        {
            DeleteEndpointLoggingTemplates.LogEndpointFailureNullRef(
                exception:ex,
                modelName:nameof(ModelMetaInfoModel),
                modelId:modelId,
                callingUserId:userId);
            return Results.NotFound();
        }
        DeleteEndpointLoggingTemplates.LogEndpointFailureServerError(
            exception:ex,
            modelName:nameof(ModelMetaInfoModel),
            modelId:modelId,
            callingUserId:userId);
        return Results.Problem(
            detail: ex.ToString(),
            statusCode: StatusCodes.Status500InternalServerError);
    }
}