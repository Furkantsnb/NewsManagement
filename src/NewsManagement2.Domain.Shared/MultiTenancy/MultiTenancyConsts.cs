namespace NewsManagement2.MultiTenancy;

public static class MultiTenancyConsts
{
    /* Enable/disable multi-tenancy easily in a single point.
     * If you will never need to multi-tenancy, you can remove
     * related modules and code parts, including this file.
     */
    public const bool IsEnabled = true;

    public static string News = "NewsApp.News";
    public static string Video = "NewsApp.Video";
    public static string Gallery = "NewsApp.Gallery";
    public static string ListableContent = "NewsApp.ListableContent";
}
