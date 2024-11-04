using System;
using System.Collections.Generic;
using System.Text;
using NewsManagement2.Localization;
using Volo.Abp.Application.Services;

namespace NewsManagement2;

/* Inherit your application services from this class.
 */
public abstract class NewsManagement2AppService : ApplicationService
{
    protected NewsManagement2AppService()
    {
        LocalizationResource = typeof(NewsManagement2Resource);
    }
}
