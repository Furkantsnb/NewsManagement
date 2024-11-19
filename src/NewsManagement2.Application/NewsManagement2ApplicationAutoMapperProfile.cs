using AutoMapper;
using NewsManagement2.Entities.Tags;
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
    }
}
