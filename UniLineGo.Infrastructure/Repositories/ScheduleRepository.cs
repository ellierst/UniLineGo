namespace UniLineGo.Infrastructure.Repositories;

using UniLineGo.Domain.Entities;
using UniLineGo.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using UniLineGo.Infrastructure.Data;

public class ScheduleRepository : IScheduleRepository
{
    private readonly AppDbContext _context;

    public ScheduleRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ScheduleEntry?> GetByIdAsync(int id)
    {
        return await _context.ScheduleEntries.FindAsync(id);
    }

    public async Task<IEnumerable<ScheduleEntry>> GetAllAsync()
    {
        return await _context.ScheduleEntries.ToListAsync();
    }

    public async Task<int> AddAsync(ScheduleEntry schedule)
    {
        _context.ScheduleEntries.Add(schedule);
        await _context.SaveChangesAsync();
        return schedule.Id;
    }

    public async Task UpdateAsync(ScheduleEntry schedule)
    {
        _context.ScheduleEntries.Update(schedule);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var schedule = await GetByIdAsync(id);
        if (schedule != null)
        {
            _context.ScheduleEntries.Remove(schedule);
            await _context.SaveChangesAsync();
        }
    }
}
