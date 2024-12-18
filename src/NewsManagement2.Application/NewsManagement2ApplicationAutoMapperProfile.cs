using AutoMapper;
using NewsManagement2.Entities.Categories;
using NewsManagement2.Entities.Cities;
using NewsManagement2.Entities.Galleries;
using NewsManagement2.Entities.Newses;
using NewsManagement2.Entities.Tags;
using NewsManagement2.Entities.Videos;
using NewsManagement2.EntityDtos.CategoryDtos;
using NewsManagement2.EntityDtos.CityDtos;
using NewsManagement2.EntityDtos.GalleryDtos;
using NewsManagement2.EntityDtos.Newses;
using NewsManagement2.EntityDtos.TagDtos;
using NewsManagement2.EntityDtos.VideoDtos;
using NewsManagement2.EntityDtos.Videos;
using System.Linq;

namespace NewsManagement2;

public class NewsManagement2ApplicationAutoMapperProfile : Profile
{
    public NewsManagement2ApplicationAutoMapperProfile()
    {
        #region Tag
        CreateMap<Tag, TagDto>().ReverseMap();
        CreateMap<UpdateTagDto, Tag>().ReverseMap();
        CreateMap<CreateTagDto, Tag>().ReverseMap();
        #endregion

        #region City
        CreateMap<City, CityDto>().ReverseMap();
        CreateMap<UpdateCityDto, City>().ReverseMap();
        CreateMap<CreateCityDto, City>().ReverseMap();
        #endregion

        #region Category
        CreateMap<Category, CategoryDto>().ReverseMap();
        CreateMap<UpdateCategoryDto, Category>().ReverseMap();
        CreateMap<CreateCategoryDto, Category>().ReverseMap();
        #endregion

        #region Gallery
        CreateMap<Gallery, GalleryDto>().ReverseMap();
        CreateMap<UpdateGalleryDto, Gallery>().ReverseMap();
        CreateMap<CreateGalleryDto, Gallery>().ReverseMap();
        CreateMap<GalleryImage, GalleryImageDto>().ReverseMap();

        CreateMap<CreateGalleryDto, UpdateGalleryDto>().ReverseMap();
        #endregion

        #region Video
        CreateMap<Video, VideoDto>().ReverseMap();
        CreateMap<UpdateVideoDto, Video>().ReverseMap();
        CreateMap<CreateVideoDto, Video>().ReverseMap();
        CreateMap<CreateVideoDto, UpdateVideoDto>().ReverseMap();
        #endregion

        #region News
        CreateMap<News, NewsDto>()
        .ForMember(dest => dest.DetailImageId, opt => opt.MapFrom(src => src.DetailImageIds.Select(d => d.DetailImageId).ToList()))
        .ReverseMap()
        .ForMember(dest => dest.DetailImageIds, opt => opt.MapFrom(src => src.DetailImageId.Select(id => new NewsDetailImage { DetailImageId = id }).ToList()));

        CreateMap<UpdateNewsDto, News>().ReverseMap();
        CreateMap<CreateNewsDto, News>().ReverseMap();

        CreateMap<NewsDetailImageDto, NewsDetailImage>().ReverseMap();
        CreateMap<CreateNewsDto, UpdateNewsDto>().ReverseMap();
        #endregion
    }

}
