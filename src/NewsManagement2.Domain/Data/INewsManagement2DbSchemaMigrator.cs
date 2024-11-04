using System.Threading.Tasks;

namespace NewsManagement2.Data;

public interface INewsManagement2DbSchemaMigrator
{
    Task MigrateAsync();
}
