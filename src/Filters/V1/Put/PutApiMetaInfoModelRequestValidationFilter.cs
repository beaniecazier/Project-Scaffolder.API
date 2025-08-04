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
public class PutApiMetaInfoModelRequestValidationFilter : 
    PutRequestValidationFilter<PutApiMetaInfoModelRequest>
{
	/// <summary>
	/// SUMMARY NEEDED HERE
	/// </summary>
	public const string ModelsKey = "PutModelsModelsRequest";

	/// <summary>
	/// SUMMARY NEEDED HERE
	/// </summary>
	public const string IconKey = "PutIconModelsRequest";

	private readonly IModelMetaInfoModelService _modelsService;

	/// <summary>
	/// 
	/// </summary>
	/// <param name="requestValidator"></param>
	/// <param name="modelsService"></param>
	public PutApiMetaInfoModelRequestValidationFilter(
        IValidator<PutApiMetaInfoModelRequest> requestValidator,
		IModelMetaInfoModelService modelsService) : 
        base(requestValidator)
    {
		_modelsService = modelsService;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<ValidationResult?> PreEndpointFilterActionAsync(
        EndpointFilterInvocationContext context,
        PutApiMetaInfoModelRequest request)
    {
        
		var modelsGetAllOptions = new GetAllModelsOptions()
		{
			PageIndex = 0,
			PageSize = request.ModelMetaInfoIds.Count,
			SpecificIds = request.ModelMetaInfoIds.ToArray(),
		};
		var modelsResult = await _modelsService.GetByIdRangeAsync(modelsGetAllOptions);
		var modelsModels = modelsResult.IfFail(
			fail =>
			{
				Log.Error(fail.ToException(),
					"Server issue encountered while trying to query for the list of requested ModelMetaInfo Models ");
				return [];
			});
		if (!modelsModels.Any()) ((Error)modelsResult).Throw();

		context.HttpContext.Items.Add(ModelsKey, modelsModels);
        
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