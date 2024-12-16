using NewsManagement2.EntityDtos.PagedAndSortedDto;
using NewsManagement2.EntityDtos.Videos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace NewsManagement2.EntityDtos.VideoDtos
{
    public interface IVideoAppService : ICrudAppService<VideoDto, int, GetListPagedAndSortedDto, CreateVideoDto, UpdateVideoDto>
    {
        Task DeleteHardAsync(int id);
    }
}
