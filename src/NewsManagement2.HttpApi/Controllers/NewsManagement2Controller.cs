using NewsManagement2.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace NewsManagement2.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class NewsManagement2Controller : AbpControllerBase
{
    protected NewsManagement2Controller()
    {
        LocalizationResource = typeof(NewsManagement2Resource);
    }
}
