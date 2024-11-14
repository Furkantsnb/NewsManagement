using EasyAbp.FileManagement.Files;
using EasyAbp.FileManagement.Options.Containers;
using NewsManagement2.Entities.Categories;
using NewsManagement2.Entities.Cities;
using NewsManagement2.Entities.Galleries;
using NewsManagement2.Entities.ListableContentRelations;
using NewsManagement2.Entities.Newses;
using NewsManagement2.Entities.Tags;
using NewsManagement2.Entities.Videos;
using NewsManagement2.EntityConsts.RoleConsts;
using NewsManagement2.MultiTenancy;
using NewsManagement2.Permissions;
using System;
using System.Collections.Generic;
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
            IRepository<IdentityRole,Guid> roleRepository, IdentityUserManager identityUserManager,IdentityRoleManager identityRoleManager,
            IPermissionManager permissionManager,TenantManager tenantManager,
            ITenantRepository tenantRepository,
            ICurrentTenant currentTenant,
            IFeatureManager featureManager,
            IFeatureChecker featureChecker,FileManager fileManager,
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

        public Task SeedAsync(DataSeedContext context)
        {
            throw new NotImplementedException();
        }

        #region Tenants
        //private async Task SeedTenantAsync()
        //{

        //    if (await _tenantRepository.FindByNameAsync(NewsManagement2Consts.ChildTenanName) == null)
        //    {
        //        var childTenant = await _tenantManager.CreateAsync(NewsManagement2Consts.ChildTenanName);
        //        await _tenantRepository.InsertAsync(childTenant);

        //        using (_currentTenant.Change(childTenant.Id))
        //        {
        //            await _featureManager.SetForTenantAsync(childTenant.Id, MultiTenancyConsts.Gallery, true.ToString());

        //            var filesImageId = NewsManagement2Consts.ChildTenanFilesImageId;
        //            var uploadImageId = NewsManagement2Consts.ChildTenanUploadImageId;


        //        }

        //    }

        //    if (await _tenantRepository.FindByNameAsync(NewsManagement2Consts.YoungTenanName) == null)
        //    {
        //        var youngTenant = await _tenantManager.CreateAsync(NewsManagement2Consts.YoungTenanName);
        //        await _tenantRepository.InsertAsync(youngTenant);

        //        using (_currentTenant.Change(youngTenant.Id))
        //        {
        //            await _featureManager.SetForTenantAsync(youngTenant.Id, MultiTenancyConsts.Video, true.ToString());

        //            var filesImageId = NewsManagement2Consts.YoungTenanFilesImageId;
        //            var uploadImageId = NewsManagement2Consts.YoungTenanUploadImageId;


        //        }

        //    }
        //}

        #endregion

       

    }
}
