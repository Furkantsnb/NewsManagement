﻿using System;
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

    }
}
