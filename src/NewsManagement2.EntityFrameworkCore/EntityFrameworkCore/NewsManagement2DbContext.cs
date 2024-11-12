using Microsoft.EntityFrameworkCore;
using NewsManagement2.Entities.Categories;
using NewsManagement2.Entities.Cities;
using NewsManagement2.Entities.Galleries;
using NewsManagement2.Entities.ListableContentRelations;
using NewsManagement2.Entities.ListableContents;
using NewsManagement2.Entities.Newses;
using NewsManagement2.Entities.Tags;
using NewsManagement2.Entities.Videos;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace NewsManagement2.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class NewsManagement2DbContext :
    AbpDbContext<NewsManagement2DbContext>,
    IIdentityDbContext,
    ITenantManagementDbContext
{

    public DbSet<Tag> Tags { get; set; }
    public DbSet<News> Newses { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Video> Videos { get; set; }
    public DbSet<Gallery> Galleries { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<GalleryImage> GalleryImages { get; set; }
    public DbSet<NewsDetailImage> NewsDetailImages { get; set; }
    public DbSet<ListableContent> ListableContents { get; set; }
    public DbSet<ListableContentTag> ListableContentTags { get; set; }
    public DbSet<ListableContentCity> ListableContentCities { get; set; }
    public DbSet<ListableContentRelation> ListableContentRelations { get; set; }
    public DbSet<ListableContentCategory> ListableContentCategories { get; set; }

    #region Entities from the modules

    //Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }

    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion

    public NewsManagement2DbContext(DbContextOptions<NewsManagement2DbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureFeatureManagement();
        builder.ConfigureTenantManagement();
       // builder.ConfigureFileManagement();
        #region Gallery, Video, News
        builder.Entity<Gallery>(b =>
        {

            b.ToTable(NewsManagement2Consts.DbTablePrefix + "Galleries", NewsManagement2Consts.DbSchema);
            b.ConfigureByConvention();
        });

        builder.Entity<GalleryImage>(b =>
        {
            b.HasKey(x => new { x.GalleryId, x.ImageId });

            b.HasOne(x => x.Gallery).WithMany(x => x.GalleryImages).HasForeignKey(x => x.GalleryId);


            b.ToTable(NewsManagement2Consts.DbTablePrefix + "GalleryImages", NewsManagement2Consts.DbSchema);
            b.ConfigureByConvention();
        });

        builder.Entity<NewsDetailImage>(b =>
        {
            b.HasKey(x => new { x.NewsId, x.DetailImageId });

            b.HasOne(x => x.News).WithMany(x => x.DetailImageIds).HasForeignKey(x => x.NewsId);

            b.ToTable(NewsManagement2Consts.DbTablePrefix + "NewsDetailImages", NewsManagement2Consts.DbSchema);
            b.ConfigureByConvention();
        });

        builder.Entity<News>(b =>
        {

            b.ToTable(NewsManagement2Consts.DbTablePrefix + "Newses", NewsManagement2Consts.DbSchema);
            b.ConfigureByConvention();
        });

        builder.Entity<Video>(b =>
        {

            b.ToTable(NewsManagement2Consts.DbTablePrefix + "Videos", NewsManagement2Consts.DbSchema);
            b.ConfigureByConvention();
        });

        #endregion

        #region ListableContent

        builder.Entity<ListableContentRelation>(b =>
        {
            b.HasKey(x => new { x.ListableContentId, x.RelatedListableContentId });

            b.HasOne(x => x.ListableContent).WithMany(x => x.ListableContentRelations).HasForeignKey(x => x.ListableContentId);//.OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.RelatedListableContent).WithMany().HasForeignKey(x => x.RelatedListableContentId);//.OnDelete(DeleteBehavior.Restrict);

            b.ToTable(NewsManagement2Consts.DbTablePrefix + "ListableContentRelations", NewsManagement2Consts.DbSchema);
            b.ConfigureByConvention();
        });

        #endregion

        #region ListableContent(Tag, City, Category)
        builder.Entity<ListableContentTag>(b =>
        {
            b.HasKey(x => new { x.ListableContentId, x.TagId });
            b.HasOne(x => x.ListableContent).WithMany(x => x.ListableContentTags).HasForeignKey(x => x.ListableContentId);
            b.HasOne(x => x.Tag).WithMany(x => x.ListableContentTags).HasForeignKey(x => x.TagId);

            b.ToTable(NewsManagement2Consts.DbTablePrefix + "ListableContentTags", NewsManagement2Consts.DbSchema);
            b.ConfigureByConvention();
        });

        builder.Entity<ListableContentCity>(b =>
        {
            b.HasKey(x => new { x.ListableContentId, x.CityId });
            b.HasOne(x => x.ListableContent).WithMany(x => x.ListableContentCities).HasForeignKey(x => x.ListableContentId);
            b.HasOne(x => x.City).WithMany(x => x.ListableContentCities).HasForeignKey(x => x.CityId);

            b.ToTable(NewsManagement2Consts.DbTablePrefix + "ListableContentCities", NewsManagement2Consts.DbSchema);
            b.ConfigureByConvention();
        });

        builder.Entity<ListableContentCategory>(b =>
        {
            b.HasKey(x => new { x.ListableContentId, x.CategoryId });
            b.HasOne(x => x.ListableContent).WithMany(x => x.ListableContentCategories).HasForeignKey(x => x.ListableContentId);
            b.HasOne(x => x.Category).WithMany(x => x.ListableContentCategories).HasForeignKey(x => x.CategoryId);

            b.ToTable(NewsManagement2Consts.DbTablePrefix + "ListableContentCategories", NewsManagement2Consts.DbSchema);
            b.ConfigureByConvention();
        });
        #endregion

        #region Tag, City, Category
        builder.Entity<Tag>(b =>
        {
            b.HasMany(x => x.ListableContentTags).WithOne(x => x.Tag).HasForeignKey(x => x.TagId).IsRequired();

            b.ToTable(NewsManagement2Consts.DbTablePrefix + "Tags", NewsManagement2Consts.DbSchema);
            b.ConfigureByConvention();
        });

        builder.Entity<City>(b =>
        {
            b.HasMany(x => x.ListableContentCities).WithOne(x => x.City).HasForeignKey(x => x.CityId).IsRequired();

            b.ToTable(NewsManagement2Consts.DbTablePrefix + "Cities", NewsManagement2Consts.DbSchema);
            b.ConfigureByConvention();
        });

        builder.Entity<Category>(b =>
        {
            b.HasMany(x => x.ListableContentCategories).WithOne(x => x.Category).HasForeignKey(x => x.CategoryId).IsRequired();

            b.ToTable(NewsManagement2Consts.DbTablePrefix + "Categories", NewsManagement2Consts.DbSchema);
            b.ConfigureByConvention();
        });
        #endregion
    }
}
