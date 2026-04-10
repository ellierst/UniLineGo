namespace UniLineGo.Domain.Interfaces;

using UniLineGo.Domain.Entities;

public interface IReminderRepository
{
    Task<Reminder?> GetByIdAsync(int id);
    Task<IEnumerable<Reminder>> GetAllAsync();
    Task<int> AddAsync(Reminder reminder);
    Task UpdateAsync(Reminder reminder);
    Task DeleteAsync(int id);
}
