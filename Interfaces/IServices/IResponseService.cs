using CoolFormApi.DTO.Response;

namespace CoolFormApi.Interfaces.IServices;

public interface IResponseService
{
    Task<ScoreResultDto> SubmitAnswersAsync(SubmitAnswersDto submitDto);
    Task<List<MyAnswersDto>> GetUserAnswers(int userId);
    Task<AttemptDetailsDto> GetAttemptDetailsAsync(int scoreId, int currentUserId);
    Task<List<FormAnswersDTO>> GetFormAnswers(int formId);
}