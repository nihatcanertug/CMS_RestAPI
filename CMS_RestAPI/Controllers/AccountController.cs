using CMS_RestAPI.Models;
using CMS_RestAPI.Models.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CMS_RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly AppSettings _appSetting;

        public AccountController(ApplicationDbContext db, IOptions<AppSettings> options)
        {
            _db = db;
            _appSetting = options.Value;
        }

        [HttpPost("login")]
        public IActionResult Login(AppUser appUser)
        {

            AppUser user = Authentication(appUser.UserName, appUser.Password);

            if (user == null) return BadRequest(new { message = "User name or password are incorrect..!" });

            return Ok("Succeded..!");
        }

        private AppUser Authentication(string userName, string password)
        {
            AppUser user = _db.Users.SingleOrDefault(x => x.UserName == userName && x.Password == password);

            if (user == null) return null;
            else
            {
                //şayet kullanıcı varsa token üret
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSetting.SecretKey);
                var tokenDescription = new SecurityTokenDescriptor()
                {
                    Subject = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, user.Id.ToString()) }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                };

                user.Token = tokenHandler.CreateToken(tokenDescription);

                return user;
            }
        }
    }
}
