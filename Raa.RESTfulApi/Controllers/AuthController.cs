using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Raa.RESTfulApi.Entities;
using Raa.RESTfulApi.Models;
using Raa.RESTfulApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Raa.RESTfulApi.Controllers
{
    [Authorize]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IJwtService _jwtService;
        private IPasswordHasher<ApplicationUser> _passwordHasher;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager,
            IJwtService jwtService, IPasswordHasher<ApplicationUser> passwordHasher)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
            _passwordHasher = passwordHasher;

        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid) return BadRequest();

            var user = new ApplicationUser()
            {
                UserName = model.Username,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            await _userManager.AddToRoleAsync(user, "user");

            var LoginDtoModel = new LoginDto { Username = model.Username, Password = model.Password };
            var tokenResult = _jwtService.CreateToken(user);

            return CreatedAtRoute("CreateToken", new { model = LoginDtoModel }, tokenResult);


        }

        [AllowAnonymous]
        [HttpPost("token", Name = "CreateToken")]
        public async Task<IActionResult> CreateToken([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid) return BadRequest();

            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null) return Unauthorized();

            var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);

            if (verifyResult != PasswordVerificationResult.Success) return Unauthorized();

            var tokenResult = _jwtService.CreateToken(user);


            return Ok(tokenResult);

        }

    }
}
