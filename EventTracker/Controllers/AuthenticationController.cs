using EventTracker.Models;
using EventTracker.Models.Authentication.Register;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace EventTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthenticationController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager, IConfiguration configuration, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _roleManager = roleManager;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody]RegisterUser registerRequest)
        {

            var userExist = await _userManager.FindByEmailAsync(registerRequest.Email);
            if (userExist != null)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new Response { Status = "Error", Message = "User already exists!" });
            }

            User user = new()
            {
                Email = registerRequest.Email,
                Name = registerRequest.Name,
                Surname = registerRequest.Surname,
                UserName = registerRequest.Email,
                PhoneNumber = registerRequest.PhoneNumber,
                TwoFactorEnabled = true,
                SecurityStamp = Guid.NewGuid().ToString(),
            };
           
                var result = await _userManager.CreateAsync(user, registerRequest.Password);

            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = "User failed to register." });
            }

            if (!await _roleManager.RoleExistsAsync("Client"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Client"));
            }

            await _userManager.AddToRoleAsync(user, "Client");

            return StatusCode(StatusCodes.Status201Created,
                new Response { Status = "Success", Message = "User registered successfully! Please login!" });
        }

    }

       
    }

