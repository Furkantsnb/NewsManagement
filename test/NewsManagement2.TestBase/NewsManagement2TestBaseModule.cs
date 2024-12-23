using EasyAbp.FileManagement;
using EasyAbp.FileManagement.Containers;
using EasyAbp.FileManagement.Files;
using EasyAbp.FileManagement.Options;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.DependencyInjection;
using NewsManagement2.Entities.BackgroundJobs;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.BlobStoring;
using Volo.Abp.BlobStoring.Minio;
using Volo.Abp.Data;
using Volo.Abp.Hangfire;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace NewsManagement2;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpTestBaseModule),
     typeof(AbpHangfireModule),
    typeof(AbpAuthorizationModule),
    typeof(AbpBlobStoringMinioModule),
    typeof(NewsManagement2DomainModule)
    )]
public class NewsManagement2TestBaseModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {

    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        Configure<AbpBackgroundJobOptions>(options =>
        {
            options.IsJobExecutionEnabled = false;
        });

        context.Services.AddHangfire(config =>
        {
            config.UsePostgreSqlStorage("Host=localhost;Port=5432;Database=NewsManagement;UserName=postgres;Password=1234;");
        });
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
        context.Services.AddHangfireServer();
        context.Services.AddAlwaysAllowAuthorization();
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        SeedTestData(context);
        RecurringJob.AddOrUpdate<UpdateScheduledStatusJob>(
          "ChangingStatusTypeJob",
           job => job.ExecuteAsync(0),
         Cron.MinuteInterval(1)
       );
    }

    private static void SeedTestData(ApplicationInitializationContext context)
    {
        AsyncHelper.RunSync(async () =>
        {
            using (var scope = context.ServiceProvider.CreateScope())
            {
                await scope.ServiceProvider
                    .GetRequiredService<IDataSeeder>()
                    .SeedAsync();
            }
        });
    }
}
