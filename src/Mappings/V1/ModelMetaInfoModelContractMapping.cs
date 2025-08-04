using Gay.Silverbranch.Api.Models.Enum.V1;

using Gay.Silverbranch.ProjectScaffolder.Contracts.Requests.V1.Post;
using Gay.Silverbranch.ProjectScaffolder.Contracts.Requests.V1.Put;
using Gay.Silverbranch.ProjectScaffolder.Contracts.Responses.V1;
using Gay.Silverbranch.ProjectScaffolder.Models.Entities.V1;

namespace Gay.Silverbranch.ProjectScaffolder.Api.Mappings.V1;

/// <summary>
/// 
/// </summary>
public static class ModelMetaInfoModelContractMapping
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="id"></param>
    /// <param name="username"></param>
    /// <param name="PropertiesModels"></param>
    /// <returns></returns>
    public static ModelMetaInfoModel MapToModelFromPostRequest(
        this PostModelMetaInfoModelRequest request,
        string id,
        string username,
		List<PropertyMetaInfoModel> PropertiesModels)
    {
        return new ModelMetaInfoModel(
            id,
            request.Name,
            username, 
            request.Notes)
        {
			NameAsCollection = request.NameAsCollection,
			Inherits = request.Inherits,
			EndpointTag = request.EndpointTag,
			Properties = PropertiesModels
        };
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="id"></param>
    /// <param name="username"></param>
	/// <param name="propertiesModels"></param>
    /// <returns></returns>
    public static ModelMetaInfoModel MapToModelFromPutRequest(
        this PutModelMetaInfoModelRequest request,
        string id,
        string username,
		List<PropertyMetaInfoModel> propertiesModels)
    {
        return new ModelMetaInfoModel(id,
            request.Name,
            username,
            request.Notes)
        {
			NameAsCollection = request.NameAsCollection,
			Inherits = request.Inherits,
			EndpointTag = request.EndpointTag,
			Properties = propertiesModels
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public static ModelMetaInfoModelResponse MapToResponseFromModel(
        this ModelMetaInfoModel model)
    {
        return new ModelMetaInfoModelResponse()
        {
            Id = model.CommonIdentity,
            Name = model.Name,
            
			NameAsCollection = model.NameAsCollection,
			Inherits = model.Inherits,
			EndpointTag = model.EndpointTag,
			PropertyMetaInfoIds = model.Properties.Select(m => m.CommonIdentity).ToList()
        };
    }
}