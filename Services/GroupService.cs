using CoolFormApi.DTO.Group;
using CoolFormApi.Interfaces;
using CoolFormApi.Interfaces.IServices;
using CoolFormApi.Mappers.MapForms;
using CoolFormApi.Models;

namespace CoolFormApi.Services;

public class GroupService:IGroupService
{
    private readonly IGenericRepository<Group> _groupRepository;

    public GroupService(IGenericRepository<Group> groupRepository)
    {
        _groupRepository = groupRepository;
    }

    public async Task<List<GetGroupsDTO>> GetGroups()
    {
        var groups = await _groupRepository.GetAllAsync();
        return groups.Select(x => x.ToGetGroupsDto()).ToList();
    }

    public async Task<GetGroupsDTO> GetGroupById(int id)
    {
        var group = await _groupRepository.GetByIdAsync(id);
        return group.ToGetGroupsDto();
    }

    public async Task<CreatedGroupDTO> CreateGroup(CreateGroupDTO createGroupDto)
    {
        var creationGroup = await _groupRepository.CreateAsync(createGroupDto.ToGroup());
        return creationGroup.ToCreatedGroupDto();
    }

    public async Task<bool> UpdateGroup(UpdateGroupDTO updateFormDto, int groupId)
    {
        var group = await _groupRepository.GetByIdAsync(groupId);
        if (group == null) return false;
        group.Name = updateFormDto.Name;
        await _groupRepository.UpdateAsync(group);
        return true;
    }

    public async Task<bool> DeleteGroup(int groupId)
    {
        var group = await _groupRepository.GetByIdAsync(groupId);
        if (group == null) return false;
        await _groupRepository.DeleteAsync(group);
        return true;
    }
}