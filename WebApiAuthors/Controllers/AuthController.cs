using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiAuthors.Dtos;

namespace WebApiAuthors.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> manager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IConfiguration configuration;

        public AuthController(UserManager<IdentityUser> manager, SignInManager<IdentityUser> signInManager ,IConfiguration configuration)
        {
            this.manager = manager;
            this.signInManager = signInManager;
            this.configuration = configuration;
        }

        [HttpPost("Login")]
        public async Task<ActionResult<AuthenticationResponse>> Login(UserCredentialsDto userCredentialsDto)
        {
            var result = await signInManager
                .PasswordSignInAsync(
                    userCredentialsDto.Email, 
                    userCredentialsDto.Password, 
                    isPersistent: false, 
                    lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return BadRequest("Invalid Credentials");
            }

            return BuildToken(userCredentialsDto);
        }

        [HttpPost("Register")]
        public async Task<ActionResult<AuthenticationResponse>> RegisterUser(UserCredentialsDto userCredentialsDto)
        {
            IdentityUser user = new IdentityUser()
            {
                UserName = userCredentialsDto.Email,
                Email = userCredentialsDto.Email
            };

            IdentityResult result = await manager.CreateAsync(user, userCredentialsDto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return BuildToken(userCredentialsDto);
        }

        [HttpGet("RenewToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult<AuthenticationResponse> RenewToken()
        {
            Claim emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            string email = emailClaim?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized();
            }

            UserCredentialsDto user = new UserCredentialsDto()
            {
                Email = email,
            };

            return BuildToken(user);
        }

        private AuthenticationResponse BuildToken(UserCredentialsDto userCredentialsDto)
        {
            List<Claim> claims = new List<Claim>() { 
                new Claim("email", userCredentialsDto.Email)
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtKey"]));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            DateTime expirationToken = DateTime.UtcNow.AddMinutes(30);
            JwtSecurityToken securityToken = new JwtSecurityToken(
                issuer: null, 
                audience: null, 
                claims,
                expires: expirationToken,
                signingCredentials: credentials);

            return new AuthenticationResponse()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiration = expirationToken,
            };
        }
    }
}
