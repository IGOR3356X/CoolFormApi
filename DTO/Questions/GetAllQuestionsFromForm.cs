using CoolFormApi.Models;

namespace CoolFormApi.DTO.Questions;

public class GetFormDescription
{
    public string FormName { get; set; }
    public string FormDescription { get; set; }
    public List<GetAllQuestionsFromForm>? Questions { get; set; }
}
public class GetAllQuestionsFromForm
{
    public int Id { get; set; }
    public string QuestionText { get; set; }
    public string QuestionType { get; set; }
    public int Points { get; set; }
    public List<QuestionOptions> QuestionOptions { get; set; }
}

public class QuestionOptions
{
    public int Id { get; set; }
    public string OptionText { get; set; }
}