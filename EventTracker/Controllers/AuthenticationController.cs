using EventTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UMenagmentService.Models;
using UMenagmentService.Models.Authentication.Login;
using UMenagmentService.Models.Authentication.Register;
using UMenagmentService.Service;


namespace EventTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IUserManagement _user;
        private readonly IConfiguration _configuration;

        public AuthenticationController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager, IConfiguration configuration, RoleManager<IdentityRole> roleManager, IEmailService emailService, IUserManagement user)
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

                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message= $"User registered, please verify your email!" , IsSuccess = true });
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Message = tokenResponse.Message, IsSuccess=false});
           

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
            var user = await _userManager.FindByEmailAsync(loginUser.Email);
            if (user.TwoFactorEnabled)
            {
                await _signInManager.SignOutAsync();
                await _signInManager.PasswordSignInAsync(user, loginUser.Password, false, true);
                var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

                var message = new Message(new string[] { user.Email! }, "OTP Confrimation", token);
                _emailService.SendEmail(message);

                return StatusCode(StatusCodes.Status200OK,
                 new Response { Status = "Success", Message = $"We have sent an OTP to your Email {user.Email}" });
            }

            if (user != null && await _userManager.CheckPasswordAsync(user, loginUser.Password))
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

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                    expiration = jwtToken.ValidTo
                });

            }
            return Unauthorized();

        }

        [HttpPost]
        [Route("login-2FA")]
        public async Task<IActionResult> LoginWithOTP(string code, string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            var signIn = await _signInManager.TwoFactorSignInAsync("Email", code, false, false);
            if (signIn.Succeeded)
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

                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                        expiration = jwtToken.ValidTo
                    });

                }
            }
            return StatusCode(StatusCodes.Status404NotFound,
                new Response { Status = "Success", Message = $"Invalid Code" });
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("forgot-pass")]
        public async Task<IActionResult> ForgotPassword([Required]string email)
        {
            var user = await _userManager.FindByNameAsync(email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var forgotPassLink = Url.Action("ResetPassword", "Authentication", new { token, email = user.Email }, Request.Scheme);
                var message = new Message(new string[] { user.Email! }, "Forgot Password link", forgotPassLink!);
                _emailService.SendEmail(message);
                return StatusCode(StatusCodes.Status200OK,
                 new Response { Status = "Success", Message = $"Password Changed link is sent to " +
                 $"{user.Email}. Please verify" });
            }
            return StatusCode(StatusCodes.Status400BadRequest,
                 new Response { Status = "Error", Message = $"Couldnt sent link. Please try again." });

        }

        [HttpGet("reset-pass")]
        public async Task<IActionResult> ResetPassword(string token, string email)
        {
            var model = new ResetPassword {Token = token, Email = email };
           
            return Ok( new{
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

    }


}

