﻿using EasyAbp.FileManagement.Files;
using NewsManagement2.Entities.Categories;
using NewsManagement2.Entities.Cities;
using NewsManagement2.Entities.Galleries;
using NewsManagement2.Entities.ListableContentRelations;
using NewsManagement2.Entities.ListableContents;
using NewsManagement2.Entities.Tags;
using NewsManagement2.Entities.Videos;
using NewsManagement2.EntityDtos.Newses;
using NewsManagement2.EntityDtos.PagedAndSortedDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;

namespace NewsManagement2.Entities.Newses
{
    /// <summary>
    /// Haber içeriklerini yönetmek için kullanılan bir sınıf.
    /// - Haberlerin oluşturulması, güncellenmesi, silinmesi, listelenmesi ve alınması gibi işlemleri gerçekleştirir.
    /// - Haber detay görselleriyle birlikte ilişkisel varlıkları yönetir.
    /// - Etiketler, şehirler, kategoriler ve diğer ilişkiler için çapraz bağlantıları düzenler.
    /// </summary>
    public class NewsManager : ListableContentBaseManager<News, NewsDto, GetListPagedAndSortedDto, CreateNewsDto, UpdateNewsDto>
    {
       
        private readonly ICityRepository _cityRepository;
        private readonly ITagRepository _tagRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly INewsRepository _newsRepository;
        private readonly IVideoRepository _videoRepository;
        private readonly IGalleryRepository _galleryRepository;
        private readonly IRepository<NewsDetailImage> _newsDetailImageRepository;
        private readonly IListableContentRelationRepository _listableContentRelationRepository;
        private readonly IListableContentTagRepository _listableContentTagRepository;
        private readonly IListableContentCategoryRepository _listableContentCategoryRepository;
        private readonly IListableContentGenericRepository<News> _genericRepository;
        private readonly IListableContentCityRepository _listableContentCityRepository;
        private readonly IObjectMapper _objectMapper;
        private readonly IFileRepository _fileRepository;
        public NewsManager(
          IObjectMapper objectMapper,
          ITagRepository tagRepository,
          IFileRepository fileRepository,
          INewsRepository newsRepository,
          ICityRepository cityRepository,
          IVideoRepository videoRepository,
          IGalleryRepository galleryRepository,
          ICategoryRepository categoryRepository,
          IRepository<NewsDetailImage> newsDetailImageRepository,
          IListableContentGenericRepository<News> genericRepository,
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
            _newsDetailImageRepository = newsDetailImageRepository;
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
