using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.ObjectMapping;
using NewsManagement2.Entities.Exceptions;
using NewsManagement2.EntityDtos.CategoryDtos;
using Volo.Abp.Domain.Repositories;
using NewsManagement2.EntityDtos.PagedAndSortedDto;
using Volo.Abp.Application.Dtos;

namespace NewsManagement2.Entities.Categories
{
    /// <summary>
    /// CategoryManager sınıfı, kategori yönetimi için iş mantıklarını içerir.
    /// </summary>
    public class CategoryManager : DomainService
    {
        private readonly IObjectMapper _objectMapper;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IDataFilter<ISoftDelete> _softDeleteFilter;

        public CategoryManager(ICategoryRepository categoryRepository, IObjectMapper objectMapper, IDataFilter<ISoftDelete> softDeleteFilter)
        {
            _categoryRepository = categoryRepository;
            _softDeleteFilter = softDeleteFilter;
            _objectMapper = objectMapper;
        }

        /// <summary>
        /// Yeni bir kategori oluşturur.
        /// </summary>
        /// <param name="createCategoryDto">Oluşturulacak kategorinin DTO'su.</param>
        /// <returns>Oluşturulan kategorinin DTO'su.</returns>
        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
        {
            // DTO'dan Entity'ye dönüştürme
            var category = _objectMapper.Map<CreateCategoryDto, Category>(createCategoryDto);

            // Ana kategori kontrolü
            if (!category.ParentCategoryId.HasValue)
                await ValidateMainCategoryAsync(category);

            // Alt kategori kontrolü
            if (category.ParentCategoryId.HasValue)
                category = await ValidateSubCategoryAsync(category);

            // Veritabanına kaydetme
            var createdCategory = await _categoryRepository.InsertAsync(category, autoSave: true);

            // Oluşturulan kategoriyi DTO'ya dönüştürme ve döndürme
            return _objectMapper.Map<Category, CategoryDto>(createdCategory);
        }

        /// <summary>
        /// Mevcut bir kategoriyi günceller.
        /// </summary>
        /// <param name="id">Güncellenecek kategorinin ID'si.</param>
        /// <param name="updateCategoryDto">Güncelleme için DTO.</param>
        /// <returns>Güncellenen kategorinin DTO'su.</returns>
        public async Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryDto updateCategoryDto)
        {
            // Mevcut kategoriyi al
            var existingCategory = await _categoryRepository.GetAsync(id);

            // Ana kategoriden alt kategoriye geçiş kontrolü
            if (!existingCategory.ParentCategoryId.HasValue && updateCategoryDto.ParentCategoryId.HasValue)
                await ValidateParentIdChangeAsync(id);

            // Güncelleme DTO'sunu mevcut kategoriye uygula
            var updatingCategory = _objectMapper.Map(updateCategoryDto, existingCategory);

            // Ana kategori için validasyon
            if (!updatingCategory.ParentCategoryId.HasValue)
                await ValidateMainCategoryAsync(updatingCategory);

            // Alt kategori için validasyon
            if (updatingCategory.ParentCategoryId.HasValue)
                updatingCategory = await ValidateSubCategoryAsync(updatingCategory, id);

            // Güncellenmiş kategoriyi veritabanına kaydet
            var updatedCategory = await _categoryRepository.UpdateAsync(updatingCategory, autoSave: true);

            // Alt kategorileri ana kategorinin aktiflik durumuna göre güncelle
            if (!updatingCategory.ParentCategoryId.HasValue && !updatingCategory.IsActive)
                await UpdateSubCategoriesActivationAsync(updatingCategory);

            // Güncellenen kategoriyi DTO'ya dönüştür ve döndür
            return _objectMapper.Map<Category, CategoryDto>(updatedCategory);
        }

        /// <summary>
        /// Ana kategori validasyonu yapar.
        /// </summary>
        private async Task ValidateMainCategoryAsync(Category category)
        {
            var isExist = await _categoryRepository.AnyAsync(c =>
                c.CategoryName == category.CategoryName &&
                c.ParentCategoryId == category.ParentCategoryId &&
                c.listableContentType == category.listableContentType &&
                c.Id != category.Id);

            if (isExist)
                throw new AlreadyExistException(typeof(Category), category.CategoryName);
        }

        /// <summary>
        /// Alt kategori validasyonu yapar.
        /// </summary>
        private async Task<Category> ValidateSubCategoryAsync(Category category, int? categoryId = null)
        {
            var isExist = await _categoryRepository.AnyAsync(c =>
                c.CategoryName == category.CategoryName &&
                c.ParentCategoryId == category.ParentCategoryId &&
                c.listableContentType == category.listableContentType &&
                c.Id != categoryId);

            if (isExist)
                throw new AlreadyExistException(typeof(Category), category.CategoryName);

            var parentCategory = await _categoryRepository.GetAsync((int)category.ParentCategoryId);

            if (parentCategory.CategoryName == category.CategoryName)
                throw new BusinessException(NewsManagement2DomainErrorCodes.SubcategoryCannotHaveSameNameParentCategory);

            if (parentCategory.listableContentType != category.listableContentType)
                throw new BusinessException(NewsManagement2DomainErrorCodes.MustHaveTheSameContentType);

            if (parentCategory.ParentCategoryId.HasValue)
                throw new BusinessException(NewsManagement2DomainErrorCodes.OnlyOneSubCategory);

            if (!parentCategory.IsActive)
                category.IsActive = false;

            return category;
        }

        /// <summary>
        /// Ana kategorinin alt kategoriye dönüştürülmesini engeller.
        /// </summary>
        private async Task ValidateParentIdChangeAsync(int id)
        {
            if (await _categoryRepository.AnyAsync(c => c.ParentCategoryId == id))
                throw new BusinessException(NewsManagement2DomainErrorCodes.MainCategoryWithSubCannotBeChanged);
        }

        /// <summary>
        /// Ana kategori aktifliğini alt kategorilere uygular.
        /// </summary>
        private async Task UpdateSubCategoriesActivationAsync(Category category)
        {
            var subCategories = await _categoryRepository.GetListAsync(c =>
                c.ParentCategoryId == category.Id &&
                c.listableContentType == category.listableContentType);

            foreach (var subCategory in subCategories)
            {
                subCategory.IsActive = category.IsActive;
                await _categoryRepository.UpdateAsync(subCategory, autoSave: true);
            }
        }

        /// <summary>
        /// Belirtilen kategoriyi ve varsa alt kategorilerini soft delete yapar.
        /// </summary>
        /// <param name="id">Silinecek kategorinin ID'si.</param>
        /// <returns>Soft delete yapılan kategorilerin listesi.</returns>
        public async Task<List<Category>> SoftDeleteAsync(int id)
        {
            // Kategori veritabanından alınır
            var category = await _categoryRepository.GetAsync(id);

            var deletingList = new List<Category>();

            // Ana kategori ise, alt kategoriler dahil edilir
            if (!category.ParentCategoryId.HasValue)
            {
                deletingList.AddRange(await _categoryRepository.GetListAsync(
                    c => c.ParentCategoryId == id && c.listableContentType == category.listableContentType
                ));
            }

            // Kategori listeye eklenir
            deletingList.Add(category);

            // Soft delete uygulanacak kategoriler döndürülür
            return deletingList;
        }

        /// <summary>
        /// Belirtilen kategoriyi ve varsa alt kategorilerini tamamen siler (Hard Delete).
        /// </summary>
        /// <param name="id">Silinecek kategorinin ID'si.</param>
        public async Task HardDeleteAsync(int id)
        {
            // Kategori veritabanından alınır
            var category = await _categoryRepository.GetAsync(id);

            // Ana kategori ise alt kategorileri kontrol et
            if (!category.ParentCategoryId.HasValue)
            {
                using (_softDeleteFilter.Disable()) // Soft delete filtresini devre dışı bırak
                {
                    // Alt kategoriler alınır
                    var deletingList = await _categoryRepository.GetListAsync(
                        c => c.ParentCategoryId == id && c.listableContentType == category.listableContentType
                    );

                    // Ana kategori alt kategorilerle birlikte tamamen silinir
                    deletingList.Add(category);
                    await _categoryRepository.HardDeleteAsync(deletingList);
                }
            }
            else
            {
                // Tek kategori hard delete yapılır
                await _categoryRepository.HardDeleteAsync(category);
            }
        }

        /// <summary>
        /// Filtre, sıralama ve sayfalama destekli kategori listesi döndürür.
        /// </summary>
        /// <param name="input">Sayfalama ve filtreleme için DTO.</param>
        /// <returns>PagedResultDto formatında kategori listesi.</returns>
        public async Task<PagedResultDto<CategoryDto>> GetListAsync(GetListPagedAndSortedDto input)
        {
            // Kategori sayısı alınır
            var totalCount = input.Filter == null
                ? await _categoryRepository.CountAsync()
                : await _categoryRepository.CountAsync(c => c.CategoryName.Contains(input.Filter));

            // Hiç kategori bulunamazsa hata fırlat
            if (totalCount == 0)
                throw new NotFoundException(typeof(Category), input.Filter ?? string.Empty);

            // Sayfalama sınırlarını kontrol et
            if (input.SkipCount >= totalCount)
                throw new BusinessException(NewsManagement2DomainErrorCodes.FilterLimitsError);

            // Sıralama değeri boş ise varsayılan olarak "CategoryName" kullanılır
            if (input.Sorting.IsNullOrWhiteSpace())
                input.Sorting = nameof(Category.CategoryName);

            // Kategoriler listelenir
            var categoryList = await _categoryRepository.GetListAsync(input.SkipCount, input.MaxResultCount, input.Sorting, input.Filter);

            // Kategoriler DTO'ya dönüştürülür
            var categoryDtoList = _objectMapper.Map<List<Category>, List<CategoryDto>>(categoryList);

            // Sonuç PagedResultDto formatında döndürülür
            return new PagedResultDto<CategoryDto>(totalCount, categoryDtoList);
        }



    }
}
