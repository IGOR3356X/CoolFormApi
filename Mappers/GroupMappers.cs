using CoolFormApi.DTO.Group;
using CoolFormApi.Models;

namespace CoolFormApi.Mappers.MapForms;

public static class GroupMappers
{
    public static GetGroupsDTO ToGetGroupsDto(this Group group)
    {
        return new GetGroupsDTO()
        {
            Id = group.Id,
            Name = group.Name
        };
    }

    public static Group ToGroup(this CreateGroupDTO createGroupDto)
    {
        return new Group()
        {
            Name = createGroupDto.Name,
        };
    }

    public static CreatedGroupDTO ToCreatedGroupDto(this Group group)
    {
        return new CreatedGroupDTO
        {
            Id = group.Id,
            Name = group.Name
        };
    }
}