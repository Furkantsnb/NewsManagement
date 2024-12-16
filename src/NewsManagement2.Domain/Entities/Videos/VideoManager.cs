
using EasyAbp.FileManagement.Files;
using NewsManagement2.Entities.Categories;
using NewsManagement2.Entities.Cities;
using NewsManagement2.Entities.Galleries;
using NewsManagement2.Entities.ListableContentRelations;
using NewsManagement2.Entities.ListableContents;
using NewsManagement2.Entities.Newses;
using NewsManagement2.Entities.Tags;
using NewsManagement2.EntityConsts.VideoConsts;
using NewsManagement2.EntityDtos.PagedAndSortedDto;
using NewsManagement2.EntityDtos.VideoDtos;
using NewsManagement2.EntityDtos.Videos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.ObjectMapping;

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

        /// <summary>
        /// Yeni bir video oluşturur ve ilişkisel varlıkları ekler.
        /// - Video türüne (Video veya Link) göre özel doğrulamalar yapar.
        /// </summary>
        /// <param name="createVideoDto">Oluşturulacak videoya ait DTO.</param>
        /// <returns>Oluşturulan videonun DTO'su.</returns>
        /// <exception cref="BusinessException">
        /// - Eğer VideoType "Video" ise VideoId boş olamaz.
        /// - Eğer VideoType "Link" ise URL boş olamaz.
        /// </exception>
        public async Task<VideoDto> CreateAsync(CreateVideoDto createVideoDto)
        {
            // 1. Girdilerin doğruluğunu kontrol eder
            var creatingVideo = await CheckCreateInputBaseAsync(createVideoDto);

            // 2. Video türüne göre özel doğrulamalar yapar
            if (creatingVideo.VideoType == VideoType.Video)
            {
                if (creatingVideo.VideoId == null)
                    throw new BusinessException(NewsManagement2DomainErrorCodes.VideoIdRequiredForVideoContent);

                if (creatingVideo.Url != null)
                    throw new BusinessException(NewsManagement2DomainErrorCodes.UrlNotAllowedForVideoContent);

                var images = _fileRepository.GetAsync((Guid)creatingVideo.VideoId).Result;
            }

            if (creatingVideo.VideoType == VideoType.Link)
            {
                if (creatingVideo.Url == null)
                    throw new BusinessException(NewsManagement2DomainErrorCodes.UrlRequiredForLinkContent);

                if (creatingVideo.VideoId != null)
                    throw new BusinessException(NewsManagement2DomainErrorCodes.VideoIdNotAllowedForLinkContent);
            }

            // 3. Video kaydını veri tabanına ekler
            var video = await _genericRepository.InsertAsync(creatingVideo, autoSave: true);

            // 4. İlişkisel varlıkları ekler
            await CreateCrossEntity(createVideoDto, video.Id);

            // 5. DTO'ya dönüştürür ve döndürür
            var videoDto = _objectMapper.Map<Video, VideoDto>(video);
            await GetCrossEntityAsync(videoDto);

            return videoDto;
        }


        /// <summary>
        /// Mevcut bir videoyu günceller ve ilişkisel varlıkları yeniden oluşturur.
        /// </summary>
        /// <param name="id">Güncellenecek videonun ID'si.</param>
        /// <param name="updateVideoDto">Güncelleme işlemine ait DTO.</param>
        /// <returns>Güncellenen videonun DTO'su.</returns>
        /// <exception cref="BusinessException">
        /// - Video türüne göre aynı doğrulama kuralları geçerlidir.
        /// </exception>
        public async Task<VideoDto> UpdateAsync(int id, UpdateVideoDto updateVideoDto)
        {
            var updatingVideo = await CheckUpdateInputBaseAsync(id, updateVideoDto);

            if (updatingVideo.VideoType == VideoType.Video)
            {
                if (updatingVideo.VideoId == null)
                    throw new BusinessException(NewsManagement2DomainErrorCodes.VideoIdRequiredForVideoContent);

                if (updatingVideo.Url != null)
                    throw new BusinessException(NewsManagement2DomainErrorCodes.UrlNotAllowedForVideoContent);

                var images = _fileRepository.GetAsync((Guid)updatingVideo.VideoId).Result;
            }

            if (updatingVideo.VideoType == VideoType.Link)
            {
                if (updatingVideo.Url == null)
                    throw new BusinessException(NewsManagement2DomainErrorCodes.UrlRequiredForLinkContent);

                if (updatingVideo.VideoId != null)
                    throw new BusinessException(NewsManagement2DomainErrorCodes.VideoIdNotAllowedForLinkContent);
            }

            var video = await _genericRepository.UpdateAsync(updatingVideo, autoSave: true);
            await ReCreateCrossEntity(updateVideoDto, video.Id);

            var videoDto = _objectMapper.Map<Video, VideoDto>(video);
            await GetCrossEntityAsync(videoDto);

            return videoDto;
        }


        /// <summary>
        /// Belirtilen filtre ve sayfalama kriterlerine göre video listesini döndürür.
        /// </summary>
        /// <param name="input">Filtreleme ve sayfalama bilgileri.</param>
        /// <returns>Sayfalama destekli video DTO listesi.</returns>
        public async Task<PagedResultDto<VideoDto>> GetListAsync(GetListPagedAndSortedDto input)
        {
            return await GetListFilterBaseAsync(input);
        }

        /// <summary>
        /// Verilen ID'ye sahip videoyu döndürür.
        /// </summary>
        /// <param name="id">Video ID'si.</param>
        /// <returns>Video DTO'su.</returns>
        public async Task<VideoDto> GetByIdAsync(int id)
        {
            var video = await GetByIdBaseAsync(id);
            return video;
        }

        public async Task DeleteAsync(int id)
        {
            await CheckDeleteInputBaseAsync(id);
        }
        public async Task DeleteHardAsync(int id)
        {
            await CheckDeleteHardInputBaseAsync(id);
        }
    }
}
