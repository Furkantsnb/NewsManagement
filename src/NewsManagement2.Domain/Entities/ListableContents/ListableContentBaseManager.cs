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

            if (createDto.Status == StatusType.Deleted)
                throw new BusinessException(NewsManagement2DomainErrorCodes.InvalidStatusTransition);


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

            if (createDto.RelatedListableContentIds != null)
                await CheckListableContentByIdBaseAsync(createDto.RelatedListableContentIds);

            await CheckListableContentCategoryBaseAsync(createDto.ListableContentCategoryDtos, entity.ListableContentType);

            // Yayın tarihi ve durumu kontrol edilir.
            CheckStatusAndDateTimeBaseAsync(createDto.Status, createDto.PublishTime);

            return entity;
        }

        #endregion
        #region Yardımcı Metotlar (Helper Methods)

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

        /// <summary>
        /// İçeriğin durumu (status) ve yayın tarihi ile ilgili kontrolleri gerçekleştirir.
        /// - Durumlara göre tarih değerinin uygunluğunu kontrol eder.
        /// - Geçersiz durumlar veya tarih kombinasyonları için hata fırlatır.
        /// </summary>
        /// <param name="type">İçeriğin durumu (örneğin: Draft, Published, Archived).</param>
        /// <param name="dateTime">İçeriğin yayın tarihi.</param>
        /// <exception cref="BusinessException">
        /// Durum ve tarih kombinasyonu hatalı ise uygun bir hata kodu ile istisna fırlatılır.
        /// </exception>
        protected void CheckStatusAndDateTimeBaseAsync(StatusType type, DateTime? dateTime)
        {
            // Taslak (Draft) durumunda tarih belirtilmişse hata fırlatılır.
            if (type == StatusType.Draft && dateTime.HasValue)
                throw new BusinessException(NewsManagement2DomainErrorCodes.InvalidDraftPublishTime);

            // Arşivlenmiş (Archived) durumunda tarih belirtilmişse hata fırlatılır.
            if (type == StatusType.Archived && dateTime.HasValue)
                throw new BusinessException(NewsManagement2DomainErrorCodes.InvalidDraftPublishTime);

            // Silinmiş (Deleted) durumunda tarih belirtilmişse hata fırlatılır.
            if (type == StatusType.Deleted && dateTime.HasValue)
                throw new BusinessException(NewsManagement2DomainErrorCodes.DeletedStatusCannotBeModified);

            // Yayınlanmış (Published) durumunda tarih belirtilmemişse hata fırlatılır.
            if (type == StatusType.Published && !dateTime.HasValue)
                throw new BusinessException(NewsManagement2DomainErrorCodes.MissingPublishTimeForSelectedStatus);

            // Yayınlanmış (Published) durumunda tarih şu anki zaman değilse hata fırlatılır.
            if (type == StatusType.Published && dateTime.Value.ToString("yyyyMMddHHmm") != DateTime.Now.ToString("yyyyMMddHHmm"))
                throw new BusinessException(NewsManagement2DomainErrorCodes.InvalidPublishedTime);

            // Planlanmış (Scheduled) durumunda tarih belirtilmemişse hata fırlatılır.
            if (type == StatusType.Scheduled && !dateTime.HasValue)
                throw new BusinessException(NewsManagement2DomainErrorCodes.MissingPublishTimeForSelectedStatus);

            // Planlanmış (Scheduled) durumunda tarih geçmiş bir zaman ise hata fırlatılır.
            if (type == StatusType.Scheduled && dateTime.Value <= DateTime.Now)
                throw new BusinessException(NewsManagement2DomainErrorCodes.InvalidScheduledPublishTime);
        }

        /// <summary>
        /// Verilen listelenebilir içerik ID'lerinin doğruluğunu kontrol eder.
        /// - ID'lerin çakışmadığını (duplicate) kontrol eder.
        /// - Her ID'nin galeriler, videolar veya haberler arasında mevcut olup olmadığını doğrular.
        /// </summary>
        /// <param name="RelatedListableContentIds">Kontrol edilecek içerik ID'lerinin listesi.</param>
        /// <exception cref="NotFoundException">
        /// Eğer herhangi bir ID galeriler, videolar veya haberler arasında bulunamazsa bu hata fırlatılır.
        /// </exception>
        protected async Task CheckListableContentByIdBaseAsync(List<int> RelatedListableContentIds)
        {
            // Çakışan (duplicate) içerik ID'lerini kontrol eder.
            CheckDuplicateInputsBase(nameof(RelatedListableContentIds), RelatedListableContentIds);

            // Verilen her içerik ID'sinin mevcut olup olmadığını kontrol eder.
            foreach (var ListableContentId in RelatedListableContentIds)
            {
                var gallery = await _galleryRepository.AnyAsync(x => x.Id == ListableContentId);
                var video = await _videoRepository.AnyAsync(x => x.Id == ListableContentId);
                var news = await _newsRepository.AnyAsync(x => x.Id == ListableContentId);

                // Eğer ID galerilerde, videolarda veya haberlerde bulunmazsa hata fırlatılır.
                if (!gallery && !video && !news)
                    throw new NotFoundException(typeof(ListableContent), ListableContentId.ToString());
            }
        }


        /// <summary>
        /// Listelenebilir içerik için kategori doğrulamalarını gerçekleştirir.
        /// - Kategori ID'lerinin geçerli olup olmadığını kontrol eder.
        /// - Her kategori için üst kategori (parent) varlığını doğrular.
        /// - Kategori türünün (listableContentType) uyumluluğunu kontrol eder.
        /// </summary>
        /// <param name="listableContentCategoryDto">Doğrulanacak kategori DTO'larının listesi.</param>
        /// <param name="type">İçeriğin türü (örneğin: Haber, Video, Galeri).</param>
        /// <exception cref="NotFoundException">
        /// - Kategori ID'lerinden biri bulunamazsa bu hata fırlatılır.
        /// - Kategorinin üst kategorisi eksikse bu hata fırlatılır.
        /// </exception>
        /// <exception cref="BusinessException">
        /// - Kategori türü içeriğin türüyle eşleşmiyorsa hata fırlatılır.
        /// - Birden fazla "birincil kategori" (primary category) seçilmişse hata fırlatılır.
        /// </exception>
        protected async Task CheckListableContentCategoryBaseAsync(List<ListableContentCategoryDto> listableContentCategoryDto, ListableContentType type)
        {
            // Kategori ID'lerini kontrol etmek için bir liste oluşturur.
            var categoryIds = listableContentCategoryDto.Select(x => x.CategoryId).ToList();

            // Çakışan (duplicate) kategori ID'lerini kontrol eder.
            CheckDuplicateInputsBase(nameof(categoryIds), categoryIds);

            // Kategorileri veri tabanından alır.
            var categories = await _categoryRepository.GetListAsync(c => categoryIds.Contains(c.Id));

            // Bulunamayan kategori ID'lerini kontrol eder.
            var isExistCategoyIds = categoryIds.Except(categories.Select(c => c.Id)).ToList();
            if (isExistCategoyIds.Any())
                throw new NotFoundException(typeof(ListableContentCategory), string.Join(", ", isExistCategoyIds));

            // Üst kategorileri kontrol eder.
            var parentCategoryIds = categories.Where(c => c.ParentCategoryId == null).Select(x => x.Id).ToList();
            var missingCategoryIds = categories.Where(c => c.ParentCategoryId != null && !parentCategoryIds.Contains(c.ParentCategoryId.Value))
                .Select(x => x.Id)
                .ToList();

            // Kategori türü uyumluluğunu kontrol eder.
            if (categories.Any(x => x.listableContentType != type))
                throw new BusinessException(NewsManagement2DomainErrorCodes.MustHaveTheSameContentType);

            // Birden fazla birincil kategori varsa hata fırlatır.
            if (listableContentCategoryDto.Count(x => x.IsPrimary) != 1)
                throw new BusinessException(NewsManagement2DomainErrorCodes.ActiveCategoryLimitExceeded)
                    .WithData("0", listableContentCategoryDto.Count(x => x.IsPrimary));

            // Eksik üst kategori bağlantılarını kontrol eder.
            if (missingCategoryIds.Any())
                throw new BusinessException(NewsManagement2DomainErrorCodes.ParentCategoryRequired)
                    .WithData("categoryId", string.Join(", ", missingCategoryIds));
        }
        #endregion
    }
}
