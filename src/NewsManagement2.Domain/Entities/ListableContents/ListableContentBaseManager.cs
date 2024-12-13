using EasyAbp.FileManagement.Files;
using NewsManagement2.Entities.Categories;
using NewsManagement2.Entities.Cities;
using NewsManagement2.Entities.Exceptions;
using NewsManagement2.Entities.Galleries;
using NewsManagement2.Entities.ListableContentRelations;
using NewsManagement2.Entities.Newses;
using NewsManagement2.Entities.Tags;
using NewsManagement2.Entities.Videos;
using NewsManagement2.EntityConsts.ListableContentConsts;
using NewsManagement2.EntityDtos.ListableContentDtos;
using NewsManagement2.EntityDtos.PagedAndSortedDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp;
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

        #region İçerik Oluşturma (Create Operations)

        /// <summary>
        /// Yeni bir içerik oluşturulmadan önce doğrulama ve hazırlık işlemlerini yapar.
        /// </summary>
        /// <param name="createDto">Yeni içerik için giriş DTO'su.</param>
        /// <returns>Hazırlanmış içerik varlığı.</returns>
        protected async Task<TEntity> CheckCreateInputBaseAsync(TEntityCreateDto createDto)
        {
            // Başlık zaten var mı kontrol edilir.
            var isExist = await _genericRepository.AnyAsync(x => x.Title == createDto.Title);
            if (isExist)
                throw new AlreadyExistException(typeof(TEntity), createDto.Title);


            // Eğer bir resim ID'si sağlanmışsa, geçerli bir dosya ID olup olmadığı kontrol edilir.
            if (createDto.ImageId != null)
                await _fileRepository.GetAsync((Guid)createDto.ImageId);

            // DTO'dan varlık nesnesine dönüştürme işlemi.
            var entity = _objectMapper.Map<TEntityCreateDto, TEntity>(createDto);

            // İçerik türü, varlık tipi adına göre atanır.
            string entityTypeName = typeof(TEntity).Name;
            entity.ListableContentType = (ListableContentType)Enum.Parse(typeof(ListableContentType), entityTypeName);

            // İlgili varlıkların kontrolü yapılır.
            await CheckTagByIdBaseAsync(createDto.TagIds);
            await CheckCityByIdBaseAsync(createDto.CityIds);



            return entity;
        }

        #endregion

        /// <summary>
        /// Verilen etiket ID'lerinin (tagIds) geçerliliğini kontrol eder.
        /// - Çakışan (duplicate) ID'ler var mı kontrol edilir.
        /// - Her ID'nin veri tabanında mevcut olup olmadığı doğrulanır.
        /// </summary>
        /// <param name="tagIds">Doğrulanacak etiket ID'lerinin listesi.</param>
        /// <exception cref="NotFoundException">
        /// Eğer herhangi bir etiket ID veri tabanında bulunmazsa bu hata fırlatılır.
        /// </exception>
        protected async Task CheckTagByIdBaseAsync(List<int> tagIds)
        {
            // Çakışan (duplicate) girişleri kontrol eder.
            CheckDuplicateInputsBase(nameof(tagIds), tagIds);

            // Verilen her etiket ID'nin mevcut olup olmadığını kontrol eder.
            foreach (var tagId in tagIds)
            {
                var existTag = await _tagRepository.AnyAsync(t => t.Id == tagId);
                if (!existTag)
                    throw new NotFoundException(typeof(Tag), tagId.ToString()); // Etiket bulunamazsa hata fırlatır.
            }
        }

        /// <summary>
        /// Verilen şehir ID'lerinin (cityIds) geçerliliğini kontrol eder.
        /// - Çakışan (duplicate) ID'ler var mı kontrol edilir.
        /// - Her ID'nin veri tabanında mevcut olup olmadığı doğrulanır.
        /// </summary>
        /// <param name="cityIds">Doğrulanacak şehir ID'lerinin listesi.</param>
        /// <exception cref="NotFoundException">
        /// Eğer herhangi bir şehir ID veri tabanında bulunmazsa bu hata fırlatılır.
        /// </exception>
        protected async Task CheckCityByIdBaseAsync(List<int> cityIds)
        {
            // Çakışan (duplicate) girişleri kontrol eder.
            CheckDuplicateInputsBase(nameof(cityIds), cityIds);

            // Verilen her şehir ID'nin mevcut olup olmadığını kontrol eder.
            foreach (var cityId in cityIds)
            {
                var existCity = await _cityRepository.AnyAsync(c => c.Id == cityId);
                if (!existCity)
                    throw new NotFoundException(typeof(City), cityId.ToString()); // Şehir bulunamazsa hata fırlatır.
            }
        }

        /// <summary>
        /// Verilen ID listesinde tekrarlayan (duplicate) değerleri kontrol eder.
        /// Eğer tekrarlayan değerler bulunursa bir hata fırlatır.
        /// </summary>
        /// <param name="inputName">Kontrol edilen listenin adı (örneğin: "tagIds" veya "cityIds").</param>
        /// <param name="inputId">Kontrol edilecek ID listesidir.</param>
        /// <exception cref="BusinessException">
        /// Eğer liste içinde tekrarlayan değerler bulunursa, bir iş kuralı istisnası (BusinessException) fırlatılır.
        /// </exception>
        protected void CheckDuplicateInputsBase(string inputName, List<int> inputId)
        {
            var duplicates = inputId.GroupBy(x => x) // Listeyi gruplar (her benzersiz ID için bir grup oluşturur).
                .Where(u => u.Count() > 1)          // Birden fazla kez görünen (duplicate) ID'leri seçer.
                .Select(u => u.Key)                 // Bu grupların anahtarlarını (tekrarlayan ID'leri) alır.
                .ToList();                          // Sonuçları bir listeye dönüştürür.

            if (duplicates.Count > 0) // Eğer duplicate değerler varsa
            {
                var duplicateUnits = string.Join(", ", duplicates); // Duplicate ID'leri birleştirip, virgülle ayırır.
                throw new BusinessException(NewsManagement2DomainErrorCodes.RepeatedDataError) // Hata fırlatır.
                    .WithData("0", inputName) // Hata detayında input ismini belirtir.
                    .WithData("1", duplicateUnits); // Hata detayında tekrarlayan ID'leri belirtir.
            }
        }

    }
}
