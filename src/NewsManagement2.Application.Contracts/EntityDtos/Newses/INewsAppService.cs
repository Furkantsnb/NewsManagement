using NewsManagement2.EntityDtos.PagedAndSortedDto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace NewsManagement2.EntityDtos.Newses
{
    public interface INewsAppService:ICrudAppService<NewsDto,int,GetListPagedAndSortedDto,CreateNewsDto,UpdateNewsDto>
    {
        Task DeleteHardAsync(int id);
    }
}
