
using Volo.Abp.ObjectMapping;
using NewsManagement2.EntityDtos.TagDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp;

namespace NewsManagement2.Entities.Tags
{
    public class TagManager : DomainService
    {
        private readonly ITagRepository _tagRepository;
        private readonly IObjectMapper _objectMapper;

        public TagManager(ITagRepository tagRepository, IObjectMapper objectMapper)
        {
            _tagRepository = tagRepository;
            _objectMapper = objectMapper;
        }

        public async Task<TagDto> CreateAsync(CreateTagDto createTagDto)
        {
            var tagAlreadyExists = await _tagRepository.AnyAsync(t => t.TagName == createTagDto.TagName);

            if (tagAlreadyExists)
                throw new BusinessException("aynı isimde tag mevcut");

            var createTag = _objectMapper.Map<CreateTagDto, Tag>(createTagDto);

            var tag = await _tagRepository.InsertAsync(createTag);

            var tagDto = _objectMapper.Map<Tag, TagDto>(tag);

            return tagDto;
        }

    }
}
