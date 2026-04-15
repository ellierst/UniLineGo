namespace UniLineGo.Application.Services;

using System.Security.Cryptography;
using System.Text;
using UniLineGo.Domain.Entities;
using UniLineGo.Domain.Interfaces;

public class AuthService
{
    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<(bool Success, string Message)> RegisterAsync(
        string username, string email, string password)
    {
        if (await _userRepository.ExistsAsync(email, username))
            return (false, "Користувач з таким email або іменем вже існує.");

        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = HashPassword(password),
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);
        return (true, "Реєстрація успішна!");
    }

    public async Task<(bool Success, string Message, User? User)> LoginAsync(
        string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
            return (false, "Користувача не знайдено.", null);

        if (user.PasswordHash != HashPassword(password))
            return (false, "Невірний пароль.", null);

        return (true, "Вхід успішний!", user);
    }

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes);
    }
}