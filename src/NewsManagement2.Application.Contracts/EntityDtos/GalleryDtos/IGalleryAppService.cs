using NewsManagement2.EntityDtos.PagedAndSortedDto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace NewsManagement2.EntityDtos.GalleryDtos
{
    public interface IGalleryAppService : ICrudAppService<GalleryDto, int, GetListPagedAndSortedDto, CreateGalleryDto, UpdateGalleryDto>
    {
        Task DeleteHardAsync(int id);
    }
}
