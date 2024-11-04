using NewsManagement2.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace NewsManagement2;

[DependsOn(
    typeof(NewsManagement2EntityFrameworkCoreTestModule)
    )]
public class NewsManagement2DomainTestModule : AbpModule
{

}
