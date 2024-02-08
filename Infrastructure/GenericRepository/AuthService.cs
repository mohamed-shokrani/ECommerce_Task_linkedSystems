using Core.Constants;
using Core.Dto;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.GenericRepository;
public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly JWT _jwt;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AuthService(UserManager<AppUser> userManager, IOptions<JWT> jwt, RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _jwt = jwt.Value;
    }

    public async Task<string> AddRoleAsync(AddRoleDto addRoleDto)
    {
        var user = await _userManager.FindByIdAsync(addRoleDto.UserId);
      
        if (user is null ||!await _roleManager.RoleExistsAsync(addRoleDto.RoleName))
            return "Invalid User ID or Role Name";

        if (await _userManager.IsInRoleAsync(user,addRoleDto.RoleName))
            return "User Already assigned to this role";

        var result = await _userManager.AddToRoleAsync(user,addRoleDto.RoleName);

        return result.Succeeded ? "Roles added successfully" : "Something went wrong";

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
        authModel.IsAuthenticated = true;
        authModel.Email = registerDto.Email;
        authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        var roles = await _userManager.GetRolesAsync(user);
        authModel.Roles = roles.ToList();
        authModel.ExpiresOn = jwtSecurityToken.ValidTo;
        authModel.UserName = user.UserName;

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
        return new AuthModel
        {
            Email = user.Email,
            ExpiresOn = jwtSecurityToken.ValidTo,
            IsAuthenticated = true,
            Roles = new List<string> { RolesConstants.User },
            Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
            UserName = user.UserName,


        };

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



        }.Union(userClaims).Union(rolesClaims);
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
}
