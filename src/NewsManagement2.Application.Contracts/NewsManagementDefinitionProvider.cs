using NewsManagement2.Localization;
using NewsManagement2.MultiTenancy;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Features;
using Volo.Abp.Localization;

namespace NewsManagement2
{
    public class NewsManagementDefinitionProvider : FeatureDefinitionProvider
    {
        public override void Define(IFeatureDefinitionContext context)
        {
            var newsGroup = context.AddGroup("NewsApp");

            newsGroup.AddFeature(
             MultiTenancyConsts.News,
             displayName: LocalizableString.Create<NewsManagement2Resource>("News")
            );

            newsGroup.AddFeature(
              MultiTenancyConsts.Video,
              displayName: LocalizableString.Create<NewsManagement2Resource>("Video")
            );

            newsGroup.AddFeature(
             MultiTenancyConsts.Gallery,
             displayName: LocalizableString.Create<NewsManagement2Resource>("Gallery")
            );

            newsGroup.AddFeature(
             MultiTenancyConsts.ListableContent,
              displayName: LocalizableString.Create<NewsManagement2Resource>("ListableContent")
            );

        }
    }
}
