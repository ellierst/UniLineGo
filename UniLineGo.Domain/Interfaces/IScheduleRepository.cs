namespace UniLineGo.Domain.Interfaces;

using UniLineGo.Domain.Entities;

public interface IScheduleRepository
{
    Task<ScheduleEntry?> GetByIdAsync(int id);
    Task<IEnumerable<ScheduleEntry>> GetAllAsync();
    Task<int> AddAsync(ScheduleEntry schedule);
    Task UpdateAsync(ScheduleEntry schedule);
    Task DeleteAsync(int id);
}
