using System;
using System.Collections.Generic;

namespace CoolFormApi.Models;

public partial class FormGroup
{
    public int Id { get; set; }

    public int GroupId { get; set; }

    public int FormId { get; set; }

    public virtual Form Form { get; set; } = null!;

    public virtual Group Group { get; set; } = null!;
}
