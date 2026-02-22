using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces;

namespace ExpenseTracker.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private static readonly Dictionary<string, string> _phoneCodes = new();

    public AuthService(IUserRepository userRepository, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (await _userRepository.ExistsAsync(request.Email))
            throw new ArgumentException("User already exists");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);

        var token = _jwtService.GenerateToken(user.Id, user.Email);
        return new AuthResponse(token, user.Email, user.FirstName, user.LastName);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials");

        var token = _jwtService.GenerateToken(user.Id, user.Email);
        return new AuthResponse(token, user.Email, user.FirstName, user.LastName);
    }

    public async Task<AuthResponse> LoginWithGoogleAsync(GoogleLoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        
        if (user == null)
        {
            var names = request.Name.Split(' ', 2);
            user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                PasswordHash = string.Empty,
                FirstName = names.Length > 0 ? names[0] : "User",
                LastName = names.Length > 1 ? names[1] : "",
                GoogleId = request.GoogleId,
                CreatedAt = DateTime.UtcNow
            };
            await _userRepository.AddAsync(user);
        }
        else if (string.IsNullOrEmpty(user.GoogleId))
        {
            user.GoogleId = request.GoogleId;
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
        }

        var token = _jwtService.GenerateToken(user.Id, user.Email);
        return new AuthResponse(token, user.Email, user.FirstName, user.LastName);
    }

    public async Task<PhoneCodeResponse> SendPhoneVerificationAsync(SendPhoneCodeRequest request)
    {
        var code = new Random().Next(100000, 999999).ToString();
        _phoneCodes[request.PhoneNumber] = code;
        
        // TODO: Integrate with Twilio/AWS SNS to send actual SMS
        Console.WriteLine($"Verification code for {request.PhoneNumber}: {code}");
        
        return new PhoneCodeResponse($"Verification code sent to {request.PhoneNumber}");
    }

    public async Task<AuthResponse> LoginWithPhoneAsync(PhoneLoginRequest request)
    {
        if (!_phoneCodes.TryGetValue(request.PhoneNumber, out var storedCode) || 
            storedCode != request.VerificationCode)
        {
            throw new UnauthorizedAccessException("Invalid verification code");
        }

        _phoneCodes.Remove(request.PhoneNumber);

        var user = await _userRepository.GetByPhoneAsync(request.PhoneNumber);
        
        if (user == null)
        {
            user = new User
            {
                Id = Guid.NewGuid(),
                Email = $"{request.PhoneNumber}@phone.local",
                PasswordHash = string.Empty,
                FirstName = "User",
                LastName = "",
                PhoneNumber = request.PhoneNumber,
                CreatedAt = DateTime.UtcNow
            };
            await _userRepository.AddAsync(user);
        }

        var token = _jwtService.GenerateToken(user.Id, user.Email);
        return new AuthResponse(token, user.Email, user.FirstName, user.LastName);
    }
}
