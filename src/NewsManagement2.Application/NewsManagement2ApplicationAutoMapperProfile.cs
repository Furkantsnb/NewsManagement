using AutoMapper;
using NewsManagement2.Entities.Cities;
using NewsManagement2.Entities.Tags;
using NewsManagement2.EntityDtos.CityDtos;
using NewsManagement2.EntityDtos.TagDtos;

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
    }

}
