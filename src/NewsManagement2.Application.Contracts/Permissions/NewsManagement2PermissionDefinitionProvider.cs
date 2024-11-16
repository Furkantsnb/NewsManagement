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

        #region Tag
        var tagsPermission = myGroup.AddPermission(NewsManagement2Permissions.Tags.Default, L("Permission:Tags"));
        tagsPermission.AddChild(NewsManagement2Permissions.Tags.Create, L("Permission:Tags.Create"));
        tagsPermission.AddChild(NewsManagement2Permissions.Tags.Edit, L("Permission:Tags.Edit"));
        tagsPermission.AddChild(NewsManagement2Permissions.Tags.Delete, L("Permission:Tags.Delete"));
        #endregion

        #region City
        var citysPermission = myGroup.AddPermission(NewsManagement2Permissions.Cities.Default, L("Permission:Cities"));
        citysPermission.AddChild(NewsManagement2Permissions.Cities.Create, L("Permission:Cities.Create"));
        citysPermission.AddChild(NewsManagement2Permissions.Cities.Edit, L("Permission:Cities.Edit"));
        citysPermission.AddChild(NewsManagement2Permissions.Cities.Delete, L("Permission:Cities.Delete"));
        #endregion

        #region Category
        var categoriesPermission = myGroup.AddPermission(NewsManagement2Permissions.Categories.Default, L("Permission:Categories"));
        categoriesPermission.AddChild(NewsManagement2Permissions.Categories.Create, L("Permission:Categories.Create"));
        categoriesPermission.AddChild(NewsManagement2Permissions.Categories.Edit, L("Permission:Categories.Edit"));
        categoriesPermission.AddChild(NewsManagement2Permissions.Categories.Delete, L("Permission:Categories.Delete"));
        #endregion

        #region Video 
        var videosPermission = myGroup.AddPermission(NewsManagement2Permissions.Videos.Default, L("Permission:Videos"));
        videosPermission.AddChild(NewsManagement2Permissions.Videos.Create, L("Permission:Videos.Create"));
        videosPermission.AddChild(NewsManagement2Permissions.Videos.Edit, L("Permission:Videos.Edit"));
        videosPermission.AddChild(NewsManagement2Permissions.Videos.Delete, L("Permission:Videos.Delete"));
        #endregion

        #region News
        var newsesPermission = myGroup.AddPermission(NewsManagement2Permissions.Newses.Default, L("Permission:Newses"));
        newsesPermission.AddChild(NewsManagement2Permissions.Newses.Create, L("Permission:Newses.Create"));
        newsesPermission.AddChild(NewsManagement2Permissions.Newses.Edit, L("Permission:Newses.Edit"));
        newsesPermission.AddChild(NewsManagement2Permissions.Newses.Delete, L("Permission:Newses.Delete"));
        #endregion

        #region Gallery
        var galleriesPermission = myGroup.AddPermission(NewsManagement2Permissions.Galleries.Default, L("Permission:Galleries"));
        galleriesPermission.AddChild(NewsManagement2Permissions.Galleries.Create, L("Permission:Galleries.Create"));
        galleriesPermission.AddChild(NewsManagement2Permissions.Galleries.Edit, L("Permission:Galleries.Edit"));
        galleriesPermission.AddChild(NewsManagement2Permissions.Galleries.Delete, L("Permission:Galleries.Delete"));
        #endregion
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<NewsManagement2Resource>(name);
    }
}
