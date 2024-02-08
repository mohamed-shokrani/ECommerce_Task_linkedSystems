using Core.Dto;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAuthService _authService;

    public AccountController(IAuthService authService)
    {
        _authService = authService;
    }
    [HttpPost]
    public async Task<IActionResult> RegisterAsync([FromBody ]RegisterDto registerDto)
    {
        if (!ModelState.IsValid)
          return  BadRequest(ModelState);

        var result = await _authService.RegisterAsync(registerDto);
        if (!result.IsAuthenticated)
            return BadRequest(result.Message);
        return Ok(result);
    }
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.Login(loginDto);
        if (!result.IsAuthenticated)
            return BadRequest(result.Message);
        return Ok(result);
    }
    [HttpPost("AddRole")]
    public async Task<ActionResult> AddRoleAsync([FromBody] AddRoleDto addRoleDto )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var result = await _authService.AddRoleAsync(addRoleDto);
        if (!string.IsNullOrEmpty(result))
               return   BadRequest(result);
        
        return Ok(addRoleDto);
    }
}
