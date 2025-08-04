using Asp.Versioning;
using FluentValidation.Results;
using LanguageExt.Common;
using Microsoft.AspNetCore.OutputCaching;

using Gay.Silverbranch.Api.Bll.Services.Interface.V1;
using Gay.Silverbranch.Api.Utilities.Backend.Logging.Endpoints.V1;
using Gay.Silverbranch.Api.Utilities.Backend.Middleware;
using Gay.Silverbranch.Api.Utilities.Common.Endpoints.Interfaces;
using Gay.Silverbranch.Api.Utilities.Common.Versioning;
using Gay.Silverbranch.Utilities.Security.Constants;

using Gay.Silverbranch.ProjectScaffolder.Api.Filters.V1.Post;
using Gay.Silverbranch.ProjectScaffolder.Api.Mappings.V1;
using Gay.Silverbranch.ProjectScaffolder.Bll.Services.Interface.Model.V1;
using Gay.Silverbranch.ProjectScaffolder.Contracts.Endpoints.V1;
using Gay.Silverbranch.ProjectScaffolder.Contracts.Requests.V1.Post;
using Gay.Silverbranch.ProjectScaffolder.Models.Entities.V1;

namespace Gay.Silverbranch.ProjectScaffolder.Api.Endpoints.V1.Post;

/// <summary>
/// The collection of Post Endpoints for the PropertyMetaInfo Model in API
/// </summary>
[ApiVersion(1.0)]
public class PostPropertyMetaInfoModelEndpoint : IEndpoints
{
    private const string ContentType = "application/json";
    private const string ModelTypeName = "PropertyMetaInfo";

    /// <summary>
    /// Register specific services need for the PropertyMetaInfo Model POST endpoint to the DI container
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        PostEndpointLoggingTemplates.LogAddServices(modelName: ModelTypeName);
    }

    /// <summary>
    /// Map all PropertyMetaInfo Model POST Endpoint with correct settings
    /// </summary>
    /// <param name="app"></param>
    public static void DefineEndpoints(IEndpointRouteBuilder app)
    {
        PostEndpointLoggingTemplates.LogDefined(modelName: ModelTypeName);
        var singleEndpoint = app.MapPost(
                PropertyMetaInfoModelEndpoints.PostEndpoint,
                PostPropertyMetaInfoModelAsync)
            .WithName(PropertyMetaInfoModelEndpoints.PostEndpointName)
            .Accepts<PropertyMetaInfoModel>(ContentType)
            .Produces<PropertyMetaInfoModel>(StatusCodes.Status201Created)
            .Produces<IEnumerable<ValidationFailure>>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithApiVersionSet(ApiVersioning.VersionSet!)
            .HasApiVersion(1.0)
            .WithTags(PropertyMetaInfoModelEndpoints.Tag)
            .AddEndpointFilter<PostPropertyMetaInfoModelRequestValidationFilter>();

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
    /// Create a new PropertyMetaInfo Model
    /// </summary>
    /// <param name="http">the http context</param>
    /// <param name="request">The parameters used to make the new PropertyMetaInfo Model</param>
    /// <param name="service">The service class the serves this Endpoint for database operations</param>
    /// <param name="outputCacheStore">Access to the Output Cache</param>
    /// <param name="linker">The web linker</param>
    /// <param name="token">Cancellation token</param>
    /// <returns>The newly created model</returns>
    /// <response code="201">Model was successfully created and added to the database</response>
    /// <response code="400">Invalid information was provided and the request failed validation</response>
    /// <response code="500">Something went wrong or the database does not exist</response>
    private static async Task<IResult> PostPropertyMetaInfoModelAsync(
        HttpContext http,
        PostPropertyMetaInfoModelRequest request,
        IPropertyMetaInfoModelService service,
        IOutputCacheStore outputCacheStore,
        LinkGenerator linker,
        CancellationToken token)
    {
        var userNameId = http.Items[UsernameIdentifierMiddleware.Key]!.ToString()!;
        PostEndpointLoggingTemplates.LogCalled(
            modelName: ModelTypeName,
            callingUserId: userNameId);

        var id = await service.GetNewValidId();
        
        var model = request.MapToModelFromPostRequest(
            id: id,
            username: userNameId);
        
        var created = await service.CreateAsync(
            model: model,
            token: token);
        if(!created.IsFail)
            await outputCacheStore.EvictByTagAsync(
                tag: PropertyMetaInfoModelEndpoints.Tag,
                cancellationToken: token);

        return created.Match(
            Succ: count => HandlePostModelEndpointSuccess(
                context: http,
                linker: linker,
                model: model,
                count: count,
                userId: userNameId),
            Fail: error => HandlePostModelEndpointFail(error, userNameId)
        );
    }

    private static IResult HandlePostModelEndpointSuccess(
        HttpContext context,
        LinkGenerator linker,
        PropertyMetaInfoModel model,
        int count,
        string userId)
    {
        PostEndpointLoggingTemplates.LogEndpointSuccess(
            modelName: model.GetType().Name,
            modelId: model.CommonIdentity,
            callingUserId: userId);
        var locationUri = linker.GetUriByName(
            httpContext: context,
            endpointName: PropertyMetaInfoModelEndpoints.GetByIdEndpointName,
            values: new { id = model.CommonIdentity });
        return Results.Created(locationUri, count);
    }

    private static IResult HandlePostModelEndpointFail(
        Error error,
        string userId)
    {
        var ex = error.ToException();
        PostEndpointLoggingTemplates.LogEndpointFailureServerError(
            exception:ex,
            modelName:nameof(PropertyMetaInfoModel),
            callingUserId:userId);
        return Results.Problem(ex.ToString(),
            statusCode: StatusCodes.Status500InternalServerError);
    }
}