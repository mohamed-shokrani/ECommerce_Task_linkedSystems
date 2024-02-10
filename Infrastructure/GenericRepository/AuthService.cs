using Azure.Core;
using Core.Constants;
using Core.Dto;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.GenericRepository;
public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly JWT _jwt;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AuthService(UserManager<AppUser> userManager, IOptions<JWT> jwt,
                                            RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _jwt = jwt.Value;
    }
   
    public async Task<AuthModel> Login(LoginDto registerDto)
    {
        var authModel = new AuthModel();
        var user = await _userManager.FindByEmailAsync(registerDto.Email);

        if (user is  null || !await _userManager.CheckPasswordAsync(user, registerDto.Password))
        {
            authModel.Message = "Email or Password is Wrong";
            return authModel;
        }
        var jwtSecurityToken = await CreateJwtToken(user);
        if (user.RefreshTokens.Any(x => x.IsActive) && user.RefreshTokens.Any())
        {
            var activeRefreshToken = user.RefreshTokens.FirstOrDefault(x => x.IsActive);
            authModel.RefreshToken = activeRefreshToken.Token;
            authModel.RefreshTokeExpiration = activeRefreshToken.ExpiresOn;
        }
        else
        {
            var refershToken = GenerateRefreshToken();
            authModel.RefreshToken = refershToken.Token;
            authModel.RefreshTokeExpiration = refershToken.ExpiresOn;

            user.RefreshTokens.Add(refershToken);
           
            await _userManager.UpdateAsync(user);
        }


        authModel.IsAuthenticated = true;
        authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        authModel.Email = registerDto.Email;
        
        var rolesList = await _userManager.GetRolesAsync(user);

        authModel.Roles = rolesList.ToList();

        return authModel;

    }
   
  
    public async Task<AuthModel> RegisterAsync(RegisterDto registerDto)
    {
        if (await _userManager.FindByEmailAsync(registerDto.Email) is not null)
            return new AuthModel { Message = "Email is already registered" };

        if (await _userManager.FindByEmailAsync(registerDto.UserName) is not null)
            return new AuthModel { Message = "UserName is already registered" };

        var user = new AppUser
        {
            UserName = registerDto.UserName,
            Email = registerDto.Email,
            FisrtName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Address = registerDto.Address,
        };
       var result= await _userManager.CreateAsync(user,registerDto.Password);
        if (!result.Succeeded)
        {
            var errors = new StringBuilder();
            foreach (var error in result.Errors)
            {
                errors.Append(error.Description +", ");   
            }
            return new AuthModel { Message = errors.ToString() };
        }
        await _userManager.AddToRoleAsync(user, "User");
        var jwtSecurityToken =await CreateJwtToken(user);
        var authModel = new AuthModel();
        if (user.RefreshTokens.Any(x => x.IsActive) && user.RefreshTokens.Any())
        {
            var activeRefreshToken = user.RefreshTokens.FirstOrDefault(x => x.IsActive);
            authModel.RefreshToken = activeRefreshToken.Token;
            authModel.RefreshTokeExpiration = activeRefreshToken.ExpiresOn;
        }
        else
        {
            var refershToken = GenerateRefreshToken();
            authModel.RefreshToken = refershToken.Token;
            authModel.RefreshTokeExpiration = refershToken.ExpiresOn;

            user.RefreshTokens.Add(refershToken);
         
            await _userManager.UpdateAsync(user);
        }

        authModel.Email = registerDto.Email;
      
        authModel.IsAuthenticated = true;
        authModel.Roles = new List<string> { RolesConstants.User };
        authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        authModel.UserName = registerDto.UserName;
        return authModel;
    }
    public async Task<bool> RevokeTokenAsync(string token)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(x => x.RefreshTokens
                                            .Any(x => x.Token == token));
        if (user == null)
            return false;
        var refreshToken = user.RefreshTokens.Single(x => x.Token == token);
        if (!refreshToken.IsActive)
            return false;

        refreshToken.RevokedOn = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);


        return true;
    }
    public async Task<AuthModel> RefreshTokenAsync(string token)
    {
        var authModel = new AuthModel();

        var user = await _userManager.Users.SingleOrDefaultAsync(x => x.RefreshTokens.Any(x => x.Token == token));
        if (user == null)
        {
            authModel.Message = "Invalid Token";
            return authModel;
        }
        var refreshToken = user.RefreshTokens.Single(x => x.Token == token);
        if (!refreshToken.IsActive)
        {
            authModel.Message = "InActive Token";
            return authModel;
        }
        refreshToken.ExpiresOn = DateTime.UtcNow;
        var newRefreshToken = GenerateRefreshToken();
        user.RefreshTokens.Add(newRefreshToken);
        await _userManager.UpdateAsync(user);

        var jwtToken = await CreateJwtToken(user);
        authModel.IsAuthenticated = true;
        authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
        authModel.UserName = user.UserName;
        var roles = await _userManager.GetRolesAsync(user);
        authModel.Roles = roles.ToList();
        authModel.RefreshToken = newRefreshToken.Token;
        authModel.RefreshTokeExpiration = newRefreshToken.ExpiresOn;

        return authModel;
    }
    public async Task<string> AddRoleAsync(AddRoleDto addRoleDto)
    {
        var user = await _userManager.FindByIdAsync(addRoleDto.UserId);

        if (user is null || !await _roleManager.RoleExistsAsync(addRoleDto.RoleName))
            return "Invalid User ID or Role Name";

        if (await _userManager.IsInRoleAsync(user, addRoleDto.RoleName))
            return "User Already assigned to this role";

        var result = await _userManager.AddToRoleAsync(user, addRoleDto.RoleName);

        return result.Succeeded ? "Roles added successfully" : "Something went wrong";

    }

    private async Task<JwtSecurityToken> CreateJwtToken(AppUser appUser)
    {
        var userClaims = await _userManager.GetClaimsAsync(appUser);
        var roles = await _userManager.GetRolesAsync(appUser);
        var rolesClaims = new List<Claim>();
        foreach (var role in roles)
            rolesClaims.Add(new Claim("roles", role));

        var claims = new[]
        {
               new Claim(JwtRegisteredClaimNames.Sub, appUser.UserName),
               new Claim(JwtRegisteredClaimNames.GivenName,$"{ appUser.FisrtName} {appUser.LastName}"),

               new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
              new Claim(JwtRegisteredClaimNames.Email, appUser.Email),



        }.Union(rolesClaims).Union(userClaims);
        var symtricSecruityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var signingCredentials = new SigningCredentials(symtricSecruityKey, SecurityAlgorithms.HmacSha256);
        var symtricSecruityToken = new JwtSecurityToken
        (
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.Now.AddDays(_jwt.DurationInDays),
            signingCredentials: signingCredentials);
        return symtricSecruityToken;

    }
    private RefreshToken GenerateRefreshToken()
    {
        var randomNumber = new byte[32];

        using var generator = new RNGCryptoServiceProvider();

        generator.GetBytes(randomNumber);

        return new RefreshToken
        {
            Token = Convert.ToBase64String(randomNumber),
            ExpiresOn = DateTime.UtcNow.AddMinutes(5),
            CreatedOn = DateTime.UtcNow
        };
    }

}
