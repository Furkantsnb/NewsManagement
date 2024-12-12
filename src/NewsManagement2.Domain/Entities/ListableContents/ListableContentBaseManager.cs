using EasyAbp.FileManagement.Files;
using NewsManagement2.Entities.Categories;
using NewsManagement2.Entities.Cities;
using NewsManagement2.Entities.Galleries;
using NewsManagement2.Entities.ListableContentRelations;
using NewsManagement2.Entities.Newses;
using NewsManagement2.Entities.Tags;
using NewsManagement2.Entities.Videos;
using NewsManagement2.EntityDtos.ListableContentDtos;
using NewsManagement2.EntityDtos.PagedAndSortedDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.ObjectMapping;

namespace NewsManagement2.Entities.ListableContents
{
    /// <summary>
    /// Listelenebilir içerikleri (örneğin haber, video, galeri) yönetmek için temel bir sınıf.
    /// Bu sınıf, içeriklerin oluşturulması, güncellenmesi, silinmesi ve doğrulanması gibi ortak işlemleri içerir.
    /// </summary>
    /// <typeparam name="TEntity">Listelenebilir içerik varlığı (örneğin Haber, Video).</typeparam>
    /// <typeparam name="TEntityDto">Listelenebilir içeriğin DTO temsili.</typeparam>
    /// <typeparam name="TPagedDto">Listelenebilir içerik için sayfalama DTO'su.</typeparam>
    /// <typeparam name="TEntityCreateDto">Yeni içerik oluşturma için kullanılan DTO.</typeparam>
    /// <typeparam name="TEntityUpdateDto">Mevcut içeriği güncellemek için kullanılan DTO.</typeparam>
    public abstract class ListableContentBaseManager<TEntity, TEntityDto, TPagedDto, TEntityCreateDto, TEntityUpdateDto> : DomainService
        where TEntity : ListableContent, new()
        where TEntityDto : ListableContentDto
        where TEntityCreateDto : CreateListableContentDto
        where TEntityUpdateDto : UpdateListableContentDto
        where TPagedDto : GetListPagedAndSortedDto
    {
        #region Bağımlılıklar (Dependencies)

        private readonly IObjectMapper _objectMapper;
        private readonly ITagRepository _tagRepository;
        private readonly ICityRepository _cityRepository;
        private readonly INewsRepository _newsRepository;
        private readonly IVideoRepository _videoRepository;
        private readonly IGalleryRepository _galleryRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IListableContentGenericRepository<TEntity> _genericRepository;
        private readonly IListableContentTagRepository _listableContentTagRepository;
        private readonly IListableContentCityRepository _listableContentCityRepository;
        private readonly IListableContentCategoryRepository _listableContentCategoryRepository;
        private readonly IListableContentRelationRepository _listableContentRelationRepository;
        private readonly IFileRepository _fileRepository;

        /// <summary>
        /// ListableContentBaseManager sınıfını gerekli bağımlılıklar ile başlatır.
        /// </summary>
        protected ListableContentBaseManager(
            IObjectMapper objectMapper,
            ITagRepository tagRepository,
            ICityRepository cityRepository,
            INewsRepository newsRepository,
            IVideoRepository videoRepository,
            IGalleryRepository galleryRepository,
            ICategoryRepository categoryRepository,
            IListableContentGenericRepository<TEntity> genericRepository,
            IListableContentTagRepository listableContentTagRepository,
            IListableContentCityRepository listableContentCityRepository,
            IListableContentCategoryRepository listableContentCategoryRepository,
            IListableContentRelationRepository listableContentRelationRepository,
            IFileRepository fileRepository
        )
        {
            _objectMapper = objectMapper;
            _tagRepository = tagRepository;
            _cityRepository = cityRepository;
            _newsRepository = newsRepository;
            _videoRepository = videoRepository;
            _galleryRepository = galleryRepository;
            _genericRepository = genericRepository;
            _categoryRepository = categoryRepository;
            _listableContentTagRepository = listableContentTagRepository;
            _listableContentCityRepository = listableContentCityRepository;
            _listableContentCategoryRepository = listableContentCategoryRepository;
            _listableContentRelationRepository = listableContentRelationRepository;
            _fileRepository = fileRepository;
        }

        #endregion
    }
}
