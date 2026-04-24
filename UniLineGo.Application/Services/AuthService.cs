namespace UniLineGo.Application.Services;

using System.Security.Cryptography;
using System.Text;
using UniLineGo.Domain.Entities;
using UniLineGo.Domain.Interfaces;

public class AuthService
{
    private readonly IUserRepository _userRepository;
    public static User? CurrentUser { get; private set; }

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

        CurrentUser = user;
        return (true, "Вхід успішний!", user);
    }

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes);
    }

    public void Logout()
    {
        CurrentUser = null;
    }

    public async Task<(bool Success, string Message)> UpdateProfileAsync(
        int userId, string newUsername, string newEmail, string? newPassword)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return (false, "Користувача не знайдено.");

        // Перевірка чи username не зайнятий
        var existingByUsername = await _userRepository.GetByUsernameAsync(newUsername);
        if (existingByUsername != null && existingByUsername.Id != userId)
            return (false, "Це ім'я вже зайняте.");

        // Перевірка чи email не зайнятий
        var existingByEmail = await _userRepository.GetByEmailAsync(newEmail);
        if (existingByEmail != null && existingByEmail.Id != userId)
            return (false, "Цей email вже використовується.");

        user.Username = newUsername;
        user.Email = newEmail;

        if (!string.IsNullOrEmpty(newPassword))
        {
            if (newPassword.Length < 6)
                return (false, "Пароль має бути не менше 6 символів.");
            user.PasswordHash = HashPassword(newPassword);
        }

        await _userRepository.UpdateAsync(user);
        CurrentUser = user;
        return (true, "Профіль оновлено!");
    }
}