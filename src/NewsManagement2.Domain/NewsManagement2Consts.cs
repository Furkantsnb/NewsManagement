using System;

namespace NewsManagement2;

public static class NewsManagement2Consts
{
    public const string DbTablePrefix = "App";
    public static Guid FilesImageId = Guid.Parse("17a4c001-a570-c250-60e0-18b9bf25b001");
    public static Guid UploadImageId = Guid.Parse("27a4c002-a570-c250-60e0-18b9bf25b002");
    public static Guid ChildTenanFilesImageId = Guid.Parse("37a4c002-a570-c250-60e0-18b9bf25b003");
    public static Guid ChildTenanUploadImageId = Guid.Parse("47a4c002-a570-c250-60e0-18b9bf25b004");
    public static Guid YoungTenanFilesImageId = Guid.Parse("57a4c002-a570-c250-60e0-18b9bf25b005");
    public static Guid YoungTenanUploadImageId = Guid.Parse("67a4c002-a570-c250-60e0-18b9bf25b006");


    public static string ChildTenanName = "Child";
    public static string YoungTenanName = "Young";

    public const string DbSchema = null;
}
