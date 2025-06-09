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

public class FormService : IFormService
{
    private readonly IGenericRepository<Form> _formRepository;
    private readonly IGenericRepository<User> _userRepository;
    private readonly IGenericRepository<FormGroup> _formGroupRepository;
    private readonly IGenericRepository<Group> _groupRepository;

    public FormService(IGenericRepository<Form> formRepository, IGenericRepository<User> userRepository,IGenericRepository<FormGroup> formGroupRepository, IGenericRepository<Group> groupRepository)
    {
        _formRepository = formRepository;
        _userRepository = userRepository;
        _formGroupRepository = formGroupRepository;
        _groupRepository = groupRepository;
    }

    public async Task<List<FormDTO>> GetTeacherFormsForTeachers(int userId)
    {
        return await _formRepository.GetQueryable()
            .Include(x => x.User)
            .Where(x=>x.UserId == userId && x.IsPublic == false)
            .Select(x => x.fromFormToFormDTO())
            .ToListAsync();
    }

    public async Task<List<FormDTO>> GetTeacherFormsForStudents(int userId, List<int>? groupIds = null)
    {
        var query = _formRepository.GetQueryable()
            .Include(x => x.User)
            .Include(x => x.FormGroups)
            .Where(x => x.UserId == userId && x.IsPublic == true);
        
        if (groupIds != null && groupIds.Any())
        {
            query = query.Where(x => x.FormGroups.Any(fg => groupIds.Contains(fg.GroupId)));
        }
    
        return await query
            .Select(x => x.fromFormToFormDTO())
            .ToListAsync();
    }

    public async Task<List<FormDTO>> GetStudentFormsForHisGroup(int userId)
    {
        var user = await _userRepository.GetQueryable()
            .Include(u => u.Group)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null || user.Group == null)
            return new List<FormDTO>();
        
        var groupId = user.Group.Id;

        return await _formRepository.GetQueryable()
            .Include(x => x.User)
            .Include(x => x.FormGroups)
            .Where(x => x.IsPublic && 
                        x.FormGroups.Any(fg => fg.GroupId == groupId))
            .Select(x => x.fromFormToFormDTO())
            .ToListAsync();
    }

    public async Task<CreatedFromDTO> CreateForm(CreateFormDTO createFormDto, int userId)
    {
        var form = createFormDto.fromCreateFormDTOToForm();
        form.UserId = userId;
        
        await _formRepository.CreateAsync(form);
        
        await UpdateFormGroups(form.Id, createFormDto.GroupIds);
        
        var createdForm = await _formRepository.GetQueryable()
            .Include(f => f.FormGroups)
            .FirstOrDefaultAsync(f => f.Id == form.Id);
    
        return createdForm.fromFormToCreatedFormDTO();
    }

    public async Task<bool> UpdateForm(int formId, UpdateFormDTO updateFormDto, int userId)
    {
        var form = await _formRepository.GetQueryable()
            .Include(f => f.FormGroups)
            .FirstOrDefaultAsync(f => f.Id == formId && f.UserId == userId);
    
        if (form == null) 
            return false;

        // Обновляем основные данные
        form.Name = updateFormDto.Name;
        form.Description = updateFormDto.Description;
        form.IsPublic = updateFormDto.IsPublic;
    
        await _formRepository.UpdateAsync(form);
    
        // Обновляем связи с группами
        await UpdateFormGroups(formId, updateFormDto.GroupIds);
    
        return true;
    }

    private async Task UpdateFormGroups(int formId, List<int> groupIds)
    {
        // Удаляем дубликаты и проверяем существование групп
        var uniqueGroupIds = groupIds?.Distinct().ToList() ?? new List<int>();
    
        if (uniqueGroupIds.Any())
        {
            // Проверяем существование групп
            var validGroupIds = await _groupRepository.GetQueryable()
                .Where(g => uniqueGroupIds.Contains(g.Id))
                .Select(g => g.Id)
                .ToListAsync();
        
            uniqueGroupIds = validGroupIds;
        }

        // Получаем текущие связи формы с группами
        var existingGroups = await _formGroupRepository.GetQueryable()
            .Where(fg => fg.FormId == formId)
            .ToListAsync();

        // Находим группы для удаления (которые больше не нужны)
        var groupsToRemove = existingGroups
            .Where(eg => !uniqueGroupIds.Contains(eg.GroupId))
            .ToList();
    
        // Находим группы для добавления (новые, которых еще нет)
        var existingGroupIds = existingGroups.Select(eg => eg.GroupId).ToList();
        var groupsToAdd = uniqueGroupIds
            .Except(existingGroupIds)
            .Select(groupId => new FormGroup 
            { 
                FormId = formId, 
                GroupId = groupId 
            })
            .ToList();

        // Выполняем операции
        if (groupsToRemove.Any())
        {
            await _formGroupRepository.DeleteRangeAsync(groupsToRemove);
        }
    
        if (groupsToAdd.Any())
        {
            await _formGroupRepository.CreateRangeAsync(groupsToAdd);
        }
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