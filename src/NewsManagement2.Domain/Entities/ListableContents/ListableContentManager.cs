using NewsManagement2.Entities.Galleries;
using NewsManagement2.Entities.Newses;
using NewsManagement2.Entities.Videos;
using NewsManagement2.EntityDtos.ListableContentDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace NewsManagement2.Entities.ListableContents
{
    /// <summary>
    /// Listelenebilir içerikleri (haber, galeri, video gibi) yönetmek için kullanılan bir sınıf.
    /// İçerik türüne göre farklı yöneticilerden (GalleryManager, NewsManager, VideoManager) veri alır.
    /// </summary>
    public class ListableContentManager : DomainService
    {
        private readonly GalleryManager _galleryManager;
        private readonly NewsManager _newsManager;
        private readonly VideoManager _videoManager;
        private readonly IListableContentRepository _listableContentRepository;


        public ListableContentManager(
          GalleryManager galleryManager,
          NewsManager newsManager,
          VideoManager videoManager,
          IListableContentRepository listableContentRepository

          )
        {
            _galleryManager = galleryManager;
            _newsManager = newsManager;
            _videoManager = videoManager;
            _listableContentRepository = listableContentRepository;

        }
        public async Task<ListableContentDto> GetByIdAsync(int id)
        {
            var listableContent = await _listableContentRepository.GetAsync(id);

            ListableContentDto dto = null;

            if (listableContent is Gallery)
            {
                dto = await _galleryManager.GetByIdAsync(id);
            }

            if (listableContent is News)
            {
                dto = await _newsManager.GetByIdAsync(id);
            }

            if (listableContent is Video)
            {
                dto = await _videoManager.GetByIdAsync(id);
            }

            return dto;
        }

    }
}
