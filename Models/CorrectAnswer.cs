using System;
using System.Collections.Generic;

namespace CoolFormApi.Models;

public partial class Correctanswer
{
    public int Id { get; set; }

    public int Questionid { get; set; }

    public int Optionid { get; set; }

    public virtual Option Option { get; set; } = null!;

    public virtual Question Question { get; set; } = null!;
}
