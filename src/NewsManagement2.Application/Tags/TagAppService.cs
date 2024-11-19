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
    public class TagAppService : CrudAppService<Tag, TagDto, int, PagedAndSortedResultRequestDto, CreateTagDto, UpdateTagDto>, ITagAppService
    {
        public TagAppService(IRepository<Tag, int> repository) : base(repository)
        {
        }

        public Task DeleteHardAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResultDto<TagDto>> GetListAsync(GetListPagedAndSortedDto input)
        {
            throw new NotImplementedException();
        }
    }
}
