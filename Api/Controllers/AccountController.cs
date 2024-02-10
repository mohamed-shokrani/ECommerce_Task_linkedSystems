using Core.Dto;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/account")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAuthService _authService;

    public AccountController(IAuthService authService)
    {
        _authService = authService;
    }
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody ]RegisterDto registerDto)
    {
        if (!ModelState.IsValid)
          return  BadRequest(ModelState);

        var result = await _authService.RegisterAsync(registerDto);
        if (!result.IsAuthenticated)
            return BadRequest(result.Message);

        if (!string.IsNullOrEmpty(result.RefreshToken))
            SetRefereshTokenInCookie(result.RefreshToken, result.RefreshTokeExpiration);

        return Ok(result);
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.Login(loginDto);

        if (!result.IsAuthenticated)
            return BadRequest(result.Message);

        if (!string.IsNullOrEmpty(result.RefreshToken))
            SetRefereshTokenInCookie(result.RefreshToken, result.RefreshTokeExpiration);
       
           
        return Ok(result);
    }
 
    [HttpPost("addRole")]
    public async Task<ActionResult> AddRoleAsync([FromBody] AddRoleDto addRoleDto )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var result = await _authService.AddRoleAsync(addRoleDto);
        if (!string.IsNullOrEmpty(result))
               return   BadRequest(result);
        
        return Ok(addRoleDto);
    }
    [HttpPost("logout")]
    public async Task<ActionResult<AuthModel>> RevokToken([FromBody] RovokeTokenDto dto)
    {
        var token = dto.Token ?? Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(token))
            return BadRequest("Token is required");

        var result = await _authService.RevokeTokenAsync(token);
        return result ? Ok() : BadRequest("Token is Invalid");
    }
    private void SetRefereshTokenInCookie(string refreshToken, DateTime expires)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = expires.ToLocalTime(),
        };

        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }
}
