using System;
using System.Collections.Generic;

namespace CoolFormApi.Models;

public partial class ResponseOption
{
    public int Id { get; set; }

    public int ResponseId { get; set; }

    public int OptionId { get; set; }

    public virtual Option Option { get; set; } = null!;

    public virtual Response Response { get; set; } = null!;
}
