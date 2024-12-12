using NewsManagement2.EntityDtos.ListableContentDtos;
using NewsManagement2.EntityDtos.PagedAndSortedDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

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
    }
}
