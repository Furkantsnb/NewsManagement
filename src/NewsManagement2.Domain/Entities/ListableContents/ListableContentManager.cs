using NewsManagement2.Entities.Galleries;
using NewsManagement2.Entities.Newses;
using NewsManagement2.Entities.Videos;
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


    }
}
