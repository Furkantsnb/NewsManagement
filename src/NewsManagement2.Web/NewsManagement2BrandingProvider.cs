using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;

namespace NewsManagement2.Web;

[Dependency(ReplaceServices = true)]
public class NewsManagement2BrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "NewsManagement2";
}
