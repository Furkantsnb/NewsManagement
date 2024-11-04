using NewsManagement2.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace NewsManagement2.Web.Pages;

/* Inherit your PageModel classes from this class.
 */
public abstract class NewsManagement2PageModel : AbpPageModel
{
    protected NewsManagement2PageModel()
    {
        LocalizationResourceType = typeof(NewsManagement2Resource);
    }
}
