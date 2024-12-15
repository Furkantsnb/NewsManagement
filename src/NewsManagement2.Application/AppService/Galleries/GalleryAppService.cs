using NewsManagement2.Entities.Galleries;
using NewsManagement2.EntityDtos.GalleryDtos;
using NewsManagement2.EntityDtos.PagedAndSortedDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace NewsManagement2.AppService.Galleries
{
    public class GalleryAppService : CrudAppService<Gallery, GalleryDto, int, GetListPagedAndSortedDto, CreateGalleryDto, UpdateGalleryDto>, IGalleryAppService
    {
        private readonly GalleryManager _galleryManager;

        public GalleryAppService(IRepository<Gallery, int> repository, GalleryManager galleryManager) : base(repository)
        {
            _galleryManager = galleryManager;
        }
        public Task DeleteHardAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
