using System;
using System.Collections.Generic;

namespace CoolFormApi.Models;

public partial class Group
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<FormGroup> FormGroups { get; set; } = new List<FormGroup>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
