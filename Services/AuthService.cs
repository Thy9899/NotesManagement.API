using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using NotesManagement.API.DTOs.Auth;
using NotesManagement.API.Helpers;
using NotesManagement.API.Models;
using NotesManagement.API.Repositories;

namespace NotesManagement.API.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly PasswordHasher _passwordHasher;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        IUserRepository userRepository,
        PasswordHasher passwordHasher,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;

        _jwtSettings = configuration
            .GetSection("Jwt")
            .Get<JwtSettings>()
            ?? throw new InvalidOperationException(
                "JWT settings are missing."
            );
    }

    /*
    |-------------------------------------------------------------------
    | AUTH REGISTER
    |-------------------------------------------------------------------
    */
    public async Task<LoginResponse> RegisterAsync(
        RegisterRequest request)
    {
        // VALIDATE INPUT
        var name = request.Name.Trim();
        var email = request.Email.Trim().ToLowerInvariant();
        var password = request.Password;

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is required.");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email is required.");
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password is required.");
        }

        if (password.Length < 6)
        {
            throw new ArgumentException(
                "Password must be at least 6 characters."
            );
        }

        // CHECK WHETHER EAMIL ALREADY EXITS
        var existingUser =
            await _userRepository.GetByEmailAsync(email);

        if (existingUser is not null)
        {
            throw new ArgumentException(
                "Email is already registered."
            );
        }

        // HASH PASSWORD
        var passwordHash =
            _passwordHasher.HashPassword(password);

        // CREATE USER
        var user = new User
        {
            Name = name,
            Email = email,
            PasswordHash = passwordHash
        };

        var userId =
            await _userRepository.CreateAsync(user);

        // GENERATE JWT
        var token = GenerateToken(
            userId,
            name,
            email
        );

        // RETURN RESPONE
        return new LoginResponse
        {
            UserId = userId,
            Name = name,
            Email = email,
            Token = token
        };
    }

    /*
    |-------------------------------------------------------------------
    | AUTH LOGIN
    |-------------------------------------------------------------------
    */
    public async Task<LoginResponse?> LoginAsync(
        LoginRequest request)
    {
        // // VALIDATE INPUT
        var email = request.Email.Trim().ToLowerInvariant();
        var password = request.Password;

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email is required.");
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password is required.");
        }

        // FIND USER
        var user =
            await _userRepository.GetByEmailAsync(email);

        if (user is null)
        {
            return null;
        }

        // VERIFY PASSWORD
        var passwordValid =
            _passwordHasher.VerifyPassword(
                password,
                user.PasswordHash
            );

        if (!passwordValid)
        {
            return null;
        }

        // GENERATE JWT
        var token = GenerateToken(
            user.Id,
            user.Name,
            user.Email
        );

        // RETURN RESPONE
        return new LoginResponse
        {
            UserId = user.Id,
            Name = user.Name,
            Email = user.Email,
            Token = token
        };
    }

    /*
    |-------------------------------------------------------------------
    | GENERATE TOKEN
    |-------------------------------------------------------------------
    */
    private string GenerateToken(
        int userId,
        string name,
        string email)
    {
        var claims = new[]
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                userId.ToString()
            ),

            new Claim(
                ClaimTypes.Name,
                name
            ),

            new Claim(
                ClaimTypes.Email,
                email
            )
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                _jwtSettings.Key
            )
        );

        var credentials =
            new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

        var expiration =
            DateTime.UtcNow.AddMinutes(
                _jwtSettings.ExpiresInMinutes
            );

        var tokenDescriptor =
            new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials
            );

        return new JwtSecurityTokenHandler()
            .WriteToken(tokenDescriptor);
    }
}