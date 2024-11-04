using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NewsManagement2.Data;
using Volo.Abp.DependencyInjection;

namespace NewsManagement2.EntityFrameworkCore;

public class EntityFrameworkCoreNewsManagement2DbSchemaMigrator
    : INewsManagement2DbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreNewsManagement2DbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the NewsManagement2DbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<NewsManagement2DbContext>()
            .Database
            .MigrateAsync();
    }
}
