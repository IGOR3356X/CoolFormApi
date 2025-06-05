using System;
using System.Collections.Generic;

namespace CoolFormApi.Models;

public partial class Question
{
    public int Id { get; set; }

    public int FormId { get; set; }

    public string QuestionText { get; set; } = null!;

    public string QuestionType { get; set; } = null!;

    public string? CorrectAnswer { get; set; }

    public int? Points { get; set; }

    public virtual ICollection<Correctanswer> Correctanswers { get; set; } = new List<Correctanswer>();

    public virtual Form Form { get; set; } = null!;

    public virtual ICollection<Option> Options { get; set; } = new List<Option>();

    public virtual ICollection<Response> Responses { get; set; } = new List<Response>();
}
