using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace NewsManagement2.Entities.ListableContentRelations
{
    public interface IListableContentCityRepository : IRepository<ListableContentCategory>
    {
        Task<List<ListableContentCategory>> GetCrossListAsync(int id);
    }
}
