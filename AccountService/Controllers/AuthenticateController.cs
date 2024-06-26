﻿using AccountService.Models;
using AccountService.Models.VM;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQHelper;
using SharingModels.ModelsVM;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AccountService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private MapperConfiguration config;
        private Mapper mapper;

        public AuthenticateController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;

            config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            mapper = new Mapper(config);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModelVM model)
        {
            if (!await _roleManager.RoleExistsAsync("User"))
                await _roleManager.CreateAsync(new IdentityRole("User"));
            if (!await _roleManager.RoleExistsAsync("GameMaster"))
                await _roleManager.CreateAsync(new IdentityRole("GameMaster"));


            var userExists = await _userManager.FindByNameAsync(model.Username);
            var userExist2 = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null || userExist2 != null)
                return StatusCode(StatusCodes.Status500InternalServerError, value: "User already exists!");

            ApplicationUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, value: "User creation failed! Please check user details and try again.");

            if (model.UserRole != null)
                if (await _roleManager.RoleExistsAsync(model.UserRole))
                    await _userManager.AddToRoleAsync(user, model.UserRole);

            #region RabbitMQ
            RabbitMQCRUD rabbitMQCRUD = new RabbitMQCRUD("localhost", "rabbitmq", "rabbitmq");
            var existQueue = rabbitMQCRUD.ExistQueue("users");
            if (!existQueue)
            {
                List<ApplicationUser> users = _userManager.Users.ToList();
                List<ApplicationUserVM> applicationUserVM = mapper.Map<List<ApplicationUserVM>>(users);
                var json = JsonConvert.SerializeObject(applicationUserVM);

                rabbitMQCRUD.NewQueues("users", json);
            }
            else
            {
                List<ApplicationUser> users = new List<ApplicationUser> { user };
                List<ApplicationUserVM> applicationUserVM = mapper.Map<List<ApplicationUserVM>>(users);
                var json = JsonConvert.SerializeObject(applicationUserVM);

                rabbitMQCRUD.NewQueues("users", json);
            }
            #endregion

            return Created();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModelVM model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);

            if (user == null)
                user = await _userManager.FindByEmailAsync(model.Username);

            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                authClaims.Add(new Claim("FullName", user.FirstName + " " + user.LastName));
                authClaims.Add(new Claim("Id", user.Id));

                var token = CreateToken(authClaims);
                var refreshToken = GenerateRefreshToken();

                user.RefreshToken = refreshToken;
                if (model.RememberMe)
                {
                    _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);
                    user.RefreshTokenExpiryTime = DateTime.Now.AddDays(refreshTokenValidityInDays);
                }
                else
                {
                    _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInMonthsRememberMe"], out int refreshTokenValidityInMonths);
                    user.RefreshTokenExpiryTime = DateTime.Now.AddDays(refreshTokenValidityInMonths);
                }


                await _userManager.UpdateAsync(user);

                return Ok(new
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    RefreshToken = refreshToken,
                    Expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }

        private JwtSecurityToken CreateToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            _ = int.TryParse(_configuration["JWT:TokenValidityInMinutes"], out int tokenValidityInMinutes);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddMinutes(tokenValidityInMinutes),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
