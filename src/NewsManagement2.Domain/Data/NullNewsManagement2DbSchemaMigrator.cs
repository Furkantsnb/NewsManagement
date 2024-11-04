using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace NewsManagement2.Data;

/* This is used if database provider does't define
 * INewsManagement2DbSchemaMigrator implementation.
 */
public class NullNewsManagement2DbSchemaMigrator : INewsManagement2DbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
