using System;
using System.Collections.Generic;

namespace CoolFormApi.Models;

public partial class Score
{
    public int Id { get; set; }

    public int FormId { get; set; }

    public int UserId { get; set; }

    public int Score1 { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Form Form { get; set; } = null!;

    public virtual ICollection<Response> Responses { get; set; } = new List<Response>();

    public virtual User User { get; set; } = null!;
}
