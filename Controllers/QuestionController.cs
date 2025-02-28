﻿using CoolFormApi.DTO.Questions;
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
    public async Task<IActionResult> GetQuestions(int formId)
    {
        var gg = await _questionService.getAllQuestions(formId);
        return Ok(gg);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddQuestion([FromBody] CreateNewQuestionDTO newQuestion)
    {
         await _questionService.AddQuestion(newQuestion);
         await _questionService.UpdateFormMaxScore(newQuestion.FormId);
         return Ok();
    }

    [HttpPut]
    [Authorize]
    [Route("{questionId}")]
    public async Task<IActionResult> UpdateQuestion([FromBody] UpdateQuestionDTO updateQuestion, int questionId)
    {
        await _questionService.UpdateQuestion(updateQuestion, questionId);
        await _questionService.UpdateFormMaxScore(updateQuestion.FormId);
        return Ok();
    }
    
    [HttpDelete]
    [Authorize]
    [Route("{questionId}")]
    public async Task<IActionResult> DeleteQuestion(int formId ,int questionId)
    {
        await _questionService.DeleteQuestion(questionId);
        await _questionService.UpdateFormMaxScore(formId);
        return Ok();
    }
}