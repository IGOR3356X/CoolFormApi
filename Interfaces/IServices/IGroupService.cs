using CoolFormApi.DTO.Group;

namespace CoolFormApi.Interfaces.IServices;

public interface IGroupService
{
    Task<List<GetGroupsDTO>> GetGroups();
    Task<GetGroupsDTO> GetGroupById(int id);
    Task<CreatedGroupDTO> CreateGroup(CreateGroupDTO createGroupDto);
    Task<bool> UpdateGroup(UpdateGroupDTO updateFormDto,int groupId);
    Task<bool> DeleteGroup(int groupId);
}