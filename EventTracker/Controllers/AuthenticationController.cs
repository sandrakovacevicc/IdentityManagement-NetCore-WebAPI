using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using UserManagement.API.Models;
using UserManagement.Data.Models;
using UserManagment.Service.Models;
using UserManagment.Service.Models.Authentication.Login;
using UserManagment.Service.Models.Authentication.Register;
using UserManagment.Service.Service;


namespace UserManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IUManagement _user;
        private readonly IConfiguration _configuration;

        public AuthenticationController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager, IConfiguration configuration, RoleManager<IdentityRole> roleManager, IEmailService emailService, IUManagement user)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
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

                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = $"User registered, please verify your email!", IsSuccess = true });
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Message = tokenResponse.Message, IsSuccess = false });


        }

        //[HttpGet]
        //public IActionResult TestEmail()
        //{
        //    var mess = new Message(new string[] { "kovacevicsof@gmail.com" }, "Uspeh", "Uspela sam");
        //    _emailService.SendEmail(mess);
        //    return StatusCode(StatusCodes.Status200OK, new Response
        //    {
        //        Status = "Success", Message = "Email sent successfully"
        //    });

        //}

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status200OK,
                      new Response { Status = "Success", Message = "Email Verified Successfully" });
                }
            }
            return StatusCode(StatusCodes.Status500InternalServerError,
                       new Response { Status = "Error", Message = "This User Doesnt exist!" });
        }


        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginUser loginUser)
        {

            var loginOtpResponse = await _user.GetOtpByLoginAsync(loginUser);

            if (loginOtpResponse.Response != null)
            {
                var user = loginOtpResponse.Response.User;

                if (user.TwoFactorEnabled)
                {
                    var token = loginOtpResponse.Response.Token;
                    var message = new Message(new string[] { user.Email! }, "OTP Confrimation", token);
                    _emailService.SendEmail(message);

                    return StatusCode(StatusCodes.Status200OK,
                     new Response { IsSuccess = loginOtpResponse.IsSuccess, Status = "Success", Message = $"We have sent an OTP to your Email {user.Email}" });
                }

                if (user != null && await _userManager.CheckPasswordAsync(user, loginUser.Password))
                {
                    var jwtResponse = await _user.GenerateJwtTokenAsync(user);

                    return Ok(jwtResponse);
                }
            }

            return Unauthorized();

        }

        [HttpPost]
        [Route("login-2FA")]
        public async Task<IActionResult> LoginWithOTP(string code, string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            var signIn = await _signInManager.TwoFactorSignInAsync("Email", code, false, false);
            if (signIn.Succeeded && user != null)
            {
                var jwtResponse = await _user.GenerateJwtTokenAsync(user);
                return Ok(jwtResponse);
            }
            return StatusCode(StatusCodes.Status404NotFound,
                new Response { Status = "Unsuccessfu", Message = $"Invalid Code" });
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
                     Status = "Success",
                     Message = $"Password Changed link is sent to " +
                 $"{user.Email}. Please verify"
                 });
            }
            return StatusCode(StatusCodes.Status400BadRequest,
                 new Response { Status = "Error", Message = $"Couldnt sent link. Please try again." });

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
                    foreach (var error in resetPass.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return Ok(ModelState);
                }
                return StatusCode(StatusCodes.Status200OK,
                     new Response { Status = "Success", Message = "Password has been changed!" });

            }
            return StatusCode(StatusCodes.Status400BadRequest,
                     new Response { Status = "Error", Message = $"Couldnt sent link. Please try again." });
        }

        [HttpPost("google-login")]
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
                }

                var tokenResponse = await _user.GenerateJwtTokenAsync(user);

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

    }

}

