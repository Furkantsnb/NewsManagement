using EasyAbp.FileManagement.Files;
using NewsManagement2.Entities.Categories;
using NewsManagement2.Entities.Cities;
using NewsManagement2.Entities.ListableContentRelations;
using NewsManagement2.Entities.ListableContents;
using NewsManagement2.Entities.Newses;
using NewsManagement2.Entities.Tags;
using NewsManagement2.Entities.Videos;
using NewsManagement2.EntityDtos.GalleryDtos;
using NewsManagement2.EntityDtos.PagedAndSortedDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;

namespace NewsManagement2.Entities.Galleries
{
    /// <summary>
    /// Galeri içeriklerini yönetmek için kullanılan bir sınıf.
    /// - Galeri içeriklerinin oluşturulması, güncellenmesi, silinmesi, listelenmesi ve alınması işlemlerini gerçekleştirir.
    /// - Galeriye ait ilişkisel varlıklar (etiketler, şehirler, kategoriler ve resimler) üzerinde işlemler yapar.
    /// </summary>
    public class GalleryManager : ListableContentBaseManager<Gallery, GalleryDto, GetListPagedAndSortedDto, CreateGalleryDto, UpdateGalleryDto>
    {
        private readonly IObjectMapper _objectMapper;
        private readonly ITagRepository _tagRepository;
        private readonly IFileRepository _fileRepository;
        private readonly ICityRepository _cityRepository;
        private readonly INewsRepository _newsRepository;
        private readonly IVideoRepository _videoRepository;
        private readonly IGalleryRepository _galleryRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IRepository<GalleryImage> _galleryImageRepository;
        private readonly IListableContentGenericRepository<Gallery> _genericRepository;
        private readonly IListableContentTagRepository _listableContentTagRepository;
        private readonly IListableContentCityRepository _listableContentCityRepository;
        private readonly IListableContentCategoryRepository _listableContentCategoryRepository;
        private readonly IListableContentRelationRepository _listableContentRelationRepository;

        public GalleryManager(
            IObjectMapper objectMapper,
            ITagRepository tagRepository,
            IFileRepository fileRepository,
            ICityRepository cityRepository,
            INewsRepository newsRepository,
            IVideoRepository videoRepository,
            IGalleryRepository galleryRepository,
            ICategoryRepository categoryRepository,
            IRepository<GalleryImage> galleryImageRepository,
            IListableContentGenericRepository<Gallery> genericRepository,
            IListableContentTagRepository listableContentTagRepository,
            IListableContentCityRepository listableContentCityRepository,
            IListableContentCategoryRepository listableContentCategoryRepository,
            IListableContentRelationRepository listableContentRelationRepository
        ) : base(
            objectMapper, tagRepository, cityRepository, newsRepository,
            videoRepository, galleryRepository, categoryRepository, genericRepository,
            listableContentTagRepository, listableContentCityRepository, listableContentCategoryRepository,
            listableContentRelationRepository, fileRepository
        )
        {
            _objectMapper = objectMapper;
            _tagRepository = tagRepository;
            _fileRepository = fileRepository;
            _cityRepository = cityRepository;
            _newsRepository = newsRepository;
            _videoRepository = videoRepository;
            _galleryRepository = galleryRepository;
            _genericRepository = genericRepository;
            _categoryRepository = categoryRepository;
            _galleryImageRepository = galleryImageRepository;
            _listableContentTagRepository = listableContentTagRepository;
            _listableContentCityRepository = listableContentCityRepository;
            _listableContentCategoryRepository = listableContentCategoryRepository;
            _listableContentRelationRepository = listableContentRelationRepository;
        }

    }
}
