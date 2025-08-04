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
public class PutModelMetaInfoModelRequestValidationFilter : 
    PutRequestValidationFilter<PutModelMetaInfoModelRequest>
{
	/// <summary>
	/// SUMMARY NEEDED HERE
	/// </summary>
	public const string PropertiesKey = "PutPropertiesModelsRequest";

	private readonly IPropertyMetaInfoModelService _propertiesService;

	/// <summary>
	/// 
	/// </summary>
	/// <param name="requestValidator"></param>
	/// <param name="propertiesService"></param>
	public PutModelMetaInfoModelRequestValidationFilter(
        IValidator<PutModelMetaInfoModelRequest> requestValidator,
		IPropertyMetaInfoModelService propertiesService) : 
        base(requestValidator)
    {
		_propertiesService = propertiesService;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<ValidationResult?> PreEndpointFilterActionAsync(
        EndpointFilterInvocationContext context,
        PutModelMetaInfoModelRequest request)
    {
        
		var propertiesGetAllOptions = new GetAllModelsOptions()
		{
			PageIndex = 0,
			PageSize = request.PropertyMetaInfoIds.Count,
			SpecificIds = request.PropertyMetaInfoIds.ToArray(),
		};
		var propertiesResult = await _propertiesService.GetByIdRangeAsync(propertiesGetAllOptions);
		var propertiesModels = propertiesResult.IfFail(
			fail =>
			{
				Log.Error(fail.ToException(),
					"Server issue encountered while trying to query for the list of requested PropertyMetaInfo Models ");
				return [];
			});
		if (!propertiesModels.Any()) ((Error)propertiesResult).Throw();

		context.HttpContext.Items.Add(PropertiesKey, propertiesModels);
        
        return await _requestValidator.ValidateAsync(request);
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