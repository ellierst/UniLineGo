namespace UniLineGo.Infrastructure.Repositories;

using UniLineGo.Domain.Entities;
using UniLineGo.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using UniLineGo.Infrastructure.Data;

public class ReminderRepository : IReminderRepository
{
    private readonly AppDbContext _context;

    public ReminderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Reminder?> GetByIdAsync(int id)
    {
        return await _context.Reminders.FindAsync(id);
    }

    public async Task<IEnumerable<Reminder>> GetAllAsync()
    {
        return await _context.Reminders.ToListAsync();
    }

    public async Task<int> AddAsync(Reminder reminder)
    {
        _context.Reminders.Add(reminder);
        await _context.SaveChangesAsync();
        return reminder.Id;
    }

    public async Task UpdateAsync(Reminder reminder)
    {
        _context.Reminders.Update(reminder);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var reminder = await GetByIdAsync(id);
        if (reminder != null)
        {
            _context.Reminders.Remove(reminder);
            await _context.SaveChangesAsync();
        }
    }
}
