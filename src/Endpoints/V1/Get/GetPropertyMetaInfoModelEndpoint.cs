using Asp.Versioning;
using LanguageExt.Common;
using System.Security.Claims;

using Gay.Silverbranch.Api.Bll.Options.V1;
using Gay.Silverbranch.Api.Bll.Services.Interface.V1;
using Gay.Silverbranch.Api.Models.Enum;
using Gay.Silverbranch.Api.Models.Enum.V1;
using Gay.Silverbranch.Api.Utilities.Backend.Filters;
using Gay.Silverbranch.Api.Utilities.Backend.Logging.Endpoints.V1;
using Gay.Silverbranch.Api.Utilities.Backend.Mapping.V1;
using Gay.Silverbranch.Api.Utilities.Backend.Middleware;
using Gay.Silverbranch.Api.Utilities.Common.Endpoints.Interfaces;
using Gay.Silverbranch.Api.Utilities.Common.Versioning;
using Gay.Silverbranch.Api.Utilities.Contract.Requests;
using Gay.Silverbranch.Api.Utilities.Contract.Requests.V1;
using Gay.Silverbranch.Utilities.Security.Constants;

using Gay.Silverbranch.ProjectScaffolder.Api.Mappings.V1;
using Gay.Silverbranch.ProjectScaffolder.Bll.Services.Interface.Model.V1;
using Gay.Silverbranch.ProjectScaffolder.Contracts.Endpoints.V1;
using Gay.Silverbranch.ProjectScaffolder.Contracts.Responses.V1;
using Gay.Silverbranch.ProjectScaffolder.Models.Entities.V1;

namespace Gay.Silverbranch.ProjectScaffolder.Api.Endpoints.V1.Get;

/// <summary>
/// The collection of Endpoints for the PropertyMetaInfo Model in API
/// </summary>
[ApiVersion(1.0)]
public class GetPropertyMetaInfoModelEndpoint : IEndpoints
{
    private const string ContentType = "application/json";
    private const string ModelTypeName = "PropertyMetaInfo";
    
    /// <summary>
    /// Register specific services need for the PropertyMetaInfo Model GET endpoints to the DI container
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        GetByIdEndpointLoggingTemplates.LogAddServices(modelName: ModelTypeName);
        GetAllEndpointLoggingTemplates.LogAddServices(modelName: ModelTypeName);
    }

    /// <summary>
    /// Map all PropertyMetaInfo Model Endpoints with correct settings
    /// </summary>
    /// <param name="app"></param>
    public static void DefineEndpoints(IEndpointRouteBuilder app)
    {
        GetByIdEndpointLoggingTemplates.LogDefined(modelName: ModelTypeName);
        GetAllEndpointLoggingTemplates.LogDefined(modelName: ModelTypeName);
        
        var singleEndpoint = app.MapGet(
                PropertyMetaInfoModelEndpoints.GetByIdEndpoint,
                GetPropertyMetaInfoModelByIdAsync)
            .WithName("GetPropertyMetaInfoModelByID")
            .Accepts<string>(ContentType)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithApiVersionSet(ApiVersioning.VersionSet!)
            .HasApiVersion(1.0)
            .CacheOutput(PropertyMetaInfoModelEndpoints.Tag)
            .WithTags(PropertyMetaInfoModelEndpoints.Tag);

        var multipleEndpoint = app.MapGet(
                PropertyMetaInfoModelEndpoints.GetAllEndpoint,
                GetAllPropertyMetaInfoModelsAsync)
            .WithName("GetAllPropertyMetaInfoModels")
            .Accepts<GetAllModelsRequest>(ContentType)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithApiVersionSet(ApiVersioning.VersionSet!)
            .HasApiVersion(1.0)
            .CacheOutput(PropertyMetaInfoModelEndpoints.Tag)
            .WithTags(PropertyMetaInfoModelEndpoints.Tag)
            .AddEndpointFilter<GetAllRequestValidationFilter>();

        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            singleEndpoint.AllowAnonymous();
            multipleEndpoint.AllowAnonymous();
        }
        else
        {
            singleEndpoint.RequireAuthorization(AuthConstants.TrustedMemberPolicyName);
            multipleEndpoint.RequireAuthorization(AuthConstants.TrustedMemberPolicyName);
        }
    }
    
    /// <summary>
    /// Query the database by id for the most up-to-date copy of a newModel
    /// </summary>
    /// <param name="http">The http client context</param>
    /// <param name="id">The model id used to query the database</param>
    /// <param name="service">The service class the serves this Endpoint for database operations</param>
    /// <param name="token">Cancellation token</param>
    /// <returns>The searched newModel</returns>
    /// <response code="200">Get successful</response>
    /// <response code="404">ID was not found in the database</response>
    /// <response code="500">Something went wrong or the database does not exist</response>
    private static async Task<IResult> GetPropertyMetaInfoModelByIdAsync(
        HttpContext http,
        string id,
        IPropertyMetaInfoModelService service,
        CancellationToken token)
    {
        string userNameId = http.Items[UsernameIdentifierMiddleware.Key]!.ToString()!;
        GetAllEndpointLoggingTemplates.LogCalled(
            modelName: ModelTypeName,
            callingUserId: userNameId);
        
        var ownership = (eModelOwnershipScope)http.Items[OwnershipTypeMiddleware.Key]!;
        
        var entry = await service.GetByIdAsync(
            id: id,
            isTrusted: ownership == eModelOwnershipScope.All,
            token: token);
        return entry.Match(
            Succ: model => HandleGetByIdModelEndpointSuccess(
                model: model,
                userId: userNameId),
            Fail: error => HandleGetByIdModelEndpointFail(
                error: error,
                modelId: id,
                userId: userNameId));
    }

    private static IResult HandleGetByIdModelEndpointSuccess(
        PropertyMetaInfoModel model,
        string userId)
    {
        GetByIdEndpointLoggingTemplates.LogEndpointSuccess(
            modelName:model.GetType().Name,
            modelId:model.CommonIdentity,
            callingUserId:userId);
        return Results.Ok(model.MapToResponseFromModel());
    }

    private static IResult HandleGetByIdModelEndpointFail(
        in Error error,
        string modelId,
        string userId)
    {
        var ex = error.ToException();
        if (ex.GetType() == typeof(NullReferenceException))
        {
            GetByIdEndpointLoggingTemplates.LogEndpointFailureNullRef(
                exception:ex,
                modelName:nameof(PropertyMetaInfoModel),
                modelId:modelId,
                callingUserId:userId);
            return Results.NotFound();
        }
        GetByIdEndpointLoggingTemplates.LogEndpointFailureServerError(
            exception:ex,
            modelName:nameof(PropertyMetaInfoModel),
            modelId:modelId,
            callingUserId:userId);
        return Results.Problem(
            detail: ex.ToString(),
            statusCode: StatusCodes.Status500InternalServerError);
    }
    
    /// <summary>
    /// Retrieve all PropertyMetaInfo Models from the database
    /// </summary>
    /// <param name="http">The http client context</param>
    /// <param name="service">The service class the serves this Endpoint for database operations</param>
    /// <param name="request">The encapsulated GetAllModels Request Parameters</param>
    /// <param name="token">Cancellation token</param>
    /// <returns>A list of all PropertyMetaInfo Models in the database</returns>
    /// <response code="200">Get all successful</response>
    /// <response code="500">Something went wrong or the database does not exist</response>
    private static async Task<IResult> GetAllPropertyMetaInfoModelsAsync(
        HttpContext http,
        IPropertyMetaInfoModelService service,
        [AsParameters] GetAllModelsRequest request,
        CancellationToken token)
    {
        var userNameId = http.Items[UsernameIdentifierMiddleware.Key]!.ToString()!;
        GetAllEndpointLoggingTemplates.LogCalled(
            modelName: ModelTypeName,
            callingUserId: userNameId);
        
        var ownershipScope = (eModelOwnershipScope)http.Items[OwnershipTypeMiddleware.Key]!;
        var options = request.MapToOptions()
            .WithUser(userNameId, ownershipScope);
        
        var entries = await service.GetAllAsync(options, token);
        var total = await service.GetQueryTotal(options);

        return entries.Match(
            Succ: models => HandleGetAllModelsFromRequestEndpointSuccess(
                models: models,
                options: options,
                total: total,
                userId: userNameId),
            Fail: error => HandleGetAllModelsFromRequestEndpointFail(
                error: error,
                options: options,
                userId: userNameId));
    }
    
    private static IResult HandleGetAllModelsFromRequestEndpointSuccess(
        IEnumerable<PropertyMetaInfoModel> models,
        GetAllModelsOptions options,
        int total,
        string userId)
    {
        var responsesCollection = new PropertyMetaInfoModelsResponse()
        {
            Items = models.Select(x => x.MapToResponseFromModel()),
            PageIndex = options.PageIndex,
            PageSize = options.PageSize,
            TotalNumberOfAvailableResponses = total,
        };
        GetAllEndpointLoggingTemplates.LogEndpointSuccess(
            modelName:nameof(PropertyMetaInfoModel),
            options:options,
            totalNumberOfResponses:total,
            callingUserId:userId);
        return Results.Ok(responsesCollection);
    }

    private static IResult HandleGetAllModelsFromRequestEndpointFail(
        in Error error,
        GetAllModelsOptions options,
        string userId)
    {
        var ex = error.ToException();
        GetAllEndpointLoggingTemplates.LogEndpointFailureServerError(
            exception:ex,
            modelName:nameof(PropertyMetaInfoModel),
            options:options,
            callingUserId:userId);
        return Results.Problem(ex.ToString(),
            statusCode: StatusCodes.Status500InternalServerError);
    }
}
