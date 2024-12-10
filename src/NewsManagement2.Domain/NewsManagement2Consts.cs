using System;

namespace NewsManagement2;

public static class NewsManagement2Consts
{
   public const string DbTablePrefix = "App";
  public static Guid FilesImageId = Guid.Parse("88b4c111-b780-d350-70f0-28d8df35c011");
  public static Guid UploadImageId = Guid.Parse("99b4c112-b780-d350-70f0-28d8df35c012");
  public static Guid ChildTenanFilesImageId = Guid.Parse("11c4c113-b780-d350-70f0-28d8df35c013");
  public static Guid ChildTenanUploadImageId = Guid.Parse("22c4c114-b780-d350-70f0-28d8df35c014");
  public static Guid YoungTenanFilesImageId = Guid.Parse("33c4c115-b780-d350-70f0-28d8df35c015");
  public static Guid YoungTenanUploadImageId = Guid.Parse("44c4c116-b780-d350-70f0-28d8df35c016");
  
  public static string ChildTenanName = "Child"; 
  public static string YoungTenanName = "Young"; 

  public const string DbSchema = null;
}
