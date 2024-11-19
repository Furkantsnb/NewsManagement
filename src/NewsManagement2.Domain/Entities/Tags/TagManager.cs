﻿
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
using Volo.Abp.Domain.Entities;

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

        public async Task<TagDto> UpdateAsync(int id, UpdateTagDto updateTag)
        {
            var existingTag = await _tagRepository.GetAsync(id);

            var tagAlreadyExists = await _tagRepository.AnyAsync(t => t.TagName == updateTag.TagName && t.Id != id);
            if (tagAlreadyExists)
                 throw new BusinessException("aynı isimde tag mevcut");

            _objectMapper.Map(updateTag, existingTag);

            var tag = await _tagRepository.UpdateAsync(existingTag);

            var tagDto = _objectMapper.Map<Tag, TagDto>(tag);

            return tagDto;
        }

        public async Task DeleteAsync(int id)
        {
            var isTagExist = await _tagRepository.AnyAsync(t => t.Id == id);
            if (!isTagExist)
                throw new EntityNotFoundException(typeof(Tag), id);
        }

        public async Task DeleteHardAsync(int id)
        {
            var tag = await _tagRepository.GetAsync(id);

            await _tagRepository.HardDeleteAsync(tag);
        }
    }
}
