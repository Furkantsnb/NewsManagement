using NewsManagement2.Entities.ListableContents;
using NewsManagement2.EntityDtos.ListableContentDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsManagement2.AppService.ListableContents
{
    public class ListableContentAppService : NewsManagement2AppService, IListableContentAppService
    {
        private readonly ListableContentManager _listableContentManager;

        public ListableContentAppService(ListableContentManager listableContentManager)
        {
            _listableContentManager = listableContentManager;
        }

        public Task<ListableContentDto> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
