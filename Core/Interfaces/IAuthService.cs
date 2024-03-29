﻿using Core.Dto;

namespace Core.Interfaces;
public interface IAuthService
{
    Task<AuthModel> RegisterAsync(RegisterDto registerDto);
    Task<AuthModel> Login(LoginDto registerDto);
    Task<string> AddRoleAsync(AddRoleDto addRoleDto);
    Task<AuthModel> RefreshTokenAsync(string token);
    Task<bool> RevokeTokenAsync(string token);

}
