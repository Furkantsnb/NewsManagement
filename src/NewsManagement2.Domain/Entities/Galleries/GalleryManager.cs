﻿using EasyAbp.FileManagement.Files;
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
using Volo.Abp;
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
        /// <summary>
        /// Yeni bir galeri oluşturur ve ilişkisel varlıkları (resimler, etiketler, şehirler ve kategoriler) ekler.
        /// </summary>
        /// <param name="createGalleryDto">Oluşturulacak galeriye ait DTO.</param>
        /// <returns>Oluşturulan galerinin DTO'sunu döndürür.</returns>
        public async Task<GalleryDto> CreateAsync(CreateGalleryDto createGalleryDto)
        {
            // 1. Girdilerin doğruluğunu kontrol eder
            var creatingGallery = await CheckCreateInputBaseAsync(createGalleryDto);

            // 2. Resim sırasını kontrol eder
            var orders = createGalleryDto.GalleryImages.Select(x => x.Order).ToList();
            CheckOrderInput(orders);

            // 3. Resimlerin varlığını kontrol eder
            foreach (var galleryImage in creatingGallery.GalleryImages)
            {
                var images = _fileRepository.GetAsync(galleryImage.ImageId).Result;
            }

            // 4. Galeriyi veri tabanına ekler
            var gallery = await _genericRepository.InsertAsync(creatingGallery, autoSave: true);

            // 5. İlişkisel varlıkları oluşturur
            await CreateCrossEntity(createGalleryDto, gallery.Id);

            // 6. DTO'ya dönüştürür ve ilişkisel varlıkları ekler
            var galleryDto = _objectMapper.Map<Gallery, GalleryDto>(gallery);
            await GetCrossEntityAsync(galleryDto);

            return galleryDto;
        }

        /// <summary>
        /// Verilen bir ID'ye sahip galeriyi getirir ve ilişkisel resimlerini ekler.
        /// </summary>
        /// <param name="id">Galeri ID'si.</param>
        /// <returns>Galeri DTO'su.</returns>
        public async Task<GalleryDto> GetByIdAsync(int id)
        {
            var gallery = await GetByIdBaseAsync(id);

            var galleryImage = await _galleryImageRepository.GetListAsync(x => x.GalleryId == gallery.Id);
            gallery.GalleryImages = _objectMapper.Map<List<GalleryImage>, List<GalleryImageDto>>(galleryImage);

            return gallery;
        }


        /// <summary>
        /// Belirtilen galeriyi siler.
        /// </summary>
        /// <param name="id">Silinecek galerinin ID'si.</param>
        public async Task DeleteAsync(int id)
        {
            await CheckDeleteInputBaseAsync(id);
        }

        /// <summary>
        /// Belirtilen galeriyi veri tabanından tamamen siler.
        /// </summary>
        /// <param name="id">Silinecek galerinin ID'si.</param>
        public async Task DeleteHardAsync(int id)
        {
            await CheckDeleteHardInputBaseAsync(id);
        }

        public void CheckOrderInput(List<int> input)
        {
            input.Sort();

            for (int i = 0; i < input.Count; i++)
            {
                if (input[i] != i + 1)
                {
                    throw new BusinessException(NewsManagement2DomainErrorCodes.SortingParameterInvalid)
                      .WithData("0", input[i].ToString());
                }
            }

        }
    }
}
