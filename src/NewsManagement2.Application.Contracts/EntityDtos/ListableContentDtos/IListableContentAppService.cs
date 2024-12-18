using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace NewsManagement2.EntityDtos.ListableContentDtos
{
    public interface IListableContentAppService : IApplicationService
    {
        Task<ListableContentDto> GetByIdAsync(int id);
    }
}
