using AutoMapper.Internal.Mappers;
using EasyAbp.FileManagement.Files;
using NewsManagement2.Entities.Categories;
using NewsManagement2.Entities.Cities;
using NewsManagement2.Entities.Galleries;
using NewsManagement2.Entities.ListableContentRelations;
using NewsManagement2.Entities.ListableContents;
using NewsManagement2.Entities.Newses;
using NewsManagement2.Entities.Tags;
using NewsManagement2.EntityDtos.PagedAndSortedDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsManagement2.Entities.Videos
{
    /// <summary>
    /// Video içeriklerini yönetmek için kullanılan sınıf.
    /// - Videoların oluşturulması, güncellenmesi, silinmesi, listelenmesi ve alınması gibi işlemleri gerçekleştirir.
    /// - Video türüne (Video veya Link) göre özel doğrulama kuralları içerir.
    /// - Videoya bağlı diğer ilişkisel varlıkların yönetimini sağlar (etiketler, şehirler, kategoriler vb.).
    /// </summary>
    public class VideoManager : ListableContentBaseManager<Video, VideoDto, GetListPagedAndSortedDto, CreateVideoDto, UpdateVideoDto>
    {
        private readonly IObjectMapper _objectMapper;
        private readonly ITagRepository _tagRepository;
        private readonly IFileRepository _fileRepository;
        private readonly ICityRepository _cityRepository;
        private readonly INewsRepository _newsRepository;
        private readonly IVideoRepository _videoRepository;
        private readonly IGalleryRepository _galleryRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IListableContentGenericRepository<Video> _genericRepository;
        private readonly IListableContentTagRepository _listableContentTagRepository;
        private readonly IListableContentCityRepository _listableContentCityRepository;
        private readonly IListableContentCategoryRepository _listableContentCategoryRepository;
        private readonly IListableContentRelationRepository _listableContentRelationRepository;

        public VideoManager(
          IObjectMapper objectMapper,
          ITagRepository tagRepository,
          ICityRepository cityRepository,
          INewsRepository newsRepository,
          IFileRepository fileRepository,
          IVideoRepository videoRepository,
          IGalleryRepository galleryRepository,
          ICategoryRepository categoryRepository,
          IListableContentGenericRepository<Video> genericRepository,
          IListableContentTagRepository listableContentTagRepository,
          IListableContentCityRepository listableContentCityRepository,
          IListableContentCategoryRepository listableContentCategoryRepository,
          IListableContentRelationRepository listableContentRelationRepository
          ) : base(objectMapper, tagRepository, cityRepository, newsRepository,
            videoRepository, galleryRepository, categoryRepository, genericRepository, listableContentTagRepository,
            listableContentCityRepository, listableContentCategoryRepository, listableContentRelationRepository, fileRepository
              )
        {
            _objectMapper = objectMapper;
            _tagRepository = tagRepository;
            _cityRepository = cityRepository;
            _newsRepository = newsRepository;
            _fileRepository = fileRepository;
            _videoRepository = videoRepository;
            _galleryRepository = galleryRepository;
            _genericRepository = genericRepository;
            _categoryRepository = categoryRepository;
            _listableContentTagRepository = listableContentTagRepository;
            _listableContentCityRepository = listableContentCityRepository;
            _listableContentCategoryRepository = listableContentCategoryRepository;
            _listableContentRelationRepository = listableContentRelationRepository;
        }
    }
}
