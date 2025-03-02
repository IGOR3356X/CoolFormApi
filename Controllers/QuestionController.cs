using System.ComponentModel.DataAnnotations;
using CoolFormApi.DTO.Questions;
using CoolFormApi.Interfaces.IServices;
using CoolFormApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoolFormApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuestionController:ControllerBase
{
    private readonly IQuestionService _questionService;

    public QuestionController(IQuestionService questionService)
    {
        _questionService = questionService;
    }
    
    [HttpGet]
    [Authorize]
    [Route("{formId}")]
    public async Task<IActionResult> GetQuestions(int formId)
    {
        var gg = await _questionService.getAllQuestions(formId);
        return Ok(gg);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddQuestion([FromBody] CreateNewQuestionDTO newQuestion)
    {
         var gg = await _questionService.AddQuestion(newQuestion);
         await _questionService.UpdateFormMaxScore(newQuestion.FormId);
         return Ok(gg);
    }

    [HttpPut]
    [Authorize]
    [Route("{questionId}")]
    public async Task<IActionResult> UpdateQuestion([FromBody] UpdateQuestionDTO updateQuestion, int questionId)
    {
        await _questionService.UpdateQuestion(updateQuestion, questionId);
        await _questionService.UpdateFormMaxScore(updateQuestion.FormId);
        return Ok(new { message = "Question updated" });
    }
    
    [HttpDelete]
    [Authorize]
    [Route("{questionId}")]
    public async Task<IActionResult> DeleteQuestion(int questionId,[Required]int formId)
    {
        try
        {
            await _questionService.DeleteQuestion(questionId);
            await _questionService.UpdateFormMaxScore(formId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }

    }
}