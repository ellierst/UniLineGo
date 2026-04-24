namespace UniLineGo.Domain.Interfaces;

using UniLineGo.Domain.Entities;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByIdAsync(int id); 
    Task<bool> ExistsAsync(string email, string username);
    Task<int> AddAsync(User user);
    Task UpdateAsync(User user);
}