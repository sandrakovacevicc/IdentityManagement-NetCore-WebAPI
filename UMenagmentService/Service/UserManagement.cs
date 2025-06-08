using Microsoft.AspNetCore.Identity;
using UMenagmentService.Models;
using UMenagmentService.Models.Authentication.Register;
using Microsoft.Extensions.Configuration;
using UMenagmentService.Models.Authentication.Login;
using UMenagmentService.Models.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace UMenagmentService.Service
{
    public class UserManagement : IUserManagement
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public UserManagement(UserManager<User> userManager,
            SignInManager<User> signInManager, IConfiguration configuration, RoleManager<IdentityRole> roleManager, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }
        public async Task<ApiResponse<CreateUserResponse>> CreateUserWithTokenAsync(RegisterUser registerRequest)
        {

            var userExist = await _userManager.FindByEmailAsync(registerRequest.Email);
            if (userExist != null)
            {
                return new ApiResponse<CreateUserResponse> { IsSuccess = false, StatusCode = 403, Message = "User already exists!" };
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

            if (!await _roleManager.RoleExistsAsync("Client"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Client"));
            }

     
            var result = await _userManager.CreateAsync(user, registerRequest.Password);

            if (!result.Succeeded)
            {
                return new ApiResponse<CreateUserResponse> { IsSuccess = false, StatusCode = 500, Message = "User failed to register." };
            }

            await _userManager.AddToRoleAsync(user, "Client");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            return new ApiResponse<CreateUserResponse> { Response = new CreateUserResponse() { User = user, Token = token }, IsSuccess = true, StatusCode = 201, Message = $"User registered & Email Sent to {user.Email} successfully! Please login!" };

        }

        public async Task<ApiResponse<JwtTokenResponse>> GenerateJwtTokenAsync(User user)
        {
            if (user != null)
            {
                var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

                var userRoles = await _userManager.GetRolesAsync(user);

                foreach (var role in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }

                var jwtToken = GetToken(authClaims);


                return new ApiResponse<JwtTokenResponse>
                {
                    Response = new JwtTokenResponse()
                    {
                        Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                        Expiration = jwtToken.ValidTo
                    },
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = $"Token successfully generated"
                };
            }
            else
            {
                return new ApiResponse<JwtTokenResponse>
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    Message = $"User does not exist."
                };

            }
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddDays(2),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

        public async Task<ApiResponse<LoginOtpResponse>> GetOtpByLoginAsync(LoginUser loginUser)
        {
            var user = await _userManager.FindByEmailAsync(loginUser.Email);

            if (user != null)
            {

                await _signInManager.SignOutAsync();
                await _signInManager.PasswordSignInAsync(user, loginUser.Password, false, true);

                if (user.TwoFactorEnabled)
                {

                    var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

                    return new ApiResponse<LoginOtpResponse>
                    {
                        Response = new LoginOtpResponse()
                        {
                            User = (User)user,
                            Token = token,
                            IsTwoFactorEnable = user.TwoFactorEnabled
                        },
                        IsSuccess = true,
                        StatusCode = 201,
                        Message = $"We have sent an OTP to your Email {user.Email}"
                    };

                }
                else
                {
                    return new ApiResponse<LoginOtpResponse>
                    {
                        Response = new LoginOtpResponse()
                        {
                            User = (User)user,
                            Token = string.Empty,
                            IsTwoFactorEnable = user.TwoFactorEnabled
                        },
                        IsSuccess = true,
                        StatusCode = 200,
                        Message = $"2FA is not enabled {user.Email}"
                    };
                }
            }
            else
            {
                return new ApiResponse<LoginOtpResponse>
                {
                    
                    IsSuccess = false,
                    StatusCode = 404,
                    Message = $"User does not exist."
                };
            }


        }
    }
}
