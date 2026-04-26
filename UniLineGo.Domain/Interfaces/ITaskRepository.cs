namespace UniLineGo.Domain.Interfaces;

using UniLineGo.Domain.Entities;

public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(int id);
    Task<IEnumerable<TaskItem>> GetAllByUserAsync(int userId);
    Task<int> AddAsync(TaskItem task);
    Task UpdateAsync(TaskItem task);
    Task DeleteAsync(int id);
}