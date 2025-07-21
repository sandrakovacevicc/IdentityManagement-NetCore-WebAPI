using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using UserManagement.API.Models;
using UserManagement.Data.Models;
using UserManagment.Service.Models;
using UserManagment.Service.Models.Authentication.Login;
using UserManagment.Service.Models.Authentication.Register;
using UserManagment.Service.Models.Authentication.User;
using UserManagment.Service.Service;


namespace UserManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IUManagement _user;

        public AuthenticationController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IEmailService emailService, IUManagement user)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _user = user;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUser registerRequest)
        {
            var tokenResponse = await _user.CreateUserWithTokenAsync(registerRequest);
            if (tokenResponse.IsSuccess)
            {
                var confirmationLink = Url.Action(nameof(ConfirmEmail), "Authentication", new { tokenResponse.Response.Token, email = registerRequest.Email }, Request.Scheme);
                var message = new Message(new string[] { registerRequest.Email! }, "Confirmation email link", confirmationLink!);
                _emailService.SendEmail(message);
            }

            return StatusCode(
                tokenResponse.StatusCode,
                new Response
                {
                    Message = tokenResponse.Message,
                    IsSuccess = tokenResponse.IsSuccess
                }
            );
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return StatusCode(
                        StatusCodes.Status200OK,
                        new Response { 
                            Message = "Email Verified Successfully",
                            IsSuccess = true
                        }
                    );
                }
            }
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new Response { 
                    Message = "This User doesn't exist!",
                    IsSuccess= false
                });
        }


        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginUser loginModel)
        {
            var loginOtpResponse = await _user.GetOtpByLoginAsync(loginModel);

            if (!loginOtpResponse.IsSuccess || loginOtpResponse.Response == null)
            {
                return StatusCode(
                    loginOtpResponse.StatusCode, 
                    new Response {
                        IsSuccess = loginOtpResponse.IsSuccess,
                        Message = loginOtpResponse.Message
                    }
                );
            }

            var user = loginOtpResponse.Response.User;
            var token = loginOtpResponse.Response.Token;

            if (!user.EmailConfirmed)
            {
                return Unauthorized(
                    new Response
                    {
                        IsSuccess = false,
                        Message = "Please confirm your email adress before login"
                    }
                );
            }

            var message = new Message(new string[] { user.Email! }, "OTP Confirmation", token);
            _emailService.SendEmail(message);

            return StatusCode(
                loginOtpResponse.StatusCode,
                new Response
                {
                    IsSuccess = loginOtpResponse.IsSuccess,
                    Message = loginOtpResponse.Message,
                }
            );
        }


        [HttpPost]
        [Route("login-2FA")]
        public async Task<IActionResult> LoginWithOTP(string code, string username)
        {
            var jwt = await _user.LoginUserWithJWTTokenAsync(code, username);

            if (jwt.IsSuccess)
            {
                return Ok(jwt);
            }
            return StatusCode(
                jwt.StatusCode,
                new Response { 
                    IsSuccess= jwt.IsSuccess,
                    Message = jwt.Message
                }
            );
        }

        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken(LoginResponse tokens)
        {
            var jwt = await _user.RenewAccessTokenAsync(tokens);
            if (jwt.IsSuccess)
            {
                return Ok(jwt);
            }
            return StatusCode(jwt.StatusCode,
                new Response { 
                    IsSuccess= true, 
                    Message = $"Invalid Code" 
                }
            );
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("forgot-pass")]
        public async Task<IActionResult> ForgotPassword([Required] string email)
        {
            var user = await _userManager.FindByNameAsync(email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var forgotPassLink = Url.Action("ResetPassword", "Authentication", new { token, email = user.Email }, Request.Scheme);
                var message = new Message(new string[] { user.Email! }, "Forgot Password link", forgotPassLink!);
                _emailService.SendEmail(message);
                
                return StatusCode(StatusCodes.Status200OK,
                    new Response
                    {
                        Message = $"Password Changed link is sent to {user.Email}. Please verify",
                        IsSuccess = true
                    }
                );
            }
            return StatusCode(StatusCodes.Status400BadRequest,
                 new Response 
                 { 
                     Message = $"Couldnt sent link. Please try again." , 
                     IsSuccess=false
                 }
            );

        }

        [HttpGet("reset-pass")]
        public async Task<IActionResult> ResetPassword(string token, string email)
        {
            var model = new ResetPassword { Token = token, Email = email };

            return Ok(new
            {
                model
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("reset-pass")]
        public async Task<IActionResult> ResetPassword(ResetPassword resetPassword)
        {
            var user = await _userManager.FindByNameAsync(resetPassword.Email);
            if (user != null)
            {
                var resetPass = await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.Password);
                if (!resetPass.Succeeded)
                {
                    var errors = resetPass.Errors.Select(e => e.Description).ToList();
                    return BadRequest(new { Errors = errors });
                }
                return StatusCode(StatusCodes.Status200OK,
                     new Response { IsSuccess= true, Message = "Password has been changed!" });

            }
            return StatusCode(StatusCodes.Status400BadRequest,
                     new Response { IsSuccess= false, Message = $"Couldn't sent link. Please try again." });
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLogin request)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken);

                var user = await _userManager.FindByEmailAsync(payload.Email);
                if (user == null)
                {
                    user = new AppUser
                    {
                        Email = payload.Email,
                        UserName = payload.Email,
                        EmailConfirmed = true
                    };

                    var result = await _userManager.CreateAsync(user);
                    if (!result.Succeeded)
                    {
                        return BadRequest(new { message = "Failed to create user", errors = result.Errors });
                    }

                    if (!await _roleManager.RoleExistsAsync("Client"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Client"));
                    }

                    var roleResult = await _userManager.AddToRoleAsync(user, "Client");
                    if (!roleResult.Succeeded)
                    {
                        return BadRequest(new { message = "Failed to assign role", errors = roleResult.Errors });
                    }
                }

                var tokenResponse = await _user.GetJwtTokenAsync(user);

                if (!tokenResponse.IsSuccess)
                {
                    return BadRequest(new { message = tokenResponse.Message });
                }

                return Ok(tokenResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Invalid Google token", detail = ex.Message });
            }
        }


        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromServices] ITokenBlacklistService blacklistService)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var expUnix = long.Parse(jwtToken.Claims.First(c => c.Type == "exp").Value);
            var expiry = DateTimeOffset.FromUnixTimeSeconds(expUnix) - DateTimeOffset.UtcNow;

            await blacklistService.BlacklistTokenAsync(token, expiry);

            return Ok(new Response
            {
                IsSuccess = true,
                Message = "Token successfully revoked."
            });
        }


    }

}

