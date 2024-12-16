using NewsManagement2.Entities.Videos;
using NewsManagement2.EntityDtos.PagedAndSortedDto;
using NewsManagement2.EntityDtos.VideoDtos;
using NewsManagement2.EntityDtos.Videos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Features;

namespace NewsManagement2.AppService.Videos
{
    public class VideoAppService : CrudAppService<Video, VideoDto, int, GetListPagedAndSortedDto, CreateVideoDto, UpdateVideoDto>, IVideoAppService
    {
        private readonly VideoManager _videoManager;

        public VideoAppService(IRepository<Video, int> repository, VideoManager videoManager) : base(repository)
        {
            _videoManager = videoManager;
        }
        public override async Task<VideoDto> CreateAsync(CreateVideoDto createVideoDto)
        {
            return await _videoManager.CreateAsync(createVideoDto);
        }
        public async override Task<VideoDto> UpdateAsync(int id, UpdateVideoDto updateVideoDto)
        {
            return await _videoManager.UpdateAsync(id, updateVideoDto);
        }
        [RequiresFeature("NewsApp.Video")]
        public async override Task<PagedResultDto<VideoDto>> GetListAsync(GetListPagedAndSortedDto input)
        {
            return await _videoManager.GetListAsync(input);
        }
        public async override Task<VideoDto> GetAsync(int id)
        {
            return await _videoManager.GetByIdAsync(id);
        }
        public Task DeleteHardAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
