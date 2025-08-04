using Gay.Silverbranch.Api.Models.Enum.V1;

using Gay.Silverbranch.ProjectScaffolder.Contracts.Requests.V1.Post;
using Gay.Silverbranch.ProjectScaffolder.Contracts.Requests.V1.Put;
using Gay.Silverbranch.ProjectScaffolder.Contracts.Responses.V1;
using Gay.Silverbranch.ProjectScaffolder.Models.Entities.V1;

namespace Gay.Silverbranch.ProjectScaffolder.Api.Mappings.V1;

/// <summary>
/// 
/// </summary>
public static class PropertyMetaInfoModelContractMapping
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="id"></param>
    /// <param name="username"></param>
    /// <returns></returns>
    public static PropertyMetaInfoModel MapToModelFromPostRequest(
        this PostPropertyMetaInfoModelRequest request,
        string id,
        string username)
    {
        return new PropertyMetaInfoModel(
            id,
            request.Name,
            username, 
            request.Notes)
        {
			AccessLevel = Enum.Parse<eAccessLevel>(request.AccessLevel, ignoreCase: true),
			PropType = Enum.Parse<eType>(request.PropType, ignoreCase: true),
			PropTypeName = request.PropTypeName,
			PropCollectionType = Enum.Parse<eCollectionType>(request.PropCollectionType, ignoreCase: true),
			ValidationType = Enum.Parse<eValidationType>(request.ValidationType, ignoreCase: true),
			DefaultValue = request.DefaultValue,
			PropertyModifiers = request.PropertyModifiers
				.Select(e => Enum.Parse<ePropertyModifier>(e, ignoreCase: true))
				.ToList(),
			AlternateName = request.AlternateName,
			Description = request.Description,
			DbType = Enum.Parse<eDbType>(request.DbType, ignoreCase: true),
			Format = Enum.Parse<eFormat>(request.Format, ignoreCase: true),
			Encoding = string.IsNullOrWhiteSpace(request.Encoding) ?
				Enum.Parse<eEncoding>(request.Encoding!, ignoreCase: true) :
				eEncoding.None,
			FieldLength = request.FieldLength,
			ErrorMessage = request.ErrorMessage
        };
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="id"></param>
    /// <param name="username"></param>
    /// <returns></returns>
    public static PropertyMetaInfoModel MapToModelFromPutRequest(
        this PutPropertyMetaInfoModelRequest request,
        string id,
        string username)
    {
        return new PropertyMetaInfoModel(id,
            request.Name,
            username,
            request.Notes)
        {
			AccessLevel = Enum.Parse<eAccessLevel>(request.AccessLevel, ignoreCase: true),
			PropType = Enum.Parse<eType>(request.PropType, ignoreCase: true),
			PropTypeName = request.PropTypeName,
			PropCollectionType = Enum.Parse<eCollectionType>(request.PropCollectionType, ignoreCase: true),
			ValidationType = Enum.Parse<eValidationType>(request.ValidationType, ignoreCase: true),
			DefaultValue = request.DefaultValue,
			PropertyModifiers = request.PropertyModifiers
				.Select(e => Enum.Parse<ePropertyModifier>(e, ignoreCase: true))
				.ToList(),
			AlternateName = request.AlternateName,
			Description = request.Description,
			DbType = Enum.Parse<eDbType>(request.DbType, ignoreCase: true),
			Format = Enum.Parse<eFormat>(request.Format, ignoreCase: true),
			Encoding = string.IsNullOrWhiteSpace(request.Encoding) ?
				Enum.Parse<eEncoding>(request.Encoding!, ignoreCase: true) :
				eEncoding.None,
			FieldLength = request.FieldLength,
			ErrorMessage = request.ErrorMessage
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public static PropertyMetaInfoModelResponse MapToResponseFromModel(
        this PropertyMetaInfoModel model)
    {
        return new PropertyMetaInfoModelResponse()
        {
            Id = model.CommonIdentity,
            Name = model.Name,
            
			AccessLevel = model.AccessLevel.ToString(),
			PropType = model.PropType.ToString(),
			PropTypeName = model.PropTypeName,
			PropCollectionType = model.PropCollectionType.ToString(),
			ValidationType = model.ValidationType.ToString(),
			DefaultValue = model.DefaultValue,
			PropertyModifiers = model.PropertyModifiers
				.Select(e => e.ToString())
				.ToList(),
			AlternateName = model.AlternateName,
			Description = model.Description,
			DbType = model.DbType.ToString(),
			Format = model.Format.ToString(),
			Encoding = model.Encoding.ToString(),
			FieldLength = model.FieldLength,
			ErrorMessage = model.ErrorMessage
        };
    }
}