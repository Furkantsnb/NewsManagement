using EasyAbp.FileManagement.Files;
using EasyAbp.FileManagement.Options.Containers;
using NewsManagement2.Entities.Categories;
using NewsManagement2.Entities.Cities;
using NewsManagement2.Entities.Galleries;
using NewsManagement2.Entities.ListableContentRelations;
using NewsManagement2.Entities.Newses;
using NewsManagement2.Entities.Tags;
using NewsManagement2.Entities.Videos;
using NewsManagement2.EntityConsts.ListableContentConsts;
using NewsManagement2.EntityConsts.RoleConsts;
using NewsManagement2.EntityConsts.VideoConsts;
using NewsManagement2.MultiTenancy;
using NewsManagement2.Permissions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Features;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;
using Volo.Abp.TenantManagement;

namespace NewsManagement2
{
    internal class NewsManagementDataSeederContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IGuidGenerator _guidGenerator;
        private readonly IRepository<Tag, int> _tagRepository;
        private readonly IRepository<City, int> _cityRepository;
        private readonly IRepository<Category, int> _categoryRepository;
        private readonly IRepository<News, int> _newsRepository;
        private readonly IRepository<Video, int> _videoRepository;
        private readonly IRepository<Gallery, int> _galleryRepository;
        private readonly IRepository<GalleryImage> _galleryImageRepository;
        private readonly IRepository<NewsDetailImage> _newsDetailImageRepository;
        private readonly IRepository<ListableContentTag> _listableContentTagRepository;
        private readonly IRepository<ListableContentCity> _listableContentCityRepository;
        private readonly IRepository<ListableContentCategory> _listableContentCategoryRepository;
        private readonly IRepository<ListableContentRelation> _listableContentRelationRepository;
        private readonly IRepository<IdentityUser, Guid> _userRepository;
        private readonly IRepository<IdentityRole, Guid> _roleRepository;
        private readonly IdentityUserManager _identityUserManager;
        private readonly IdentityRoleManager _identityRoleManager;
        private readonly IPermissionManager _permissionManager;
        private readonly TenantManager _tenantManager;
        private readonly ITenantRepository _tenantRepository;
        private readonly ICurrentTenant _currentTenant;
        private readonly IFeatureManager _featureManager;
        private readonly IFeatureChecker _featureChecker;
        private readonly FileManager _fileManager;
        private readonly IFileRepository _fileRepository;
        private readonly IFileBlobNameGenerator _fileBlobNameGenerator;
        private readonly IFileContentHashProvider _fileContentHashProvider;
        private readonly IFileContainerConfigurationProvider _configurationProvider;

        public NewsManagementDataSeederContributor(IGuidGenerator guidGenerator,
            IRepository<Tag, int> tagRepository,
            IRepository<City, int> cityRepository,
            IRepository<Category, int> categoryRepository,
            IRepository<News, int> newsRepository,
            IRepository<Video, int> videoRepository,
            IRepository<Gallery, int> galleryRepository,
            IRepository<GalleryImage> galleryImageRepository,
            IRepository<NewsDetailImage> newsDetailImageRepository,
            IRepository<ListableContentTag> listableContentTagRepository,
            IRepository<ListableContentCity> listableContentCityRepository,
            IRepository<ListableContentCategory> listableContentCategoryRepository,
            IRepository<ListableContentRelation> listableContentRelationRepository,
            IRepository<IdentityUser, Guid> userRepository,
            IRepository<IdentityRole, Guid> roleRepository, IdentityUserManager identityUserManager, IdentityRoleManager identityRoleManager,
            IPermissionManager permissionManager, TenantManager tenantManager,
            ITenantRepository tenantRepository,
            ICurrentTenant currentTenant,
            IFeatureManager featureManager,
            IFeatureChecker featureChecker, FileManager fileManager,
            IFileRepository fileRepository,
            IFileBlobNameGenerator fileBlobNameGenerator,
            IFileContentHashProvider fileContentHashProvider,
            IFileContainerConfigurationProvider configurationProvider)
        {
            _guidGenerator = guidGenerator;
            _tagRepository = tagRepository;
            _cityRepository = cityRepository;
            _categoryRepository = categoryRepository;
            _newsRepository = newsRepository;
            _videoRepository = videoRepository;
            _galleryRepository = galleryRepository;
            _galleryImageRepository = galleryImageRepository;
            _newsDetailImageRepository = newsDetailImageRepository;
            _listableContentTagRepository = listableContentTagRepository;
            _listableContentCityRepository = listableContentCityRepository;
            _listableContentCategoryRepository = listableContentCategoryRepository;
            _listableContentRelationRepository = listableContentRelationRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _identityUserManager = identityUserManager;
            _identityRoleManager = identityRoleManager;
            _permissionManager = permissionManager;
            _tenantManager = tenantManager;
            _tenantRepository = tenantRepository;
            _currentTenant = currentTenant;
            _featureManager = featureManager;
            _featureChecker = featureChecker;
            _fileManager = fileManager;
            _fileRepository = fileRepository;
            _fileBlobNameGenerator = fileBlobNameGenerator;
            _fileContentHashProvider = fileContentHashProvider;
            _configurationProvider = configurationProvider;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            await SeedTenantAsync();
        }

        #region Tenants
        private async Task SeedTenantAsync()
        {

            if (await _tenantRepository.FindByNameAsync(NewsManagement2Consts.ChildTenanName) == null)
            {
                var childTenant = await _tenantManager.CreateAsync(NewsManagement2Consts.ChildTenanName);
                await _tenantRepository.InsertAsync(childTenant);

                using (_currentTenant.Change(childTenant.Id))
                {
                    await _featureManager.SetForTenantAsync(childTenant.Id, MultiTenancyConsts.Gallery, true.ToString());

                    var filesImageId = NewsManagement2Consts.ChildTenanFilesImageId;
                    var uploadImageId = NewsManagement2Consts.ChildTenanUploadImageId;

                    await SeedRoleAsync(childTenant.Id);
                    await SeedUserAsync(childTenant.Id);
                    await SeedTagAsync(childTenant.Id);
                    await SeedCityAsync(childTenant.Id);
                    await SeedCategoryAsync(childTenant.Id);
                  //  await SeedFileAsync(childTenant.Id, filesImageId, uploadImageId);
                    await SeedNewsAsync(childTenant.Id, filesImageId, uploadImageId);
                    await SeedVideoAsync(childTenant.Id, filesImageId, uploadImageId);
                    await SeedGalleryAsync(childTenant.Id, filesImageId, uploadImageId);
                }

            }

            if (await _tenantRepository.FindByNameAsync(NewsManagement2Consts.YoungTenanName) == null)
            {
                var youngTenant = await _tenantManager.CreateAsync(NewsManagement2Consts.YoungTenanName);
                await _tenantRepository.InsertAsync(youngTenant);

                using (_currentTenant.Change(youngTenant.Id))
                {
                    await _featureManager.SetForTenantAsync(youngTenant.Id, MultiTenancyConsts.Video, true.ToString());

                    var filesImageId = NewsManagement2Consts.YoungTenanFilesImageId;
                    var uploadImageId = NewsManagement2Consts.YoungTenanUploadImageId;

                    await SeedRoleAsync(youngTenant.Id);
                    await SeedUserAsync(youngTenant.Id);
                    await SeedTagAsync(youngTenant.Id);
                    await SeedCityAsync(youngTenant.Id);
                    await SeedCategoryAsync(youngTenant.Id);
                   // await SeedFileAsync(youngTenant.Id, filesImageId, uploadImageId);
                    await SeedNewsAsync(youngTenant.Id, filesImageId, uploadImageId);
                    await SeedVideoAsync(youngTenant.Id, filesImageId, uploadImageId);
                    await SeedGalleryAsync(youngTenant.Id, filesImageId, uploadImageId);
                }

            }
        }

        #endregion
        #region Roles
        private async Task SeedRoleAsync(Guid? tenantId)
        {

            if (!await _identityRoleManager.RoleExistsAsync(RoleConst.Editor))
            {
                await _identityRoleManager.CreateAsync(
                  new IdentityRole
                  (
                    _guidGenerator.Create(),
                    RoleConst.Editor,
                    tenantId: tenantId
                  )
                );

                await _permissionManager.SetForRoleAsync(RoleConst.Editor, NewsManagement2Permissions.Tags.Default, true);
                await _permissionManager.SetForRoleAsync(RoleConst.Editor, NewsManagement2Permissions.Tags.Create, true);
                await _permissionManager.SetForRoleAsync(RoleConst.Editor, NewsManagement2Permissions.Tags.Edit, true);
                await _permissionManager.SetForRoleAsync(RoleConst.Editor, NewsManagement2Permissions.Tags.Delete, true);

                await _permissionManager.SetForRoleAsync(RoleConst.Editor, NewsManagement2Permissions.Cities.Default, true);
                await _permissionManager.SetForRoleAsync(RoleConst.Editor, NewsManagement2Permissions.Cities.Create, true);
                await _permissionManager.SetForRoleAsync(RoleConst.Editor, NewsManagement2Permissions.Cities.Edit, true);
                await _permissionManager.SetForRoleAsync(RoleConst.Editor, NewsManagement2Permissions.Cities.Delete, true);

                await _permissionManager.SetForRoleAsync(RoleConst.Editor, NewsManagement2Permissions.Categories.Default, true);
                await _permissionManager.SetForRoleAsync(RoleConst.Editor, NewsManagement2Permissions.Categories.Create, true);
                await _permissionManager.SetForRoleAsync(RoleConst.Editor, NewsManagement2Permissions.Categories.Edit, true);
                await _permissionManager.SetForRoleAsync(RoleConst.Editor, NewsManagement2Permissions.Categories.Delete, true);

                await _permissionManager.SetForRoleAsync(RoleConst.Editor, NewsManagement2Permissions.Videos.Default, true);
                await _permissionManager.SetForRoleAsync(RoleConst.Editor, NewsManagement2Permissions.Videos.Create, true);
                await _permissionManager.SetForRoleAsync(RoleConst.Editor, NewsManagement2Permissions.Videos.Edit, true);
                await _permissionManager.SetForRoleAsync(RoleConst.Editor, NewsManagement2Permissions.Videos.Delete, true);

                await _permissionManager.SetForRoleAsync(RoleConst.Editor, NewsManagement2Permissions.Galleries.Default, true);
                await _permissionManager.SetForRoleAsync(RoleConst.Editor, NewsManagement2Permissions.Galleries.Create, true);
                await _permissionManager.SetForRoleAsync(RoleConst.Editor, NewsManagement2Permissions.Galleries.Edit, true);
                await _permissionManager.SetForRoleAsync(RoleConst.Editor, NewsManagement2Permissions.Galleries.Delete, true);

                await _permissionManager.SetForRoleAsync(RoleConst.Editor, NewsManagement2Permissions.Newses.Default, true);
                await _permissionManager.SetForRoleAsync(RoleConst.Editor, NewsManagement2Permissions.Newses.Create, true);
                await _permissionManager.SetForRoleAsync(RoleConst.Editor, NewsManagement2Permissions.Newses.Edit, true);
                await _permissionManager.SetForRoleAsync(RoleConst.Editor, NewsManagement2Permissions.Newses.Delete, true);

            }

            if (!await _identityRoleManager.RoleExistsAsync(RoleConst.Reporter))
            {
                await _identityRoleManager.CreateAsync(
                  new IdentityRole
                  (
                    _guidGenerator.Create(),
                    RoleConst.Reporter,
                    tenantId: tenantId
                  )
                );

                await _permissionManager.SetForRoleAsync(RoleConst.Reporter, NewsManagement2Permissions.Videos.Default, true);
                await _permissionManager.SetForRoleAsync(RoleConst.Reporter, NewsManagement2Permissions.Videos.Create, true);

                await _permissionManager.SetForRoleAsync(RoleConst.Reporter, NewsManagement2Permissions.Galleries.Default, true);
                await _permissionManager.SetForRoleAsync(RoleConst.Reporter, NewsManagement2Permissions.Galleries.Create, true);

                await _permissionManager.SetForRoleAsync(RoleConst.Reporter, NewsManagement2Permissions.Newses.Default, true);
                await _permissionManager.SetForRoleAsync(RoleConst.Reporter, NewsManagement2Permissions.Newses.Create, true);

            }

        }

        #endregion
        #region Tags
        private async Task SeedTagAsync(Guid? tenantId)
        {
            if (await _tagRepository.CountAsync() > 0)
                return;

            await _tagRepository.InsertAsync(
              new Tag()
              {
                  TagName = "Tatil",
                  TenantId = tenantId
              },
              autoSave: true
            );

            await _tagRepository.InsertAsync(
              new Tag()
              {
                  TagName = "Teknoloji",
                  TenantId = tenantId
              },
              autoSave: true
            );

            await _tagRepository.InsertAsync(
              new Tag()
              {
                  TagName = "Haber",
                  TenantId = tenantId
              },
              autoSave: true
            );

            await _tagRepository.InsertAsync(
              new Tag()
              {
                  TagName = "Eğitim",
                  TenantId = tenantId
              },
              autoSave: true
            );

        }
        #endregion
        #region Users

        private async Task SeedUserAsync(Guid? tenantId)
        {
            if (!await _userRepository.AnyAsync(x => x.UserName == "Furkan"))
            {
                var userFurkan = new IdentityUser
                  (
                    _guidGenerator.Create(),
                    "Furkan",
                    "furkan@gmail.com",
                    tenantId
                  );

                await _identityUserManager.CreateAsync(userFurkan, "1q2w3E*");
                await _identityUserManager.SetRolesAsync(
                  userFurkan, new List<string> { RoleConst.Editor }
                );
            }

            if (!await _userRepository.AnyAsync(x => x.UserName == "Arif"))
            {
                var userArif = new IdentityUser
                  (
                    _guidGenerator.Create(),
                    "Arif",
                    "arif@gmail.com",
                    tenantId
                  );

                await _identityUserManager.CreateAsync(userArif, "1q2w3E*");
                await _identityUserManager.SetRolesAsync(userArif, new List<string> { RoleConst.Reporter });
            }

            if (!await _userRepository.AnyAsync(x => x.UserName == "Ahmet"))
            {
                var userAhmet = new IdentityUser
                  (
                    _guidGenerator.Create(),
                    "Ahmet",
                    "ahmet2@gmail.com",
                    tenantId
                  );

                await _identityUserManager.CreateAsync(userAhmet, "1q2w3E*");
                await _identityUserManager.SetRolesAsync(userAhmet, new List<string> { RoleConst.Reporter });
            }

            if (!await _userRepository.AnyAsync(x => x.UserName == "Danyal"))
            {
                var userDanyal = new IdentityUser
                  (
                    _guidGenerator.Create(),
                    "Danyal",
                    "danyal@gmail.com",
                    tenantId
                  );

                await _identityUserManager.CreateAsync(userDanyal, "1q2w3E*");
                await _identityUserManager.SetRolesAsync(userDanyal, new List<string> { RoleConst.Reporter });
            }
        }

        #endregion
        #region Categories
        private async Task SeedCategoryAsync(Guid? tenantId)
        {
            if (await _categoryRepository.CountAsync() > 0)
                return;

            await _categoryRepository.InsertAsync(
              new Category()
              {
                  CategoryName = "Yazılım",
                  ColorCode = "#f9e79f",
                  IsActive = true,
                  ParentCategoryId = null,
                  listableContentType = ListableContentType.Gallery,
                  TenantId = tenantId
              },
              autoSave: true
            );

            await _categoryRepository.InsertAsync(
              new Category()
              {
                  CategoryName = "Bilgisayar",
                  ColorCode = "#148f77",
                  IsActive = true,
                  ParentCategoryId = null,
                  listableContentType = ListableContentType.Video,
                  TenantId = tenantId
              },
              autoSave: true
            );

            await _categoryRepository.InsertAsync(
              new Category()
              {
                  CategoryName = "Eğitim",
                  ColorCode = "#ec7063",
                  IsActive = true,
                  ParentCategoryId = null,
                  listableContentType = ListableContentType.News,
                  TenantId = tenantId
              },
              autoSave: true
            );

            await _categoryRepository.InsertAsync(
              new Category()
              {
                  CategoryName = "Üniversite",
                  ColorCode = "#ec7063",
                  IsActive = true,
                  ParentCategoryId = null,
                  listableContentType = ListableContentType.News,
                  TenantId = tenantId
              },
              autoSave: true
            );

            await _categoryRepository.InsertAsync(
              new Category()
              {
                  CategoryName = "Haber",
                  ColorCode = "#ec7063",
                  IsActive = true,
                  ParentCategoryId = null,
                  listableContentType = ListableContentType.News,
                  TenantId = tenantId
              },
              autoSave: true
            );

            await _categoryRepository.InsertAsync(
              new Category()
              {
                  CategoryName = "Yazılım Mühendisi",
                  ColorCode = "#ec70ff",
                  IsActive = true,
                  ParentCategoryId = (await _categoryRepository.GetAsync(c => c.CategoryName == "Yazılım")).Id,
                  listableContentType = ListableContentType.Gallery,
                  TenantId = tenantId
              },
              autoSave: true
            );

            await _categoryRepository.InsertAsync(
              new Category()
              {
                  CategoryName = "Bilgisayar Mühendisi",
                  ColorCode = "#8c7063",
                  IsActive = true,
                  ParentCategoryId = (await _categoryRepository.GetAsync(c => c.CategoryName == "Bilgisayar")).Id,
                  listableContentType = ListableContentType.Gallery,
                  TenantId = tenantId
              },
              autoSave: true
            );

            await _categoryRepository.InsertAsync(
              new Category()
              {
                  CategoryName = "Eğitim Sistemi",
                  ColorCode = "#7c0e63",
                  IsActive = true,
                  ParentCategoryId = (await _categoryRepository.GetAsync(c => c.CategoryName == "Eğitim")).Id,
                  listableContentType = ListableContentType.Video,
                  TenantId = tenantId
              },
              autoSave: true
            );

            await _categoryRepository.InsertAsync(
              new Category()
              {
                  CategoryName = "Kodlama",
                  ColorCode = "#7a0e65",
                  IsActive = true,
                  ParentCategoryId = (await _categoryRepository.GetAsync(c => c.CategoryName == "Yazılım")).Id,
                  listableContentType = ListableContentType.Video,
                  TenantId = tenantId
              },
              autoSave: true
            );

        }
        #endregion
        #region Cities
        private async Task SeedCityAsync(Guid? tenantId)
        {
            if (await _cityRepository.CountAsync() > 0)
                return;

            await _cityRepository.InsertAsync(
              new City()
              {
                  TenantId = tenantId,
                  CityName = "İstanbul",
                  CityCode = 34,
              },
              autoSave: true
            );

            await _cityRepository.InsertAsync(
              new City()
              {
                  TenantId = tenantId,
                  CityName = "Batman",
                  CityCode = 72
              },
              autoSave: true
            );

            await _cityRepository.InsertAsync(
              new City()
              {
                  TenantId = tenantId,
                  CityName = "Elazığ",
                  CityCode = 23
              },
              autoSave: true
            );

            await _cityRepository.InsertAsync(
              new City()
              {
                  TenantId = tenantId,
                  CityName = "Malatya",
                  CityCode = 44
              },
              autoSave: true
            );

            await _cityRepository.InsertAsync(
              new City()
              {
                  TenantId = tenantId,
                  CityName = "Diyarbakır",
                  CityCode = 21
              },
              autoSave: true
            );

        }
        #endregion
        //#region Files
        //private async Task SeedFileAsync(Guid? tenantId, Guid filesImageId, Guid uploadImageId)
        //{
        //    if (await _fileRepository.CountAsync() > 0)
        //        return;

        //    var projectRoot = Directory.GetCurrentDirectory();
        //    //if (tenantId == null)
        //    projectRoot = Directory.GetParent(projectRoot).Parent.Parent.Parent.Parent.CreateSubdirectory("src\\NewsManagement2.Web").FullName;

        //    var containerName = "default";
        //    var typeProvider = new FileExtensionContentTypeProvider();

        //    #region Files

        //    var filesPath = Path.Combine(projectRoot, "wwwroot", "dosya.jpg");
        //    var filesName = Path.GetFileName(filesPath);
        //    typeProvider.TryGetContentType(filesPath, out var filesMimeType);
        //    //C:\Users\furka\Desktop\ABP\NewsManagement2\src\NewsManagement2.Web\wwwroot\dosya.jpg
        //    var byteSizeOfFiles = System.IO.File.ReadAllBytes(filesPath);
        //    var filesHashString = _fileContentHashProvider.GetHashString(byteSizeOfFiles);

        //    var filesConfiguration = _configurationProvider.Get(containerName);
        //    var filesBlobName = await _fileBlobNameGenerator.CreateAsync(FileType.RegularFile, filesName, null, filesMimeType, filesConfiguration.AbpBlobDirectorySeparator);

        //    var files = new EasyAbp.FileManagement.Files.File(
        //      id: uploadImageId,
        //      tenantId: tenantId,
        //      parent: null,
        //      fileContainerName: containerName,
        //      fileName: filesName,
        //      mimeType: filesMimeType,
        //      fileType: FileType.RegularFile,
        //      subFilesQuantity: 0,
        //      byteSize: byteSizeOfFiles.Length,
        //      hash: filesHashString,
        //      blobName: filesBlobName,
        //      ownerUserId: null
        //    );

        //    await _fileRepository.InsertAsync(files, autoSave: true);
        //    await _fileManager.TrySaveBlobAsync(files, byteSizeOfFiles);

        //    #endregion

        //    #region Upload

        //    var uploadPath = Path.Combine(projectRoot, "wwwroot", "upload.jpg");
        //    var uploadName = Path.GetFileName(uploadPath);
        //    typeProvider.TryGetContentType(uploadPath, out var uploadMimeType);
        //    var byteSizeOfUpload = System.IO.File.ReadAllBytes(uploadPath);
        //    var uploadHashString = _fileContentHashProvider.GetHashString(byteSizeOfUpload);

        //    var uploadConfiguration = _configurationProvider.Get(containerName);
        //    var uploadBlobName = await _fileBlobNameGenerator.CreateAsync(FileType.RegularFile, uploadName, null, uploadMimeType, uploadConfiguration.AbpBlobDirectorySeparator);

        //    var upload = new EasyAbp.FileManagement.Files.File(
        //      id: filesImageId,
        //      tenantId: tenantId,
        //      parent: null,
        //      fileContainerName: containerName,
        //      fileName: uploadName,
        //      mimeType: uploadMimeType,
        //      fileType: FileType.RegularFile,
        //      subFilesQuantity: 0,
        //      byteSize: byteSizeOfUpload.Length,
        //      hash: uploadHashString,
        //      blobName: uploadBlobName,
        //      ownerUserId: null
        //    );

        //    await _fileRepository.InsertAsync(upload, autoSave: true);
        //    await _fileManager.TrySaveBlobAsync(upload, byteSizeOfUpload);

        //    #endregion

        //}
        //#endregion
        #region Newses

        private async Task SeedNewsAsync(Guid? tenantId, Guid filesImageId, Guid uploadImageId)
        {
            if (await _newsRepository.CountAsync() > 0)
                return;

            // etiketler
            var tagTatilId = (await _tagRepository.GetAsync(x => x.TagName == "Tatil")).Id;
            var tagTeknolojiId = (await _tagRepository.GetAsync(x => x.TagName == "Teknoloji")).Id;
            var tagHaberId = (await _tagRepository.GetAsync(x => x.TagName == "Haber")).Id;
            var tagEgitimId = (await _tagRepository.GetAsync(x => x.TagName == "Eğitim")).Id;

            //  şehirler
            var cityBatmanId = (await _cityRepository.GetAsync(x => x.CityName == "Batman")).Id;
            var cityElazigId = (await _cityRepository.GetAsync(x => x.CityName == "Elazığ")).Id;
            var cityMalatyaId = (await _cityRepository.GetAsync(x => x.CityName == "Malatya")).Id;
            var cityIstanbulId = (await _cityRepository.GetAsync(x => x.CityName == "İstanbul")).Id;
            var cityDiyarbakirId = (await _cityRepository.GetAsync(x => x.CityName == "Diyarbakır")).Id;

            //  kategoriler
            var categorySoftwareId = (await _categoryRepository.GetAsync(c => c.CategoryName == "Yazılım")).Id;
            var categoryComputerId = (await _categoryRepository.GetAsync(c => c.CategoryName == "Bilgisayar")).Id;
            var categoryEducationId = (await _categoryRepository.GetAsync(c => c.CategoryName == "Eğitim")).Id;
            var categoryUniversityId = (await _categoryRepository.GetAsync(c => c.CategoryName == "Üniversite")).Id;
            var categoryNewsId = (await _categoryRepository.GetAsync(c => c.CategoryName == "Haber")).Id;

            var categoryCodingId = (await _categoryRepository.GetAsync(c => c.CategoryName == "Kodlama")).Id;
            var categoryEducationSystemId = (await _categoryRepository.GetAsync(c => c.CategoryName == "Eğitim Sistemi")).Id;
            var categoryComputerEngineeringId = (await _categoryRepository.GetAsync(c => c.CategoryName == "Bilgisayar Mühendisi")).Id;
            var categorySoftwareEngineeringId = (await _categoryRepository.GetAsync(c => c.CategoryName == "Yazılım Mühendisi")).Id;


            #region News Haber 1

            await _newsRepository.InsertAsync(
                new News()
                {
                    Title = "News Haber 1",
                    Spot = "News haber 1 içeriği",
                    ImageId = filesImageId,
                    TenantId = tenantId,
                    Status = StatusType.Draft,
                    PublishTime = null,
                    ListableContentType = ListableContentType.News
                },
                autoSave: true
            );

            var news1Id = (await _newsRepository.GetAsync(x => x.Title == "News Haber 1")).Id;

            await _newsDetailImageRepository.InsertManyAsync(
                new List<NewsDetailImage>()
                {
        new() { NewsId = news1Id, DetailImageId = filesImageId, TenantId = tenantId },
        new() { NewsId = news1Id, DetailImageId = uploadImageId, TenantId = tenantId }
                }
            );

            await _listableContentTagRepository.InsertManyAsync(
                new List<ListableContentTag>()
                {
        new() { ListableContentId = news1Id, TagId = tagTatilId, TenantId = tenantId },
        new() { ListableContentId = news1Id, TagId = tagTeknolojiId, TenantId = tenantId },
        new() { ListableContentId = news1Id, TagId = tagHaberId, TenantId = tenantId }
                },
                autoSave: true
            );

            await _listableContentCityRepository.InsertAsync(
                new ListableContentCity()
                {
                    ListableContentId = news1Id,
                    CityId = cityBatmanId,
                    TenantId = tenantId
                },
                autoSave: true
            );

            await _listableContentCategoryRepository.InsertManyAsync(
                new List<ListableContentCategory>()
                {
        new() { ListableContentId = news1Id, CategoryId = categorySoftwareId, IsPrimary = true, TenantId = tenantId },
        new() { ListableContentId = news1Id, CategoryId = categoryEducationId, TenantId = tenantId }
                },
                autoSave: true
            );

            #endregion

            #region News Haber 2

            await _newsRepository.InsertAsync(
                new News()
                {
                    Title = "News Haber 2",
                    Spot = "News haber 2 içeriği",
                    ImageId = uploadImageId,
                    TenantId = tenantId,
                    Status = StatusType.Published,
                    PublishTime = DateTime.Now,
                    ListableContentType = ListableContentType.News
                },
                autoSave: true
            );

            var news2Id = (await _newsRepository.GetAsync(x => x.Title == "News Haber 2")).Id;

            await _newsDetailImageRepository.InsertManyAsync(
                new List<NewsDetailImage>()
                {
        new() { NewsId = news2Id, DetailImageId = uploadImageId, TenantId = tenantId },
        new() { NewsId = news2Id, DetailImageId = filesImageId, TenantId = tenantId }
                }
            );

            await _listableContentTagRepository.InsertManyAsync(
                new List<ListableContentTag>()
                {
        new() { ListableContentId = news2Id, TagId = tagEgitimId, TenantId = tenantId },
        new() { ListableContentId = news2Id, TagId = tagTatilId, TenantId = tenantId }
                },
                autoSave: true
            );

            await _listableContentCityRepository.InsertManyAsync(
                new List<ListableContentCity>()
                {
        new() { ListableContentId = news2Id, CityId = cityElazigId, TenantId = tenantId },
        new() { ListableContentId = news2Id, CityId = cityDiyarbakirId, TenantId = tenantId }
                },
                autoSave: true
            );

            await _listableContentCategoryRepository.InsertManyAsync(
                new List<ListableContentCategory>()
                {
        new() { ListableContentId = news2Id, CategoryId = categoryEducationSystemId, TenantId = tenantId },
        new() { ListableContentId = news2Id, CategoryId = categoryUniversityId, IsPrimary = true, TenantId = tenantId }
                },
                autoSave: true
            );

            await _listableContentRelationRepository.InsertAsync(
                new ListableContentRelation()
                {
                    ListableContentId = news2Id,
                    RelatedListableContentId = news1Id,
                    TenantId = tenantId
                },
                autoSave: true
            );

            #endregion

            #region News Haber 3

            await _newsRepository.InsertAsync(
                new News()
                {
                    Title = "News Haber 3",
                    Spot = "News Haber 3 içeriği",
                    ImageId = uploadImageId,
                    TenantId = tenantId,
                    Status = StatusType.Published,
                    PublishTime = DateTime.Now,
                    ListableContentType = ListableContentType.News
                },
                autoSave: true
            );

            var news3Id = (await _newsRepository.GetAsync(x => x.Title == "News Haber 3")).Id;

            await _newsDetailImageRepository.InsertManyAsync(
                new List<NewsDetailImage>()
                {
        new() { NewsId = news3Id, DetailImageId = filesImageId, TenantId = tenantId },
        new() { NewsId = news3Id, DetailImageId = uploadImageId, TenantId = tenantId }
                }
            );

            await _listableContentTagRepository.InsertManyAsync(
                new List<ListableContentTag>()
                {
        new() { ListableContentId = news3Id, TagId = tagTeknolojiId, TenantId = tenantId },
        new() { ListableContentId = news3Id, TagId = tagHaberId, TenantId = tenantId }
                },
                autoSave: true
            );

            await _listableContentCityRepository.InsertManyAsync(
                new List<ListableContentCity>()
                {
        new() { ListableContentId = news3Id, CityId = cityIstanbulId, TenantId = tenantId },
        new() { ListableContentId = news3Id, CityId = cityMalatyaId, TenantId = tenantId }
                },
                autoSave: true
            );

            await _listableContentCategoryRepository.InsertManyAsync(
                new List<ListableContentCategory>()
                {
        new() { ListableContentId = news3Id, CategoryId = categoryComputerEngineeringId, IsPrimary = true, TenantId = tenantId },
        new() { ListableContentId = news3Id, CategoryId = categorySoftwareEngineeringId, TenantId = tenantId }
                },
                autoSave: true
            );

            await _listableContentRelationRepository.InsertManyAsync(
                new List<ListableContentRelation>()
                {
        new() { ListableContentId = news3Id, RelatedListableContentId = news1Id, TenantId = tenantId },
        new() { ListableContentId = news3Id, RelatedListableContentId = news2Id, TenantId = tenantId }
                },
                autoSave: true
            );

            #endregion

            #region News Haber 4

            await _newsRepository.InsertAsync(
                new News()
                {
                    Title = "News Haber 4",
                    Spot = "News Haber 4 içeriği",
                    ImageId = filesImageId,
                    TenantId = tenantId,
                    Status = StatusType.Published,
                    PublishTime = DateTime.Now,
                    ListableContentType = ListableContentType.News
                },
                autoSave: true
            );

            var news4Id = (await _newsRepository.GetAsync(x => x.Title == "News Haber 4")).Id;

            await _newsDetailImageRepository.InsertManyAsync(
                new List<NewsDetailImage>()
                {
        new() { NewsId = news4Id, DetailImageId = uploadImageId, TenantId = tenantId },
        new() { NewsId = news4Id, DetailImageId = filesImageId, TenantId = tenantId }
                }
            );

            await _listableContentTagRepository.InsertManyAsync(
                new List<ListableContentTag>()
                {
        new() { ListableContentId = news4Id, TagId = tagEgitimId, TenantId = tenantId },
        new() { ListableContentId = news4Id, TagId = tagTatilId, TenantId = tenantId }
                },
                autoSave: true
            );

            await _listableContentCityRepository.InsertManyAsync(
                new List<ListableContentCity>()
                {
        new() { ListableContentId = news4Id, CityId = cityBatmanId, TenantId = tenantId },
        new() { ListableContentId = news4Id, CityId = cityDiyarbakirId, TenantId = tenantId }
                },
                autoSave: true
            );

            await _listableContentCategoryRepository.InsertManyAsync(
                new List<ListableContentCategory>()
                {
        new() { ListableContentId = news4Id, CategoryId = categoryCodingId, IsPrimary = true, TenantId = tenantId },
        new() { ListableContentId = news4Id, CategoryId = categoryUniversityId, TenantId = tenantId }
                },
                autoSave: true
            );

            await _listableContentRelationRepository.InsertManyAsync(
                new List<ListableContentRelation>()
                {
        new() { ListableContentId = news4Id, RelatedListableContentId = news2Id, TenantId = tenantId },
        new() { ListableContentId = news4Id, RelatedListableContentId = news3Id, TenantId = tenantId },
        new() { ListableContentId = news4Id, RelatedListableContentId = news1Id, TenantId = tenantId }
                },
                autoSave: true
            );

            #endregion


        }

        #endregion
        #region Videos

        private async Task SeedVideoAsync(Guid? tenantId, Guid filesImageId, Guid uploadImageId)
        {
            if (await _videoRepository.CountAsync() > 0)
                return;

            var tagTatilId = (await _tagRepository.GetAsync(x => x.TagName == "Tatil")).Id;
            var tagTeknolojiId = (await _tagRepository.GetAsync(x => x.TagName == "Teknoloji")).Id;
            var tagHaberId = (await _tagRepository.GetAsync(x => x.TagName == "Haber")).Id;
            var tagEgitimId = (await _tagRepository.GetAsync(x => x.TagName == "Eğitim")).Id;

            var cityBatmanId = (await _cityRepository.GetAsync(x => x.CityName == "Batman")).Id;
            var cityElazigId = (await _cityRepository.GetAsync(x => x.CityName == "Elazığ")).Id;
            var cityMalatyaId = (await _cityRepository.GetAsync(x => x.CityName == "Malatya")).Id;
            var cityIstanbulId = (await _cityRepository.GetAsync(x => x.CityName == "İstanbul")).Id;
            var cityDiyarbakirId = (await _cityRepository.GetAsync(x => x.CityName == "Diyarbakır")).Id;

            var categorySoftwareId = (await _categoryRepository.GetAsync(c => c.CategoryName == "Yazılım")).Id;
            var categoryComputerId = (await _categoryRepository.GetAsync(c => c.CategoryName == "Bilgisayar")).Id;
            var categoryEducationId = (await _categoryRepository.GetAsync(c => c.CategoryName == "Eğitim")).Id;
            var categoryUniversityId = (await _categoryRepository.GetAsync(c => c.CategoryName == "Üniversite")).Id;
            var categoryNewsId = (await _categoryRepository.GetAsync(c => c.CategoryName == "Haber")).Id;

            var categoryCodingId = (await _categoryRepository.GetAsync(c => c.CategoryName == "Kodlama")).Id;
            var categoryEducationSystemId = (await _categoryRepository.GetAsync(c => c.CategoryName == "Eğitim Sistemi")).Id;
            var categoryComputerEngineeringId = (await _categoryRepository.GetAsync(c => c.CategoryName == "Bilgisayar Mühendisi")).Id;
            var categorySoftwareEngineeringId = (await _categoryRepository.GetAsync(c => c.CategoryName == "Yazılım Mühendisi")).Id;

            var news1Id = (await _newsRepository.GetAsync(x => x.Title == "News Haber 1")).Id;
            var news2Id = (await _newsRepository.GetAsync(x => x.Title == "News Haber 2")).Id;
            var news3Id = (await _newsRepository.GetAsync(x => x.Title == "News Haber 3")).Id;
            var news4Id = (await _newsRepository.GetAsync(x => x.Title == "News Haber 4")).Id;

            #region Video Haber 1 (Url)

            await _videoRepository.InsertAsync(
              new Video()
              {
                  Title = "Video Haber 1",
                  Spot = "Video haber 1 içeriği",
                  ImageId = filesImageId,
                  TenantId = tenantId,
                  Status = StatusType.Published,
                  PublishTime = DateTime.Now,
                  ListableContentType = ListableContentType.Video,
                  VideoType = VideoType.Link,
                  Url = "www.dogruhaber.com.tr"
              },
              autoSave: true
            );

            var video1Id = (await _videoRepository.GetAsync(x => x.Title == "Video Haber 1")).Id;

            await _listableContentTagRepository.InsertAsync(
              new ListableContentTag()
              {
                  ListableContentId = video1Id,
                  TagId = tagTatilId,
                  TenantId = tenantId
              },
              autoSave: true
            );

            await _listableContentCityRepository.InsertManyAsync(
              new List<ListableContentCity>()
              {
      new()
      {
        ListableContentId = video1Id,
        CityId = cityIstanbulId,
        TenantId =tenantId
      },
      new()
      {
        ListableContentId = video1Id,
        CityId =cityMalatyaId,
        TenantId =tenantId
      }
              }
              , autoSave: true
            );

            await _listableContentCategoryRepository.InsertAsync(
              new ListableContentCategory()
              {
                  ListableContentId = video1Id,
                  CategoryId = categoryUniversityId,
                  IsPrimary = true,
                  TenantId = tenantId
              },
              autoSave: true
            );

            await _listableContentRelationRepository.InsertManyAsync(
              new List<ListableContentRelation>()
              {
      new()
      {
        ListableContentId = video1Id,
        RelatedListableContentId = news1Id,
        TenantId =tenantId
      },
      new()
      {
        ListableContentId = video1Id,
        RelatedListableContentId = news4Id,
        TenantId =tenantId
      }
              },
              autoSave: true
            );

            #endregion

            #region Video Haber 2 (VideoId)

            await _videoRepository.InsertAsync(
              new Video()
              {
                  Title = "Video Haber 2",
                  Spot = "Video haber 2 içeriği",
                  ImageId = filesImageId,
                  TenantId = tenantId,
                  Status = StatusType.Published,
                  PublishTime = DateTime.Now,
                  ListableContentType = ListableContentType.Video,
                  VideoType = VideoType.Video,
                  VideoId = uploadImageId
              },
              autoSave: true
            );

            var video2Id = (await _videoRepository.GetAsync(x => x.Title == "Video Haber 2")).Id;

            await _listableContentTagRepository.InsertAsync(
              new ListableContentTag()
              {
                  ListableContentId = video2Id,
                  TagId = tagTeknolojiId,
                  TenantId = tenantId
              },
              autoSave: true
            );

            await _listableContentCityRepository.InsertManyAsync(
              new List<ListableContentCity>()
              {
      new()
      {
        ListableContentId = video2Id,
        CityId = cityElazigId,
        TenantId =tenantId
      },
      new()
      {
        ListableContentId = video2Id,
        CityId = cityBatmanId,
        TenantId =tenantId
      }
              }
              , autoSave: true
            );

            await _listableContentCategoryRepository.InsertManyAsync(
              new List<ListableContentCategory>()
              {
      new()
      {
        ListableContentId = video2Id,
        CategoryId = categoryEducationId,
         IsPrimary = true,
        TenantId =tenantId
      },
      new()
      {
        ListableContentId = video2Id,
        CategoryId = categoryEducationSystemId,
        TenantId =tenantId
      },
      new()
      {
        ListableContentId = video2Id,
        CategoryId = categoryUniversityId,
        TenantId =tenantId
      }
              },
              autoSave: true
            );

            await _listableContentRelationRepository.InsertManyAsync(
              new List<ListableContentRelation>()
              {
      new()
      {
        ListableContentId = video2Id,
        RelatedListableContentId = news1Id,
        TenantId =tenantId
      },
      new()
      {
        ListableContentId = video2Id,
        RelatedListableContentId = news3Id,
        TenantId =tenantId
      },
      new()
      {
        ListableContentId = video2Id,
        RelatedListableContentId = news4Id,
        TenantId =tenantId
      },
      new()
      {
        ListableContentId = video2Id,
        RelatedListableContentId = news2Id,
        TenantId =tenantId
      },
      new()
      {
        ListableContentId = video2Id,
        RelatedListableContentId = video1Id,
        TenantId =tenantId
      }
              },
              autoSave: true
            );

            #endregion

            #region Video Haber 3 (VideoId)

            await _videoRepository.InsertAsync(
              new Video()
              {
                  Title = "Video Haber 3",
                  Spot = "Video haber 3 içeriği",
                  ImageId = filesImageId,
                  TenantId = tenantId,
                  Status = StatusType.Published,
                  PublishTime = DateTime.Now,
                  ListableContentType = ListableContentType.Video,
                  VideoType = VideoType.Video,
                  VideoId = uploadImageId
              },
              autoSave: true
            );

            var video3Id = (await _videoRepository.GetAsync(x => x.Title == "Video Haber 3")).Id;

            await _listableContentTagRepository.InsertManyAsync(
              new List<ListableContentTag>()
              {
      new()
      {
        ListableContentId = video3Id,
        TagId = tagTeknolojiId,
        TenantId = tenantId
      },
      new()
      {
        ListableContentId = video3Id,
        TagId = tagHaberId,
        TenantId = tenantId
      },
      new()
      {
        ListableContentId = video3Id,
        TagId = tagEgitimId,
        TenantId = tenantId
      },
              },
              autoSave: true
            );

            await _listableContentCityRepository.InsertManyAsync(
              new List<ListableContentCity>()
              {
      new()
      {
        ListableContentId = video3Id,
        CityId = cityIstanbulId,
        TenantId =tenantId
      },
      new()
      {
        ListableContentId = video3Id,
        CityId = cityDiyarbakirId,
        TenantId =tenantId
      }
              }
              , autoSave: true
            );

            await _listableContentCategoryRepository.InsertManyAsync(
              new List<ListableContentCategory>()
              {
      new()
      {
        ListableContentId = video3Id,
        CategoryId = categoryEducationId,
        TenantId =tenantId
      },
      new()
      {
        ListableContentId = video3Id,
        CategoryId = categoryComputerId,
         IsPrimary = true,
        TenantId =tenantId
      }
              },
              autoSave: true
            );

            await _listableContentRelationRepository.InsertManyAsync(
              new List<ListableContentRelation>()
              {
      new()
      {
        ListableContentId = video3Id,
        RelatedListableContentId = news1Id,
        TenantId =tenantId
      },
      new()
      {
        ListableContentId = video3Id,
        RelatedListableContentId = news3Id,
        TenantId =tenantId
      },
      new()
      {
        ListableContentId = video3Id,
        RelatedListableContentId = news4Id,
        TenantId =tenantId
      },
      new()
      {
        ListableContentId = video3Id,
        RelatedListableContentId = video2Id,
        TenantId =tenantId
      },
      new()
      {
        ListableContentId = video3Id,
        RelatedListableContentId = video1Id,
        TenantId =tenantId
      }
              },
              autoSave: true
            );

            #endregion

            #region Video Haber 4 (Url)

            await _videoRepository.InsertAsync(
              new Video()
              {
                  Title = "Video Haber 4",
                  Spot = "Video haber 4 içeriği",
                  ImageId = null,
                  TenantId = tenantId,
                  Status = StatusType.Published,
                  PublishTime = DateTime.Now,
                  ListableContentType = ListableContentType.Video,
                  VideoType = VideoType.Link,
                  Url = "www.dogruhaber.com.tr"
              },
              autoSave: true
            );

            var video4Id = (await _videoRepository.GetAsync(x => x.Title == "Video Haber 4")).Id;

            await _listableContentTagRepository.InsertAsync(
              new ListableContentTag()
              {
                  ListableContentId = video4Id,
                  TagId = tagHaberId,
                  TenantId = tenantId
              },
              autoSave: true
            );

            await _listableContentCityRepository.InsertManyAsync(
              new List<ListableContentCity>()
              {
      new()
      {
        ListableContentId = video4Id,
        CityId = cityBatmanId,
        TenantId =tenantId
      },
      new()
      {
        ListableContentId = video4Id,
        CityId = cityMalatyaId,
        TenantId =tenantId
      }
              }
              , autoSave: true
            );

            await _listableContentCategoryRepository.InsertAsync(
              new ListableContentCategory()
              {
                  ListableContentId = video4Id,
                  CategoryId = categorySoftwareEngineeringId,
                  IsPrimary = true,
                  TenantId = tenantId
              },
              autoSave: true
            );

            await _listableContentRelationRepository.InsertManyAsync(
              new List<ListableContentRelation>()
              {
      new()
      {
        ListableContentId = video4Id,
        RelatedListableContentId = news1Id,
        TenantId =tenantId
      },
      new()
      {
        ListableContentId = video4Id,
        RelatedListableContentId = video3Id,
        TenantId =tenantId
      }
              },
              autoSave: true
            );

            #endregion

        }

        #endregion

        #region Galleries
        private async Task SeedGalleryAsync(Guid? tenantId, Guid filesImageId, Guid uploadImageId)
        {
            if (await _galleryRepository.CountAsync() > 0)
                return;

            var tagTatilId = (await _tagRepository.GetAsync(x => x.TagName == "Tatil")).Id;
            var tagTeknolojiId = (await _tagRepository.GetAsync(x => x.TagName == "Teknoloji")).Id;
            var tagHaberId = (await _tagRepository.GetAsync(x => x.TagName == "Haber")).Id;
            var tagEgitimId = (await _tagRepository.GetAsync(x => x.TagName == "Eğitim")).Id;

            var cityBatmanId = (await _cityRepository.GetAsync(x => x.CityName == "Batman")).Id;
            var cityElazigId = (await _cityRepository.GetAsync(x => x.CityName == "Elazığ")).Id;
            var cityMalatyaId = (await _cityRepository.GetAsync(x => x.CityName == "Malatya")).Id;
            var cityIstanbulId = (await _cityRepository.GetAsync(x => x.CityName == "İstanbul")).Id;
            var cityDiyarbakirId = (await _cityRepository.GetAsync(x => x.CityName == "Diyarbakır")).Id;

            var categorySoftwareId = (await _categoryRepository.GetAsync(c => c.CategoryName == "Yazılım")).Id;
            var categoryComputerId = (await _categoryRepository.GetAsync(c => c.CategoryName == "Bilgisayar")).Id;
            var categoryEducationId = (await _categoryRepository.GetAsync(c => c.CategoryName == "Eğitim")).Id;
            var categoryUniversityId = (await _categoryRepository.GetAsync(c => c.CategoryName == "Üniversite")).Id;
            var categoryNewsId = (await _categoryRepository.GetAsync(c => c.CategoryName == "Haber")).Id;

            var categoryCodingId = (await _categoryRepository.GetAsync(c => c.CategoryName == "Kodlama")).Id;
            var categoryEducationSystemId = (await _categoryRepository.GetAsync(c => c.CategoryName == "Eğitim Sistemi")).Id;
            var categoryComputerEngineeringId = (await _categoryRepository.GetAsync(c => c.CategoryName == "Bilgisayar Mühendisi")).Id;
            var categorySoftwareEngineeringId = (await _categoryRepository.GetAsync(c => c.CategoryName == "Yazılım Mühendisi")).Id;

            var news1Id = (await _newsRepository.GetAsync(x => x.Title == "News Haber 1")).Id;
            var news2Id = (await _newsRepository.GetAsync(x => x.Title == "News Haber 2")).Id;
            var news3Id = (await _newsRepository.GetAsync(x => x.Title == "News Haber 3")).Id;
            var news4Id = (await _newsRepository.GetAsync(x => x.Title == "News Haber 4")).Id;

            var video1Id = (await _videoRepository.GetAsync(x => x.Title == "Video Haber 1")).Id;
            var video2Id = (await _videoRepository.GetAsync(x => x.Title == "Video Haber 2")).Id;
            var video3Id = (await _videoRepository.GetAsync(x => x.Title == "Video Haber 3")).Id;
            var video4Id = (await _videoRepository.GetAsync(x => x.Title == "Video Haber 4")).Id;

            #region Gallery Haber 1

            await _galleryRepository.InsertAsync(
              new Gallery()
              {
                  Title = "Gallery Haber 1",
                  Spot = "Gallery haber 1 içeriği",
                  ImageId = filesImageId,
                  TenantId = tenantId,
                  Status = StatusType.Published,
                  PublishTime = DateTime.Now,
                  ListableContentType = ListableContentType.Gallery,
              },
              autoSave: true
            );

            var gallery1Id = (await _galleryRepository.GetAsync(x => x.Title == "Gallery Haber 1")).Id;

            await _galleryImageRepository.InsertManyAsync(
              new List<GalleryImage>()
              {
      new()
      {
        GalleryId = gallery1Id,
        ImageId = filesImageId,
        TenantId = tenantId,
        NewsContent = "Gallery 1 image 1",
        Order = 1,
      },
      new()
      {
        GalleryId = gallery1Id,
        ImageId = uploadImageId,
        TenantId = tenantId,
        NewsContent = "Gallery 1 image 2",
        Order = 2,
      }
              }
            );

            await _listableContentTagRepository.InsertAsync(
              new ListableContentTag()
              {
                  ListableContentId = gallery1Id,
                  TagId = tagTatilId,
                  TenantId = tenantId
              },
              autoSave: true
            );

            await _listableContentCityRepository.InsertManyAsync(
              new List<ListableContentCity>()
              {
      new()
      {
        ListableContentId = gallery1Id,
        CityId = cityBatmanId,
        TenantId = tenantId
      },
      new()
      {
        ListableContentId = gallery1Id,
        CityId = cityElazigId,
        TenantId = tenantId
      }
              }
              , autoSave: true
            );

            await _listableContentCategoryRepository.InsertAsync(
              new ListableContentCategory()
              {
                  ListableContentId = gallery1Id,
                  CategoryId = categorySoftwareId,
                  IsPrimary = true,
                  TenantId = tenantId
              },
              autoSave: true
            );

            await _listableContentRelationRepository.InsertManyAsync(
              new List<ListableContentRelation>()
              {
      new()
      {
        ListableContentId = gallery1Id,
        RelatedListableContentId = news1Id,
        TenantId = tenantId
      },
      new()
      {
        ListableContentId = gallery1Id,
        RelatedListableContentId = video1Id,
        TenantId = tenantId
      }
              },
              autoSave: true
            );

            #endregion

            #region Gallery Haber 2 

            await _galleryRepository.InsertAsync(
              new Gallery()
              {
                  Title = "Gallery Haber 2",
                  Spot = "Gallery haber 2 içeriği",
                  ImageId = uploadImageId,
                  TenantId = tenantId,
                  Status = StatusType.Published,
                  PublishTime = DateTime.Now,
                  ListableContentType = ListableContentType.Gallery
              },
              autoSave: true
            );

            var gallery2Id = (await _galleryRepository.GetAsync(x => x.Title == "Gallery Haber 2")).Id;

            await _galleryImageRepository.InsertManyAsync(
              new List<GalleryImage>()
              {
    new()
    {
      GalleryId = gallery2Id,
      ImageId = uploadImageId,
      TenantId = tenantId,
      NewsContent = "Gallery 2 image 1",
      Order = 1,
    },
    new()
    {
      GalleryId = gallery2Id,
      ImageId = filesImageId,
      TenantId = tenantId,
      NewsContent = "Gallery 2 image 2",
      Order = 2,
    }
              }
            );

            await _listableContentTagRepository.InsertAsync(
              new ListableContentTag()
              {
                  ListableContentId = gallery2Id,
                  TagId = tagTeknolojiId,
                  TenantId = tenantId
              },
              autoSave: true
            );

            await _listableContentCityRepository.InsertManyAsync(
              new List<ListableContentCity>()
              {
    new()
    {
      ListableContentId = gallery2Id,
      CityId = cityMalatyaId,
      TenantId = tenantId
    },
    new()
    {
      ListableContentId = gallery2Id,
      CityId = cityDiyarbakirId,
      TenantId = tenantId
    }
              }
              , autoSave: true
            );

            await _listableContentCategoryRepository.InsertAsync(
              new ListableContentCategory()
              {
                  ListableContentId = gallery2Id,
                  CategoryId = categoryComputerId,
                  IsPrimary = true,
                  TenantId = tenantId
              },
              autoSave: true
            );

            await _listableContentRelationRepository.InsertManyAsync(
              new List<ListableContentRelation>()
              {
    new()
    {
      ListableContentId = gallery2Id,
      RelatedListableContentId = news2Id,
      TenantId = tenantId
    },
    new()
    {
      ListableContentId = gallery2Id,
      RelatedListableContentId = video2Id,
      TenantId = tenantId
    }
              },
              autoSave: true
            );

            #endregion

            #region Gallery Haber 3 

            await _galleryRepository.InsertAsync(
              new Gallery()
              {
                  Title = "Gallery Haber 3",
                  Spot = "Gallery haber 3 içeriği",
                  ImageId = uploadImageId,
                  TenantId = tenantId,
                  Status = StatusType.Published,
                  PublishTime = DateTime.Now,
                  ListableContentType = ListableContentType.Gallery
              },
              autoSave: true
            );

            var gallery3Id = (await _galleryRepository.GetAsync(x => x.Title == "Gallery Haber 3")).Id;

            await _galleryImageRepository.InsertManyAsync(
              new List<GalleryImage>()
              {
    new()
    {
      GalleryId = gallery3Id,
      ImageId = uploadImageId,
      TenantId = tenantId,
      NewsContent = "Gallery 3 image 1",
      Order = 1,
    },
    new()
    {
      GalleryId = gallery3Id,
      ImageId = filesImageId,
      TenantId = tenantId,
      NewsContent = "Gallery 3 image 2",
      Order = 2,
    }
              }
            );

            await _listableContentTagRepository.InsertAsync(
              new ListableContentTag()
              {
                  ListableContentId = gallery3Id,
                  TagId = tagHaberId,
                  TenantId = tenantId
              },
              autoSave: true
            );

            await _listableContentCityRepository.InsertManyAsync(
              new List<ListableContentCity>()
              {
    new()
    {
      ListableContentId = gallery3Id,
      CityId = cityBatmanId,
      TenantId = tenantId
    },
    new()
    {
      ListableContentId = gallery3Id,
      CityId = cityIstanbulId,
      TenantId = tenantId
    }
              }
              , autoSave: true
            );

            await _listableContentCategoryRepository.InsertAsync(
              new ListableContentCategory()
              {
                  ListableContentId = gallery3Id,
                  CategoryId = categoryNewsId,
                  IsPrimary = true,
                  TenantId = tenantId
              },
              autoSave: true
            );

            await _listableContentRelationRepository.InsertManyAsync(
              new List<ListableContentRelation>()
              {
    new()
    {
      ListableContentId = gallery3Id,
      RelatedListableContentId = news3Id,
      TenantId = tenantId
    },
    new()
    {
      ListableContentId = gallery3Id,
      RelatedListableContentId = video3Id,
      TenantId = tenantId
    }
              },
              autoSave: true
            );

            #endregion


        }
        #endregion


    }
}
