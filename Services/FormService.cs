using System.Collections.Frozen;
using CoolFormApi.DTO.Form;
using CoolFormApi.Interfaces;
using CoolFormApi.Interfaces.IServices;
using CoolFormApi.Mappers.MapForms;
using CoolFormApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoolFormApi.Services;

public class FormService: IFormService
{
    private readonly IGenericRepository<Form> _formRepository;
    
    public FormService(IGenericRepository<Form> formRepository)
    {
        _formRepository = formRepository;
    }
    
    public async Task<List<FormDTO>> GetUserForms(int userId)
    {
        return await _formRepository.GetQueryable()
            .Where(x=>x.UserId == userId)
            .Include(x => x.User)
            .Select(x => x.fromFormToFormDTO())
            .ToListAsync();
    }

    public async Task<CreatedFromDTO> CreateForm(CreateFormDTO createFormDto)
    {
        var create = await _formRepository.CreateAsync(createFormDto.fromCreateFormDTOToForm());
        return create.fromFormToCreatedFormDTO();
    }

    public async Task<bool> UpdateForm(int formId, UpdateFormDTO updateFormDto)
    {
        var form = await GetFormById(formId);
        
        if (form == null)
        {
            return false;
        }

        form.Name = updateFormDto.Name;
        form.Description = updateFormDto.Description;
        form.UserId = updateFormDto.UserId;

        await _formRepository.UpdateAsync(form);
        return true;
    }

    public async Task<bool> DeleteForm(int formId)
    {
        var form = await GetFormById(formId);
        if (form == null)
        {
            return false;
        }

        await _formRepository.DeleteAsync(form);
        return true;
    }
    
    private async Task<Form>? GetFormById(int formId)
    {
        return await _formRepository.GetByIdAsync(formId);
    }
}