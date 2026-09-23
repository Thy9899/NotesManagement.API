using Microsoft.AspNetCore.Mvc;
using NotesManagement.API.DTOs.Auth;
using NotesManagement.API.Services;

namespace NotesManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /*
    |-------------------------------------------------------------------
    | AUTH REGISTER
    |-------------------------------------------------------------------
    */
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterRequest request)
    {
        try
        {
            var response =
                await _authService.RegisterAsync(request);

            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    /*
    |-------------------------------------------------------------------
    | AUTH LOGIN
    |-------------------------------------------------------------------
    */
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginRequest request)
    {
        var response =
            await _authService.LoginAsync(request);

        if (response is null)
        {
            return Unauthorized(new
            {
                message = "Invalid email or password."
            });
        }

        return Ok(response);
    }
}