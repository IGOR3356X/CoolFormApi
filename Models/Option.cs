using System;
using System.Collections.Generic;

namespace CoolFormApi.Models;

public partial class Option
{
    public int Id { get; set; }

    public int QuestionId { get; set; }

    public string OptionText { get; set; } = null!;

    public virtual ICollection<CorrectAnswer> Correctanswers { get; set; } = new List<CorrectAnswer>();

    public virtual Question Question { get; set; } = null!;

    public virtual ICollection<ResponseOption> ResponseOptions { get; set; } = new List<ResponseOption>();
}
