using System;
using System.Collections.Generic;

namespace CoolFormApi.Models;

public partial class Form
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int UserId { get; set; }

    public int MaxScore { get; set; }

    public bool IsPublic { get; set; }

    public virtual ICollection<FormGroup> FormGroups { get; set; } = new List<FormGroup>();

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

    public virtual ICollection<Response> Responses { get; set; } = new List<Response>();

    public virtual ICollection<Score> Scores { get; set; } = new List<Score>();

    public virtual User User { get; set; } = null!;
}
