using FluentValidation;
using FluentValidation.Results;
using LanguageExt.Common;
using Serilog;

using Gay.Silverbranch.Api.Bll.Options.V1;
using Gay.Silverbranch.Api.Utilities.Backend.Filters;

using Gay.Silverbranch.ProjectScaffolder.Bll.Services.Interface.Model.V1;
using Gay.Silverbranch.ProjectScaffolder.Contracts.Requests.V1.Put;

namespace Gay.Silverbranch.ProjectScaffolder.Api.Filters.V1.Put;

/// <summary>
/// 
/// </summary>
public class PutPropertyMetaInfoModelRequestValidationFilter : 
    PutRequestValidationFilter<PutPropertyMetaInfoModelRequest>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="requestValidator"></param>
    public PutPropertyMetaInfoModelRequestValidationFilter(
        IValidator<PutPropertyMetaInfoModelRequest> requestValidator) : 
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