using NewsManagement2.Entities.Videos;
using NewsManagement2.EntityDtos.PagedAndSortedDto;
using NewsManagement2.EntityDtos.VideoDtos;
using NewsManagement2.EntityDtos.Videos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace NewsManagement2.AppService.Videos
{
    public class VideoAppService : CrudAppService<Video, VideoDto, int, GetListPagedAndSortedDto, CreateVideoDto, UpdateVideoDto>, IVideoAppService
    {
        private readonly VideoManager _videoManager;

        public VideoAppService(IRepository<Video, int> repository, VideoManager videoManager) : base(repository)
        {
            _videoManager = videoManager;
        }

        public Task DeleteHardAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
