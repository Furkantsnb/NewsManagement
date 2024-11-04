using Volo.Abp.Modularity;

namespace NewsManagement2;

[DependsOn(
    typeof(NewsManagement2ApplicationModule),
    typeof(NewsManagement2DomainTestModule)
    )]
public class NewsManagement2ApplicationTestModule : AbpModule
{

}
