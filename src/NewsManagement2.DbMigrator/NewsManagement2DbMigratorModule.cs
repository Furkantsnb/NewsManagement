using EasyAbp.FileManagement;
using EasyAbp.FileManagement.Containers;
using EasyAbp.FileManagement.Files;
using EasyAbp.FileManagement.Options;
using NewsManagement2.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.BlobStoring;
using Volo.Abp.BlobStoring.Minio;
using Volo.Abp.Modularity;

namespace NewsManagement2.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(NewsManagement2EntityFrameworkCoreModule),
    typeof(NewsManagement2ApplicationContractsModule),
      typeof(AbpBlobStoringMinioModule)
    )]
public class NewsManagement2DbMigratorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpBlobStoringOptions>(options =>
        {
            options.Containers.Configure<LocalFileSystemBlobContainer>(container =>
            {
                container.IsMultiTenant = true;
                container.UseMinio(minio =>
                {
                    minio.EndPoint = "localhost:9000";
                    minio.AccessKey = "PNH9Ckkt6StJu6KULk9x";
                    minio.SecretKey = "Rr8l7gZTfmuQIAYbYKQgWA6yryLXyyJstirOFSab";
                    minio.BucketName = "news";

                });
            });
        });
        Configure<FileManagementOptions>(options =>
        {
            options.DefaultFileDownloadProviderType = typeof(LocalFileDownloadProvider);
            options.Containers.Configure<CommonFileContainer>(container =>
            {
                // private container never be used by non-owner users (except user who has the "File.Manage" permission).
                container.FileContainerType = FileContainerType.Public;
                container.AbpBlobContainerName = BlobContainerNameAttribute.GetContainerName<LocalFileSystemBlobContainer>();
                container.AbpBlobDirectorySeparator = "/";

                container.RetainUnusedBlobs = false;
                container.EnableAutoRename = true;

                container.MaxByteSizeForEachFile = 5 * 1024 * 1024;
                container.MaxByteSizeForEachUpload = 10 * 1024 * 1024;
                container.MaxFileQuantityForEachUpload = 2;

                container.AllowOnlyConfiguredFileExtensions = true;
                container.FileExtensionsConfiguration.Add(".jpg", true);
                container.FileExtensionsConfiguration.Add(".PNG", true);
                // container.FileExtensionsConfiguration.Add(".tar.gz", true);
                // container.FileExtensionsConfiguration.Add(".exe", false);

                container.GetDownloadInfoTimesLimitEachUserPerMinute = 10;
            });
        });
    }
    
}
