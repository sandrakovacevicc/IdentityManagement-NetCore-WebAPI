using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserManagment.Service.Models.Authentication.User;
using UserManagment.Service.Models.Authentication.Login;
using UserManagment.Service.Models.Authentication.Register;
using UserManagment.Service.Models;
using UserManagement.Data.Models;
using System.Security.Cryptography;

namespace UserManagment.Service.Service
{
    public class UManagement : IUManagement
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public UManagement(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager, IConfiguration configuration, RoleManager<IdentityRole> roleManager, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }
        public async Task<ApiResponse<CreateUserResponse>> CreateUserWithTokenAsync(RegisterUser registerRequest)
        {
            var userExist = await _userManager.FindByEmailAsync(registerRequest.Email.ToLower());
            if (userExist != null)
            {
                return new ApiResponse<CreateUserResponse>
                {
                    IsSuccess = false,
                    StatusCode = 403,
                    Message = "User already exists!"
                };
            }

            AppUser user = new()
            {
                Email = registerRequest.Email,
                Name = registerRequest.Name,
                Surname = registerRequest.Surname,
                UserName = registerRequest.Email,
                PhoneNumber = registerRequest.PhoneNumber,
                TwoFactorEnabled = true,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            if (!await _roleManager.RoleExistsAsync(registerRequest.Role))
            {
                return new ApiResponse<CreateUserResponse>
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = "Role doesn't exist!"
                };
            }

            var result = await _userManager.CreateAsync(user, registerRequest.Password);

            if (!result.Succeeded)
            {
                return new ApiResponse<CreateUserResponse>
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = "User failed to register."
                };
            }

            await _userManager.AddToRoleAsync(user, registerRequest.Role);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            return new ApiResponse<CreateUserResponse>
            {
                Response = new CreateUserResponse()
                {
                    User = user,
                    Token = token
                },
                IsSuccess = true,
                StatusCode = 201,
                Message = $"User registered & Email Sent to {user.Email} successfully! Please login!"
            };
        }


        public async Task<ApiResponse<LoginOtpResponse>> GetOtpByLoginAsync(LoginUser loginUser)
        {
            var user = await _userManager.FindByNameAsync(loginUser.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, loginUser.Password))
            {
                return new ApiResponse<LoginOtpResponse>
                {
                    IsSuccess = false,
                    StatusCode = 401,
                    Message = "Invalid credentials."
                };
            }

            if (!user.TwoFactorEnabled)
            {
                return new ApiResponse<LoginOtpResponse>
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = "Two-factor authentication is not enabled for this account."
                };
            }

            var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

            return new ApiResponse<LoginOtpResponse>
            {
                Response = new LoginOtpResponse
                {
                    User = user,
                    Token = token,
                    IsTwoFactorEnable = true
                },
                IsSuccess = true,
                StatusCode = 200,
                Message = $"We have sent an OTP to your Email {user.Email}"
            };
        }

        public async Task<ApiResponse<LoginResponse>> GetJwtTokenAsync(AppUser user)
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

            var jwtToken = GetToken(authClaims); //access token

            var refreshToken = GenerateRefreshToken();
            _ = int.TryParse(_configuration["JWT:RefreshTokenValidity"], out int refreshTokenValidity);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry= DateTime.UtcNow.AddDays(refreshTokenValidity);

            await _userManager.UpdateAsync(user);

            return new ApiResponse<LoginResponse>
            {
                Response = new LoginResponse()
                {
                    AccessToken = new TokenType()
                    {
                        Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                        ExpiryTokenDate = jwtToken.ValidTo
                    },
                    RefreshToken = new TokenType()
                    {
                        Token = user.RefreshToken,
                        ExpiryTokenDate = user.RefreshTokenExpiry
                    }
                },
                IsSuccess = true,
                StatusCode = 200,
                Message = $"Token successfully generated"
            };
        }

        public async Task<ApiResponse<LoginResponse>> LoginUserWithJWTTokenAsync(string otp, string userName)
        {
            var user = await _userManager.FindByEmailAsync(userName);

            if (user == null || !await _userManager.GetTwoFactorEnabledAsync(user))
            {
                return new ApiResponse<LoginResponse>
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    Message = "User not found or 2FA not enabled"
                };
            }

            var isTokenValid = await _userManager.VerifyTwoFactorTokenAsync(user, "Email", otp);

            if (isTokenValid)
            {
                return await GetJwtTokenAsync(user);
            }

            return new ApiResponse<LoginResponse>
            {
                IsSuccess = false,
                StatusCode = 404,
                Message = "Invalid OTP"
            };
        }

        public async Task<ApiResponse<LoginResponse>> RenewAccessTokenAsync(LoginResponse tokens)
        {
            var accessToken = tokens.AccessToken;
            var refreshToken = tokens.RefreshToken;
            var principal = GetClaimsPrincipal(accessToken.Token);
            var user = await _userManager.FindByNameAsync(principal.Identity.Name);

            if (refreshToken.Token != user.RefreshToken && refreshToken.ExpiryTokenDate <= DateTime.Now)
            {
                return new ApiResponse<LoginResponse>
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = $"Token invalid or expired"
                };
            }
            var response = await GetJwtTokenAsync(user);
            return response;
        }
        #region PrivateMethods
        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            _ = int.TryParse(_configuration["JWT:TokenValidityInMinutes"], out int tokenValidityInMinutes);

            var expirationTimeUtc = DateTime.UtcNow.AddMinutes(tokenValidityInMinutes);
            var localTimeZone = TimeZoneInfo.Local;
            var expirationTimeInLocalTimeZone = TimeZoneInfo.ConvertTimeFromUtc(expirationTimeUtc, localTimeZone);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: expirationTimeInLocalTimeZone,
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new Byte[64];
            var range = RandomNumberGenerator.Create();
            range.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }

        private ClaimsPrincipal GetClaimsPrincipal(string accessToken)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out SecurityToken securityToken);

            return principal;
        }
        #endregion
    }
}
