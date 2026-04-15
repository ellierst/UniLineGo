namespace UniLineGo.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using UniLineGo.Domain.Entities;
using UniLineGo.Domain.Interfaces;
using UniLineGo.Infrastructure.Data;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
        => await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<User?> GetByUsernameAsync(string username)
        => await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

    public async Task<bool> ExistsAsync(string email, string username)
        => await _context.Users.AnyAsync(u => u.Email == email || u.Username == username);

    public async Task<int> AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user.Id;
    }
}