using Microsoft.AspNetCore.Authorization;
using NewsManagement2.Entities.Newses;
using NewsManagement2.EntityDtos.Newses;
using NewsManagement2.EntityDtos.PagedAndSortedDto;
using NewsManagement2.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace NewsManagement2.AppService.Newses
{
    [Authorize(NewsManagement2Permissions.Newses.Default)]
    public class NewsAppService : CrudAppService<News, NewsDto, int, GetListPagedAndSortedDto, CreateNewsDto, UpdateNewsDto>, INewsAppService
    {
        private readonly NewsManager _newsManager;

        public NewsAppService(IRepository<News, int> repository, NewsManager newsManager) : base(repository)
        {
            _newsManager = newsManager;
        }
        [Authorize(NewsManagement2Permissions.Newses.Create)]
        public override async Task<NewsDto> CreateAsync(CreateNewsDto createNewsDto)
        {
            return await _newsManager.CreateAsync(createNewsDto);
        }
        [Authorize(NewsManagement2Permissions.Newses.Edit)]
        public async override Task<NewsDto> UpdateAsync(int id, UpdateNewsDto updateNewsDto)
        {
            return await _newsManager.UpdateAsync(id, updateNewsDto);
        }
        public async override Task<PagedResultDto<NewsDto>> GetListAsync(GetListPagedAndSortedDto input)
        {
            return await _newsManager.GetListAsync(input);
        }
        public async override Task<NewsDto> GetAsync(int id)
        {
            return await _newsManager.GetByIdAsync(id);
        }
        public Task DeleteHardAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
