using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;




namespace NewsManagement2.EntityDtos.TagDtos
{
    public interface ITagAppService : ICrudAppService<TagDto, int, CreateTagDto, UpdateTagDto>
    {
        Task DeleteHardAsync(int id);
    }
}
