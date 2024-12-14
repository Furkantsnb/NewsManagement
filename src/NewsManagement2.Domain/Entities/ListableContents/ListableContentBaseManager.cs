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
using Volo.Abp.Application.Dtos;
using NewsManagement2.EntityDtos.CategoryDtos;
using NewsManagement2.EntityDtos.CityDtos;
using NewsManagement2.EntityDtos.TagDtos;
using Volo.Abp.Domain.Entities;

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
        #region İçerik Güncelleme(Update Operations)
        /// <summary>
        /// Mevcut bir içeriği güncellemeden önce gerekli doğrulama ve hazırlıkları yapar.
        /// - Verilen ID'nin mevcut olup olmadığını kontrol eder.
        /// - Başlık tekrarı gibi veri tutarlılığı problemlerini önler.
        /// - İlgili etiketler, şehirler ve ilişkili içerikler gibi bağımlılıkların doğruluğunu kontrol eder.
        /// - Güncellenen varlığı (entity) geri döndürür.
        /// </summary>
        /// <param name="id">Güncellenecek içeriğin ID'si.</param>
        /// <param name="updateDto">Güncelleme için kullanılan DTO.</param>
        /// <returns>Güncellenmiş varlığı (entity) geri döndürür.</returns>
        /// <exception cref="NotFoundException">
        /// Verilen ID'ye sahip bir içerik bulunamazsa hata fırlatılır.
        /// </exception>
        /// <exception cref="AlreadyExistException">
        /// Aynı başlığa sahip başka bir içerik varsa hata fırlatılır.
        /// </exception>
        /// <exception cref="BusinessException">
        /// Geçersiz durumlar, çakışmalar veya bağımlılıklarda eksiklik tespit edilirse hata fırlatılır.
        /// </exception>
        protected async Task<TEntity> CheckUpdateInputBaseAsync(int id, TEntityUpdateDto updateDto)
        {
            // 1. Varlığın mevcut olup olmadığını kontrol et
            var isExistId = await _genericRepository.AnyAsync(x => x.Id == id);
            if (!isExistId)
            {
                // Eğer içerik bulunamazsa NotFoundException fırlat
                throw new NotFoundException(typeof(TEntity), id.ToString());
            }

            // 2. Başlık tekrarı olup olmadığını kontrol et
            var isExist = await _genericRepository.AnyAsync(x => x.Title == updateDto.Title && x.Id != id);
            if (isExist)
            {
                // Eğer aynı başlığa sahip başka bir içerik varsa AlreadyExistException fırlat
                throw new AlreadyExistException(typeof(TEntity), updateDto.Title);
            }

            // 3. DTO'yu mevcut varlığa (entity) dönüştür
            var entity = _objectMapper.Map(updateDto, await _genericRepository.GetAsync(id));

            // 4. Etiketlerin geçerliliğini kontrol et
            await CheckTagByIdBaseAsync(updateDto.TagIds);

            // 5. Şehirlerin geçerliliğini kontrol et
            await CheckCityByIdBaseAsync(updateDto.CityIds);

            // 6. İlişkili içeriklerin doğruluğunu kontrol et
            if (updateDto.RelatedListableContentIds != null)
            {
                // Kendine referans veren ilişkili içerikleri kontrol et
                var listableContentSelfReference = updateDto.RelatedListableContentIds.Any(x => x == id);
                if (listableContentSelfReference)
                {
                    // Kendine referans varsa BusinessException fırlat
                    throw new BusinessException(NewsManagement2DomainErrorCodes.SelfReferenceError);
                }

                // İlişkili içeriklerin mevcut olup olmadığını kontrol et
                await CheckListableContentByIdBaseAsync(updateDto.RelatedListableContentIds);
            }

            // 7. Kategorilerin doğruluğunu kontrol et
            await CheckListableContentCategoryBaseAsync(updateDto.ListableContentCategoryDtos, entity.ListableContentType);

            // 8. Durum ve tarih kombinasyonlarını kontrol et
            CheckStatusAndDateTimeBaseAsync(updateDto.Status, updateDto.PublishTime);

            // 9. Silinmiş durum kontrolü (Deleted)
            if (entity.Status == StatusType.Deleted)
            {
                // Eğer içerik "Deleted" durumunda ise silindi olarak işaretle
                entity.IsDeleted = true;
            }

            // 10. Güncellenmiş varlığı geri döndür
            return entity;
        }

        #endregion

        #region İçerik Listeleme( GetList Operations)
        /// <summary>
        /// Filtreleme, sıralama ve sayfalama destekli bir listeleme metodu.
        /// - Toplam içerik sayısını hesaplar.
        /// - Belirtilen filtre kriterlerini uygular.
        /// - İçeriklerin sıralama, sayfalama ve DTO'ya dönüştürülmesi işlemlerini yapar.
        /// </summary>
        /// <param name="input">Sayfalama, sıralama ve filtreleme için kullanılan DTO.</param>
        /// <returns>Toplam içerik sayısı ve belirtilen kriterlere uygun içeriklerin DTO listesini döndürür.</returns>
        /// <exception cref="NotFoundException">
        /// Eğer filtre sonucunda hiçbir içerik bulunamazsa hata fırlatılır.
        /// </exception>
        /// <exception cref="BusinessException">
        /// Eğer "SkipCount" toplam içerik sayısını aşarsa hata fırlatılır.
        /// </exception>
        protected async Task<PagedResultDto<TEntityDto>> GetListFilterBaseAsync(TPagedDto input)
        {
            // 1. Toplam içerik sayısını hesapla
            var totalCount = input.Filter == null
                ? await _genericRepository.CountAsync() // Filtre uygulanmamışsa toplam sayıyı al
                : await _genericRepository.CountAsync(c => c.Title.Contains(input.Filter)); // Filtre uygulanmışsa kriterlere uyan sayıyı al

            // 2. Hiç içerik bulunamazsa NotFoundException fırlat
            if (totalCount == 0)
                throw new NotFoundException(typeof(TEntity), input.Filter ?? string.Empty);

            // 3. SkipCount toplam içerik sayısını aşarsa BusinessException fırlat
            if (input.SkipCount >= totalCount)
                throw new BusinessException(NewsManagement2DomainErrorCodes.InvalidFilterCriteria);

            // 4. Varsayılan sıralama kriterini belirle
            if (input.Sorting.IsNullOrWhiteSpace())
                input.Sorting = nameof(ListableContent.Title); // Varsayılan sıralama başlık alanına göre yapılır

            // 5. Belirtilen kriterlere uygun içerik listesini getir
            var entityList = await _genericRepository.GetListAsync(input.SkipCount, input.MaxResultCount, input.Sorting, input.Filter);

            // 6. İçerikleri DTO'ya dönüştür
            var entityDtoList = _objectMapper.Map<List<TEntity>, List<TEntityDto>>(entityList);

            // 7. Her içerik için ilişkili varlıkları getir
            foreach (var entityDto in entityDtoList)
            {
                await GetCrossEntityAsync(entityDto);
            }

            // 8. Toplam içerik sayısı ve DTO listesini döndür
            return new PagedResultDto<TEntityDto>(totalCount, entityDtoList);
        }

        #endregion
        protected async Task CheckDeleteInputBaseAsync(int id)
        {
            var isExist = await _genericRepository.AnyAsync(t => t.Id == id);
            if (!isExist)
                throw new EntityNotFoundException(typeof(TEntity), id);
        }
        #region Listeleme Yardımcı Metotlar
        /// <summary>
        /// Listelenebilir bir içerik için ilgili tüm ilişkisel varlıkları (etiketler, şehirler, kategoriler ve ilişkili içerikler) oluşturur.
        /// </summary>
        /// <param name="createDto">Oluşturma işlemine ait DTO.</param>
        /// <param name="listableContentId">Oluşturulan içeriğin ID'si.</param>
        protected async Task CreateCrossEntity(TEntityCreateDto createDto, int listableContentId)
        {
            await CreateListableContentTagBaseAsync(createDto.TagIds, listableContentId);
            await CreateListableContentCityBaseAsync(createDto.CityIds, listableContentId);
            await CreateListableContentCategoryBaseAsync(createDto.ListableContentCategoryDtos, listableContentId);

            if (createDto.RelatedListableContentIds != null)
                await CreateListableContentRelationBaseAsync(createDto.RelatedListableContentIds, listableContentId);
        }

        /// <summary>
        /// Güncellenen bir içerik için tüm ilişkisel varlıkları siler ve yeniden oluşturur.
        /// </summary>
        /// <param name="updateDto">Güncelleme işlemine ait DTO.</param>
        /// <param name="listableContentId">Güncellenen içeriğin ID'si.</param>
        protected async Task ReCreateCrossEntity(TEntityUpdateDto updateDto, int listableContentId)
        {
            await DeleteAllCrossEntitiesByListableContentId(listableContentId);

            await CreateListableContentTagBaseAsync(updateDto.TagIds, listableContentId);
            await CreateListableContentCityBaseAsync(updateDto.CityIds, listableContentId);
            await CreateListableContentCategoryBaseAsync(updateDto.ListableContentCategoryDtos, listableContentId);

            if (updateDto.RelatedListableContentIds != null)
                await CreateListableContentRelationBaseAsync(updateDto.RelatedListableContentIds, listableContentId);
        }


        /// <summary>
        /// Verilen bir içerik için ilişkisel varlıkları (etiketler, şehirler, kategoriler ve ilişkili içerikler) alır ve DTO'ya dönüştürür.
        /// </summary>
        /// <param name="entityDto">İlgili içeriğin DTO'su.</param>
        protected async Task GetCrossEntityAsync(TEntityDto entityDto)
        {
            var tags = await _listableContentTagRepository.GetCrossListAsync(entityDto.Id);
            var cities = await _listableContentCityRepository.GetCrossListAsync(entityDto.Id);
            var relations = await _listableContentRelationRepository.GetCrossListAsync(entityDto.Id);
            var categiries = await _listableContentCategoryRepository.GetCrossListAsync(entityDto.Id);

            var returnTagDto = _objectMapper.Map<List<ListableContentTag>, List<ReturnTagDto>>(tags);
            var returnCityDto = _objectMapper.Map<List<ListableContentCity>, List<ReturnCityDto>>(cities);
            var returnCategoryDto = _objectMapper.Map<List<ListableContentCategory>, List<ReturnCategoryDto>>(categiries);
            var returnRelationDto = _objectMapper.Map<List<ListableContentRelation>, List<ReturnListableContentRelationDto>>(relations);

            entityDto.Tags = returnTagDto;
            entityDto.Cities = returnCityDto;
            entityDto.Categories = returnCategoryDto;
            entityDto.ListableContentRelations = returnRelationDto;
        }



        /// <summary>
        /// Verilen bir içeriğe ait tüm ilişkisel varlıkları (etiketler, şehirler, kategoriler ve ilişkili içerikler) veri tabanından siler.
        /// </summary>
        /// <param name="listableContentId">İlgili içeriğin ID'si.</param>
        protected async Task DeleteAllCrossEntitiesByListableContentId(int listableContentId)
        {
            await _listableContentCategoryRepository.DeleteAsync(x => x.ListableContentId == listableContentId);
            await _listableContentCityRepository.DeleteAsync(x => x.ListableContentId == listableContentId);
            await _listableContentTagRepository.DeleteAsync(x => x.ListableContentId == listableContentId);
            await _listableContentRelationRepository.DeleteAsync(x => x.ListableContentId == listableContentId);
        }


        /// <summary>
        /// Verilen etiket ID'leri için ilişkisel etiket varlıklarını oluşturur.
        /// </summary>
        /// <param name="tagIds">Etiket ID'lerinin listesi.</param>
        /// <param name="listableContentId">İlgili içeriğin ID'si.</param>
        protected async Task CreateListableContentTagBaseAsync(List<int> tagIds, int listableContentId)
        {
            List<ListableContentTag> listableContentTags = new();
            foreach (var tagId in tagIds)
            {
                var tag = new ListableContentTag { TagId = tagId, ListableContentId = listableContentId };
                listableContentTags.Add(tag);
            }

            await _listableContentTagRepository.InsertManyAsync(listableContentTags, autoSave: true);
        }


        /// <summary>
        /// Verilen şehir ID'leri için ilişkisel şehir varlıklarını oluşturur.
        /// </summary>
        /// <param name="cityIds">Şehir ID'lerinin listesi.</param>
        /// <param name="listableContentId">İlgili içeriğin ID'si.</param>
        protected async Task CreateListableContentCityBaseAsync(List<int> cityIds, int listableContentId)
        {
            List<ListableContentCity> listableContentCitis = new();
            foreach (var cityId in cityIds)
            {
                var city = new ListableContentCity { CityId = cityId, ListableContentId = listableContentId };
                listableContentCitis.Add(city);
            }

            await _listableContentCityRepository.InsertManyAsync(listableContentCitis, autoSave: true);
        }



        /// <summary>
        /// Verilen ilişkili içerik ID'leri için ilişkisel içerik varlıklarını oluşturur.
        /// </summary>
        /// <param name="RelatedListableContentIds">İlişkili içerik ID'lerinin listesi.</param>
        /// <param name="listableContentId">İlgili içeriğin ID'si.</param>
        protected async Task CreateListableContentRelationBaseAsync(List<int> RelatedListableContentIds, int listableContentId)
        {
            List<ListableContentRelation> listableContentRelations = new();
            foreach (var RelatedId in RelatedListableContentIds)
            {
                var listableContentRelation = new ListableContentRelation
                {
                    ListableContentId = listableContentId,
                    RelatedListableContentId = RelatedId
                };

                listableContentRelations.Add(listableContentRelation);
            }

            await _listableContentRelationRepository.InsertManyAsync(listableContentRelations, autoSave: true);
        }

        /// <summary>
        /// Verilen kategori DTO'ları için ilişkisel kategori varlıklarını oluşturur.
        /// </summary>
        /// <param name="listableContentCategoryDto">Kategori DTO'larının listesi.</param>
        /// <param name="listableContentId">İlgili içeriğin ID'si.</param>
        protected async Task CreateListableContentCategoryBaseAsync(List<ListableContentCategoryDto> listableContentCategoryDto, int listableContentId)
        {
            List<ListableContentCategory> listableContentCategories = new();
            foreach (var item in listableContentCategoryDto)
            {
                var category = new ListableContentCategory { ListableContentId = listableContentId, CategoryId = item.CategoryId, IsPrimary = item.IsPrimary };
                listableContentCategories.Add(category);
            }

            await _listableContentCategoryRepository.InsertManyAsync(listableContentCategories, autoSave: true);
        }

        /// <summary>
        /// Mevcut ilişkili içerik varlıklarını siler ve yenilerini oluşturur.
        /// </summary>
        /// <param name="RelatedListableContentIds">Yeni ilişkili içerik ID'lerinin listesi.</param>
        /// <param name="listableContentId">İlgili içeriğin ID'si.</param>
        protected async Task ReCreateListableContentRelationBaseAsync(List<int> RelatedListableContentIds, int listableContentId)
        {
            var isExist = await _listableContentRelationRepository.GetListAsync(x => x.ListableContentId == listableContentId);

            if (isExist.Count() != 0)
                await _listableContentRelationRepository.DeleteManyAsync(isExist, autoSave: true);

            await CreateListableContentRelationBaseAsync(RelatedListableContentIds, listableContentId);
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
