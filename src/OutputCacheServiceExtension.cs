using Gay.Silverbranch.ProjectScaffolder.Contracts.Endpoints.V1;
using Gay.Silverbranch.ProjectScaffolder.Contracts.Requests.V1.GetAll;
using Microsoft.AspNetCore.OutputCaching;

namespace Gay.Silverbranch.ProjectScaffolder.Api;

#pragma warning disable CS1591

public static class OutputCacheServiceExtension
{
    public static int OutputCacheExpirationInMinutes = 1;

    public static IServiceCollection AddOutputAndResponseCacheing(this IServiceCollection services)
    {
        //services.AddResponseCaching();
        services.AddOutputCache(ConfigureOptions);
        return services;
    }

    private static void ConfigureOptions(OutputCacheOptions opts)
    {
        opts.AddBasePolicy(c => c.Cache());
        
        opts.AddPolicy(PropertyMetaInfoModelEndpoints.Tag, c =>
        {
            c.Cache()
                .Expire(TimeSpan.FromMinutes(OutputCacheExpirationInMinutes))
                .SetVaryByQuery(typeof(GetAllPropertyMetaInfoModelsRequest).GetProperties()
                    .Select(p => p.Name)
                    .ToArray())
                .Tag(PropertyMetaInfoModelEndpoints.Tag);
        });
        
        opts.AddPolicy(ModelMetaInfoModelEndpoints.Tag, c =>
        {
            c.Cache()
                .Expire(TimeSpan.FromMinutes(OutputCacheExpirationInMinutes))
                .SetVaryByQuery(typeof(GetAllModelMetaInfoModelsRequest).GetProperties()
                    .Select(p => p.Name)
                    .ToArray())
                .Tag(ModelMetaInfoModelEndpoints.Tag);
        });
        
        opts.AddPolicy(ApiMetaInfoModelEndpoints.Tag, c =>
        {
            c.Cache()
                .Expire(TimeSpan.FromMinutes(OutputCacheExpirationInMinutes))
                .SetVaryByQuery(typeof(GetAllApiMetaInfoModelsRequest).GetProperties()
                    .Select(p => p.Name)
                    .ToArray())
                .Tag(ApiMetaInfoModelEndpoints.Tag);
        });
    }
}

#pragma warning restore CS1591