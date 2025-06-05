using System;
using System.Collections.Generic;

namespace CoolFormApi.Models;

public partial class User
{
    public int Id { get; set; }

    public string? Login { get; set; }

    public string? Password { get; set; }

    public string? Photo { get; set; }

    public int RoleId { get; set; }

    public int GroupId { get; set; }

    public virtual ICollection<Form> Forms { get; set; } = new List<Form>();

    public virtual Group Group { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<Score> Scores { get; set; } = new List<Score>();
}
