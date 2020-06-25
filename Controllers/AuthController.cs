using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using HOME.FINANCEMENT.API.Data;
using HOME.FINANCEMENT.API.Dtos;
using HOME.FINANCEMENT.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HOME.FINANCEMENT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
         private readonly IConfiguration _config;

        public AuthController(IAuthRepository repo ,IConfiguration config)
        {
            _repo = repo ;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto  userForLoginDto)
        {
           
            // Check if the User Exists
            var userFromRepo = await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);

            if (userFromRepo == null)
                return Unauthorized();
            //

            // Create Claims with Two Claims ID And Username 
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username)
            };

            // Create Key for Signing Credentials 
            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_config.GetSection("AppSettings:Token").Value));
            
            // Create the Signing Credentials 
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // Creating Token Descriptor With 24h 
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            // Creating the Token With JwtSecurityToken 
            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token)
            });
        }





        // UserForRegisterDto works as ViewModel (DTO stands for Data Transfer Objects ) 
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            //Validate Request

            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            if(await _repo.UserExists(userForRegisterDto.Username))
                return BadRequest("User allready exists");

            var userToCreate = new User
            {
                Username = userForRegisterDto.Username
            };

            var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);
            return StatusCode(201);
        }
    }
}
