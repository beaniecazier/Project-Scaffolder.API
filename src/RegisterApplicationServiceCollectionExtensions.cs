using FluentValidation;
using Serilog;

using Gay.Silverbranch.Api.Utilities.Contract.Interface;
using Gay.Silverbranch.ProjectScaffolder.Bll;
using Gay.Silverbranch.ProjectScaffolder.Bll.Repository.Interface.V1;
using Gay.Silverbranch.ProjectScaffolder.Bll.Repository.V1;
using Gay.Silverbranch.ProjectScaffolder.Bll.Services.Interface.Model.V1;
using Gay.Silverbranch.ProjectScaffolder.Bll.Services.Model.V1;
using Gay.Silverbranch.ProjectScaffolder.Contracts;

namespace Gay.Silverbranch.ProjectScaffolder.Api;

/// <summary>
/// 
/// </summary>
public static class RegisterApplicationServiceCollectionExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        Log.Information("Adding PropertyMetaInfo Model Service to the DI Container");
        services.AddSingleton<IPropertyMetaInfoModelRepository, PropertyMetaInfoModelSqlRepository>();
        services.AddSingleton<IPropertyMetaInfoModelService, PropertyMetaInfoModelService>();
        
        Log.Information("Adding ModelMetaInfo Model Service to the DI Container");
        services.AddSingleton<IModelMetaInfoModelRepository, ModelMetaInfoModelSqlRepository>();
        services.AddSingleton<IModelMetaInfoModelService, ModelMetaInfoModelService>();
        
        Log.Information("Adding ApiMetaInfo Model Service to the DI Container");
        services.AddScaffolderServices();
        services.AddSingleton<IApiMetaInfoModelRepository, ApiMetaInfoModelSqlRepository>();
        services.AddSingleton<IApiMetaInfoModelService, ApiMetaInfoModelService>();

        services.AddValidatorsFromAssemblyContaining<IApplicationMarker>(ServiceLifetime.Singleton);
        services.AddValidatorsFromAssemblyContaining<IApplicationContractsBaseMarker>(ServiceLifetime.Singleton);
        return services;
    }
}