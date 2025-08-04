using Gay.Silverbranch.Api.Models.Enum.V1;

using Gay.Silverbranch.ProjectScaffolder.Contracts.Requests.V1.Post;
using Gay.Silverbranch.ProjectScaffolder.Contracts.Requests.V1.Put;
using Gay.Silverbranch.ProjectScaffolder.Contracts.Responses.V1;
using Gay.Silverbranch.ProjectScaffolder.Models.Entities.V1;

namespace Gay.Silverbranch.ProjectScaffolder.Api.Mappings.V1;

/// <summary>
/// 
/// </summary>
public static class ApiMetaInfoModelContractMapping
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="id"></param>
    /// <param name="username"></param>
    /// <param name="modelsModels"></param>
    /// <returns></returns>
    public static ApiMetaInfoModel MapToModelFromPostRequest(
        this PostApiMetaInfoModelRequest request,
        string id,
        string username,
		List<ModelMetaInfoModel> modelsModels)
    {
        return new ApiMetaInfoModel(
            id,
            request.Name,
            username, 
            request.Notes)
        {
			BaseNamespace = request.BaseNamespace,
			Version = request.Version,
			Models = modelsModels,
			Style = Enum.Parse<eStyle>(request.Style, ignoreCase: true),
            NeedsSecurityPackage = request.NeedsSecurityPackage,
            IconWidth = request.IconWidth,
            IconHeight = request.IconHeight,
            IconBackgroundHex = request.IconBackgroundHex,
            IconApiFont = request.IconApiFont,
            IconProjectFont = request.IconProjectFont,
            Layers = request.Layers,
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="id"></param>
    /// <param name="username"></param>
    /// <param name="modelsModels"></param>
    /// <returns></returns>
    public static ApiMetaInfoModel MapToModelFromPutRequest(
        this PutApiMetaInfoModelRequest request,
        string id,
        string username,
		List<ModelMetaInfoModel> modelsModels)
    {
        return new ApiMetaInfoModel(id,
            request.Name,
            username,
            request.Notes)
        {
			BaseNamespace = request.BaseNamespace,
			Version = request.Version,
			Models = modelsModels,
			Style = Enum.Parse<eStyle>(request.Style, ignoreCase: true),
            NeedsSecurityPackage = request.NeedsSecurityPackage,
            IconWidth = request.IconWidth,
            IconHeight = request.IconHeight,
            IconBackgroundHex = request.IconBackgroundHex,
            IconApiFont = request.IconApiFont,
            IconProjectFont = request.IconProjectFont,
            Layers = request.Layers,
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public static ApiMetaInfoModelResponse MapToResponseFromModel(
        this ApiMetaInfoModel model)
    {
        return new ApiMetaInfoModelResponse()
        {
            Id = model.CommonIdentity,
            Name = model.Name,
            
			BaseNamespace = model.BaseNamespace,
			Version = model.Version,
			ModelMetaInfoIds = model.Models.Select(m => m.CommonIdentity).ToList(),
			Style = model.Style.ToString(),
            NeedsSecurityPackage = model.NeedsSecurityPackage,
            IconWidth = model.IconWidth,
            IconHeight = model.IconHeight,
            IconBackgroundHex = model.IconBackgroundHex,
            IconApiFont = model.IconApiFont,
            IconProjectFont = model.IconProjectFont,
            Layers = model.Layers,
        };
    }
}