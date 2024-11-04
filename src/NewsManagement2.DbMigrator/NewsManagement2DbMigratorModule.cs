using NewsManagement2.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace NewsManagement2.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(NewsManagement2EntityFrameworkCoreModule),
    typeof(NewsManagement2ApplicationContractsModule)
    )]
public class NewsManagement2DbMigratorModule : AbpModule
{

}
