using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using User.Management.Service.Model;
using User.Management.Service.Services;
using webapiPOC.Model.Authentication;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace webapiPOC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        public AccountController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager,
            IConfiguration configuration, IEmailService emailService, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _emailService = emailService;
            _signInManager = signInManager;
        }

        [HttpPost]
        public async Task<IActionResult> register([FromBody] signupId signup, string role)
        {
            //checking user
            var userExist = await _userManager.FindByEmailAsync(signup.Email);
            if (userExist != null)
            {
                return StatusCode(StatusCodes.Status403Forbidden, "user already exist!");

            }

            //adding user to database
            IdentityUser user = new IdentityUser()
            {
                Email = signup.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = signup.Username,
                //LockoutEnabled = true,
    
                //we can add this after confirming with user later as well
                TwoFactorEnabled = true
            };
            if (await _roleManager.RoleExistsAsync(role))
            {
                var result = await _userManager.CreateAsync(user, signup.Password);
                if (!result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "user failed to create!");
                }

                //assign role
                await _userManager.AddToRoleAsync(user, role);


                //Add token to verify the email
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var comfirmationLink =
                    Url.Action(nameof(ConfirmEmail), "Account", new { token, email = user.Email }, Request.Scheme);
                var message =
                    new Message(new string[] { user.Email! }, "Confirmation email link", comfirmationLink!);
                _emailService.SendEmail(message);


                return StatusCode(StatusCodes.Status201Created, $"user created and email sent to {user.Email} successfully!");
            }
            else
            {
                return StatusCode(StatusCodes.Status403Forbidden, "role is not there!");
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> login([FromBody] login loginmodel)
        {
            //checking user and password and 2FA
            var user = await _userManager.FindByEmailAsync(loginmodel.Email);

            if (user.TwoFactorEnabled)
            {
                var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

                var message = new Message(new string[] { user.Email! }, "OTP confirmation", token);
                _emailService.SendEmail(message);
                return StatusCode(StatusCodes.Status201Created, $"we have sent otp to {user.Email} successfully!");

            }

            if (user != null && await _userManager.CheckPasswordAsync(user, loginmodel.Password))
            {
                //claimlist creation
                var authClaim = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())

                };

                //add role to the list
                var userRoles = await _userManager.GetRolesAsync(user);
                foreach (var role in userRoles)
                {
                    authClaim.Add(new Claim(ClaimTypes.Role, role));
                }


                //generate token with claims..
                var jwtToken = GetToken(authClaim);

                //returning token
                return Ok(
                    new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                        expiration = jwtToken.ValidTo
                    });

            }

            return Unauthorized();
        }

        [HttpPost]
        [Route("login-2FA")]
        public async Task<IActionResult> loginwithOTP(string code, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var signIn = await _userManager.VerifyTwoFactorTokenAsync(user, "Email", code);
            if (signIn)
            {
                if (user != null)
                {
                    //claimlist creation
                    var authClaim = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())

                };

                    //add role to the list
                    var userRoles = await _userManager.GetRolesAsync(user);
                    foreach (var role in userRoles)
                    {
                        authClaim.Add(new Claim(ClaimTypes.Role, role));
                    }


                    //generate token with claims..
                    var jwtToken = GetToken(authClaim);

                    //returning token
                    return Ok(
                        new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                            expiration = jwtToken.ValidTo
                        });

                }
            }
            return StatusCode(StatusCodes.Status404NotFound, "Invalid Code");

        }

        [HttpPost]
        [Route("changePass")]
        public async Task<IActionResult> changePass([FromBody] changePassModel newPass)
        {
            var user = await _userManager.FindByEmailAsync(newPass.email);
            if (user != null)
            {
                await _userManager.ChangePasswordAsync(user, newPass.currentPass, newPass.newPass);
                return StatusCode(StatusCodes.Status200OK, "password change successfully!");
            }
            return StatusCode(StatusCodes.Status403Forbidden, "user is not there!");
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status200OK, "Email verified successfully!");
                }
            }
            return StatusCode(StatusCodes.Status500InternalServerError, "Error internally!");
        }

        //public async Task<IActionResult> forgetPass()
        //{
        //    _userManager.loc
        //}

        //testing email actionresult

        [HttpGet]
        public IActionResult testEmail()
        {
            var message =
                new Message(new string[] { "rohan.singh184@gmail.com", "mahimagoyal083@gmail.com" }, "test", "<h1>from the webAPIpoc app</h1>");
            _emailService.SendEmail(message);
            return StatusCode(StatusCodes.Status200OK, "Email sent!");
        }



        //this is private method for generating token
        private JwtSecurityToken GetToken(List<Claim> authClaim)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(1),
                claims: authClaim,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );
            return token;
        }
    }
}
