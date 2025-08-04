using FluentValidation;
using FluentValidation.Results;
using LanguageExt.Common;
using Serilog;

using Gay.Silverbranch.Api.Bll.Options.V1;
using Gay.Silverbranch.Api.Utilities.Backend.Filters;
using Gay.Silverbranch.ProjectScaffolder.Contracts.Requests.V1.Post;

namespace Gay.Silverbranch.ProjectScaffolder.Api.Filters.V1.Post;

/// <summary>
/// 
/// </summary>
public class PostPropertyMetaInfoModelRequestValidationFilter : 
    PostRequestValidationFilter<PostPropertyMetaInfoModelRequest>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="requestValidator"></param>
    public PostPropertyMetaInfoModelRequestValidationFilter(
        IValidator<PostPropertyMetaInfoModelRequest> requestValidator) : 
        base(requestValidator)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="response"></param>
    /// <returns></returns>
    protected override async ValueTask<object?> PostEndpointFilterActionAsync(
        EndpointFilterInvocationContext context,
        object? response) => response;
}