using Microsoft.AspNetCore.Authorization;
using NewsManagement2.Entities.Galleries;
using NewsManagement2.EntityDtos.GalleryDtos;
using NewsManagement2.EntityDtos.PagedAndSortedDto;
using NewsManagement2.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace NewsManagement2.AppService.Galleries
{
    [Authorize(NewsManagement2Permissions.Galleries.Default)]
    public class GalleryAppService : CrudAppService<Gallery, GalleryDto, int, GetListPagedAndSortedDto, CreateGalleryDto, UpdateGalleryDto>, IGalleryAppService
    {
        private readonly GalleryManager _galleryManager;

        public GalleryAppService(IRepository<Gallery, int> repository, GalleryManager galleryManager) : base(repository)
        {
            _galleryManager = galleryManager;
        }

        [Authorize(NewsManagement2Permissions.Galleries.Create)]
        public override async Task<GalleryDto> CreateAsync(CreateGalleryDto createGalleryDto)
        {
            return await _galleryManager.CreateAsync(createGalleryDto);
        }
        public Task DeleteHardAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
