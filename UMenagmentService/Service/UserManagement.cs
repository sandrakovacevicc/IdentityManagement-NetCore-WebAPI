using Microsoft.AspNetCore.Identity;
using UMenagmentService.Models;
using UMenagmentService.Models.Authentication.Register;
using Microsoft.Extensions.Configuration;

namespace UMenagmentService.Service
{
    public class UserManagement : IUserManagement
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public UserManagement(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager, IConfiguration configuration, RoleManager<IdentityRole> roleManager, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        public async Task<ApiResponse<string>> CreateUserWithTokenAsync(RegisterUser registerRequest)
        {

            var userExist = await _userManager.FindByEmailAsync(registerRequest.Email);
            if (userExist != null)
            {
                return new ApiResponse<string> { IsSuccess = false, StatusCode = 403, Message = "User already exists!" };
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
                return new ApiResponse<string> { IsSuccess = false, StatusCode = 500, Message = "User failed to register." };

            }

            if (!await _roleManager.RoleExistsAsync("Client"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Client"));
            }

            await _userManager.AddToRoleAsync(user, "Client");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            return new ApiResponse<string> { IsSuccess = true, StatusCode = 201, Message = $"User registered & Email Sent to {user.Email} successfully! Please login!" };



        }
    }
}
