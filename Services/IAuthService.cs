using NotesManagement.API.DTOs.Auth;

namespace NotesManagement.API.Services;

public interface IAuthService
{
    Task<LoginResponse> RegisterAsync(RegisterRequest request);

    Task<LoginResponse?> LoginAsync(LoginRequest request);
}