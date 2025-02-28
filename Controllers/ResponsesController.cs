using CoolFormApi.DTO.Response;
using CoolFormApi.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoolFormApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResponsesController : ControllerBase
{
    private readonly IResponseService _responseService;
    private readonly ILogger<ResponsesController> _logger;

    public ResponsesController(IResponseService responseService, ILogger<ResponsesController> logger)
    {
        _responseService = responseService;
        _logger = logger;
    }

    [HttpGet]
    [Authorize]
    [Route("MyResponses/{userId}")]
    public async Task<IActionResult> GetMyScores(int userId)
    {
        return Ok(await _responseService.GetUserAnswers(userId));
    }
    
    [HttpGet]
    [Authorize]
    [Route("FormResponses/{formId}")]
    public async Task<IActionResult> GetFormScores(int formId)
    {
        return Ok(await _responseService.GetFormAnswers(formId));
    }
    
    [HttpPost("Submit")]
    [Authorize]
    public async Task<ActionResult<ScoreResultDto>> SubmitAnswers([FromBody] SubmitAnswersDto submitDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _responseService.SubmitAnswersAsync(submitDto);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Question not found");
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting answers");
            return StatusCode(500, "Internal server error");
        }
    }
}