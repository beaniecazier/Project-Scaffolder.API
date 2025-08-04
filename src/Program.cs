using Asp.Versioning;
using Gay.Silverbranch.Api.Utilities.Backend.Extensions.Middleware;
using Gay.Silverbranch.Api.Utilities.Common.Endpoints.Extensions;
using Gay.Silverbranch.Api.Utilities.Common.Versioning;
using Gay.Silverbranch.ProjectScaffolder.Bll.Database.V1;
using Serilog;

namespace Gay.Silverbranch.ProjectScaffolder.Api;

/// <summary>
/// 
/// </summary>
public class Program
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="args"></param>
    public static async Task Main(string[] args)
    {
        var defaultApiVersion = new ApiVersion(1, 0);
        List<ApiVersion> versions = new List<ApiVersion>()
        {
            defaultApiVersion,
        };
        
        var builder = WebApplication.CreateBuilder(args);

        var app = builder.AddApplicationServicesAndConfigurations(defaultApiVersion, args);

        // === DIRTY HACK, we WILL come back to fix this ===
        var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ProjectScaffolderDbContext>();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        // =================================================

        app!.CreateApiVersionSet(versions);

        app.UseSwagger(options =>
        {
            //options.RouteTemplate = $"{StaticRoutingDefinitions.BaseRoute}/swagger/{{documentname}}/swagger.json";
            options.RouteTemplate = "swagger/{documentname}/swagger.json";
        });

        app.UseSwaggerUI(c =>
        {
            //var prefix = $"{StaticRoutingDefinitions.BaseRoute}/swagger";
            foreach (var description in app!.DescribeApiVersions())
            {
                c.SwaggerEndpoint(
                    $"/swagger/{description.GroupName}/swagger.json",
                    description.GroupName);
            }
            //c.RoutePrefix = prefix;
            c.RoutePrefix = "swagger";
        });

        app.UseHttpsRedirection();

        //app!.UseAuthentication();
        //app!.UseAuthorization();

        app!.UseUsernameIdentifierMiddleware();
        app!.UseOwnershipTypeFlaging();
        app!.UseApplicationEndpoints<IApiMarker>();

        if (app!.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        
        Log.Information("Starting Api Host");

        app.Run();

        await Log.CloseAndFlushAsync();
    }
}