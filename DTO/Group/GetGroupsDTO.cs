namespace CoolFormApi.DTO.Group;

public class GetGroupsDTO
{
    public int Id { get; set; }
    public string? Name { get; set; }
}

public class CreateGroupDTO
{
    public string? Name { get; set; }
}

public class CreatedGroupDTO
{
    public int Id { get; set; }
    public string? Name { get; set; }
}

public class UpdateGroupDTO
{
    public string? Name { get; set; }
}