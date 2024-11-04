using NewsManagement2.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace NewsManagement2.Permissions;

public class NewsManagement2PermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(NewsManagement2Permissions.GroupName);
        //Define your own permissions here. Example:
        //myGroup.AddPermission(NewsManagement2Permissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<NewsManagement2Resource>(name);
    }
}
