using NewsManagement2.Entities.Tags;
using NewsManagement2.EntityDtos.PagedAndSortedDto;
using NewsManagement2.EntityDtos.TagDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace NewsManagement2.Tags
{
    public class TagAppService : CrudAppService<Tag, TagDto, int, GetListPagedAndSortedDto, CreateTagDto, UpdateTagDto>, ITagAppService
    {
        private readonly TagManager _tagManager;

        public TagAppService(IRepository<Tag, int> repository, TagManager tagManager) : base(repository)
        {
            _tagManager = tagManager;

        }
        public override async Task<TagDto> CreateAsync(CreateTagDto createTagDto)
        {
            return await _tagManager.CreateAsync(createTagDto);
        }

        public override async Task DeleteAsync(int id)
        {
            await _tagManager.DeleteAsync(id);

            await base.DeleteAsync(id);
        }
        public async Task DeleteHardAsync(int id)
        {
            await _tagManager.DeleteHardAsync(id);
        }

       
        public async override Task<PagedResultDto<TagDto>> GetListAsync(GetListPagedAndSortedDto input)
        {
            return await _tagManager.GetListAsync(input);
        }
    }
}
