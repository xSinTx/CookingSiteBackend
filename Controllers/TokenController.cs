using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Temalabor.Models;

namespace JWTAuth.WebApi.Controllers
{
    [Route("api/login")]
    [ApiController]
    public class TokenController : ControllerBase
    {

        public IConfiguration _configuration;
        private readonly RecipeContext _context;

        public TokenController(IConfiguration config, RecipeContext context)
        {
            _configuration = config;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]User userParam)
        {
            
            if (userParam != null && userParam.Email!= null && userParam.Password!=null)
            {
                var user = await GetUser(userParam.Email,userParam.Password);

                if (user != null)
                {
                    //create claims details based on the user information
                    var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("UserId", user.Id.ToString()),
                        new Claim("Email", user.Email)
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        _configuration["Jwt:Issuer"],
                        _configuration["Jwt:Audience"],
                        claims,
                        expires: DateTime.UtcNow.AddMinutes(10),
                        signingCredentials: signIn);
                    Console.WriteLine("ok");

                    return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token), user = user });
                }
                else
                {
                    
                    return BadRequest("Invalid credentials");
                }
            }
            else
            {
                Console.WriteLine("itt a baj");
                return BadRequest();
            }
        }

        private async Task<User> GetUser(string email, string pw)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == pw );
        }
    }
}