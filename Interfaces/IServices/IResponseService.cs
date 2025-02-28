using CoolFormApi.DTO.Response;

namespace CoolFormApi.Interfaces.IServices;

public interface IResponseService
{
    public Task<ScoreResultDto> SubmitAnswersAsync(SubmitAnswersDto submitDto);
    public Task<List<MyAnswersDto>> GetUserAnswers(int userId);
    
    public Task<List<FormAnswersDTO>> GetFormAnswers(int formId);
}