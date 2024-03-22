using ApiWithJwtBearer.Data.Models;
using ApiWithJwtBearer.Models;
using ApiWithJwtBearer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApiWithJwtBearer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> userManager;
        private readonly ITokenService tokenService;

        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService)
        {
            this.userManager = userManager;
            this.tokenService = tokenService;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterNewUser(dtoNewUser user)
        {
            if (ModelState.IsValid)
            {
                AppUser appUser = new()
                {
                    UserName = user.UserName,
                    Email = user.Email,
                };
                IdentityResult result = await userManager.CreateAsync(appUser, user.Password);
                if (result.Succeeded)
                {
                    return Ok("Success");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }
            return BadRequest(ModelState);
        }
        [HttpPost("LogIn")]
        public async Task<IActionResult> LogIn(dtoLogin login)
        {
            if (ModelState.IsValid)
            {
                AppUser? user = await userManager.FindByNameAsync(login.UserName);
                if (user is not null)
                {
                    if (await userManager.CheckPasswordAsync(user, login.Password))
                    {
                        var authClaims = new List<Claim>()
                        {
                            new Claim(ClaimTypes.Name,user.UserName),
                            new Claim(ClaimTypes.NameIdentifier,user.Id),
                            new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                            new Claim("App","MG"),
                            new Claim(ClaimTypes.Role,"Admin"),
                        };
                        var authRoles = await userManager.GetRolesAsync(user);
                        foreach (var role in authRoles)
                        {
                            authClaims.Add(new Claim(ClaimTypes.Role, role.ToString()));
                        }

                        var token = tokenService.GenerateToken(authClaims);
                        return Ok(token);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                else
                {
                    ModelState.AddModelError("", "invalid username");
                }
            }
            return BadRequest(ModelState);
        }
    }
}
