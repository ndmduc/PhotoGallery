using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PhotoGallery.Infrastructure.Services.Abstract;
using PhotoGallery.Infrastructure.Repositories.Abstract;
using PhotoGallery.ViewModels;
using PhotoGallery.Infrastructure.Core;
using PhotoGallery.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PhotoGallery.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly IMembershipService membershipService;

        private readonly IUserRepository userRepo;

        private readonly ILoggingRepository loggingRepo;

        public AccountController(IMembershipService membershipSer, IUserRepository userRe, ILoggingRepository loggingRe)
        {
            this.membershipService = membershipSer;
            this.userRepo = userRe;
            this.loggingRepo = loggingRe;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel user)
        {
            IActionResult result = new ObjectResult(false);
            GenericResult authenticationResult = null;

            try
            {
                MembershipContext userContext = this.membershipService.ValidateUser(user.Username, user.Password);

                if (userContext.User != null)
                {
                    IEnumerable<Role> roles = this.userRepo.GetUserRoles(user.Username);
                    List<Claim> claims = new List<Claim>();
                    foreach (var role in roles)
                    {
                        Claim claim = new Claim(ClaimTypes.Role, "Admin", ClaimValueTypes.String, user.Username);
                        claims.Add(claim);
                    }

                    await HttpContext.Authentication.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)),
                        new Microsoft.AspNetCore.Http.Authentication.AuthenticationProperties { IsPersistent = user.RememberMe });

                    authenticationResult = new GenericResult()
                    {
                        Succeeded = true,
                        Message = "Authentication succeeded"
                    };
                }
                else
                {
                    authenticationResult = new GenericResult()
                    {
                        Succeeded = false,
                        Message = "Authentication failed"
                    };
                }
            }
            catch (Exception ex)
            {
                authenticationResult = new GenericResult()
                {
                    Succeeded = false,
                    Message = ex.Message
                };
                this.loggingRepo.Add(new Error { Message = ex.Message, StackTrace = ex.StackTrace, DateCreated = DateTime.Now });
                this.loggingRepo.Commit();
            }

            result = new ObjectResult(authenticationResult);
            return result;
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.Authentication.SignOutAsync("Cookies");
                return Ok();
            }
            catch (Exception ex)
            {
                this.loggingRepo.Add(new Error() { Message = ex.Message, StackTrace = ex.StackTrace, DateCreated = DateTime.Now });
                this.loggingRepo.Commit();

                return BadRequest();
            }
        }

        [Route("register")]
        [HttpPost]
        public IActionResult Register([FromBody] RegistrationViewModel user)
        {
            IActionResult result = new ObjectResult(false);
            GenericResult registrationResult = null;

            try
            {
                if (ModelState.IsValid)
                {
                    User _user = this.membershipService.CreateUser(user.Username, user.Email, user.Password, new int[] { 1 });
                    if (_user != null)
                    {
                        registrationResult = new GenericResult
                        {
                            Succeeded = true,
                            Message = "Registration succeeded"
                        };
                    }
                }
                else
                {
                    registrationResult = new GenericResult
                    {
                        Succeeded = false,
                        Message = "Invalid fields"
                    };
                }
            }
            catch (Exception ex)
            {
                registrationResult = new GenericResult()
                {
                    Succeeded = false,
                    Message = ex.Message
                };

                this.loggingRepo.Add(new Error() { Message = ex.Message, StackTrace = ex.StackTrace, DateCreated = DateTime.Now });
                this.loggingRepo.Commit();
            }

            result = new ObjectResult(registrationResult);
            return result;
        }
    }
}
