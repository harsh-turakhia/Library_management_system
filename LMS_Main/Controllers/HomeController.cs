using AspNetCoreHero.ToastNotification.Abstractions;
using LMS_Data_Entity.Dto;
using LMS_Repository.Interface;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Library_management_system.Models;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Authorization;

namespace LMS_Main.Controllers
{
    public class HomeController : Controller
    {
        private readonly INotyfService _notyf;
        private readonly IHome _home;
        private readonly IAuth _auth;

        public HomeController(IHome home, INotyfService notyf, IAuth auth)
        {
            _home = home;
            _notyf = notyf;
            _auth = auth;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Login()
        {
            //var loggedIn = "";

            /*if (User.Identity.IsAuthenticated)
            {
                loggedIn =  User.FindFirst(ClaimTypes.Role)?.Value;

                if (loggedIn == null)
                {
                    return View();
                }
                else if (loggedIn == "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }
                else if (loggedIn == "Librarian")
                {
                    return RedirectToAction("Index", "Lib");
                }
                else
                {
                    return RedirectToAction("Index", "User");
                }
            }
            else
            {
                return View();
            }*/

            if (User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return View();
            }

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var user = _auth.authenticateUser(loginDto.Email, loginDto.Password);

                    if (user == null)
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        _notyf.Error("User do not exist");
                        return View();
                    }
                    else
                    {
                        //Claims are key-value pairs that represent user data. They are used in authentication and authorization processes.
                        var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, user.RoleName),
                        new Claim("UserId", user.UserId.ToString()),
                    };


                        // Create claims identity
                        // it indicates that the claims are associated with cookie-based authentication.
                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTime.UtcNow.AddMinutes(15)
                        };

                        // HttpContext.SignInAsync: This method is used to sign in the user and create an authentication cookie.
                        // CookieAuthenticationDefaults.AuthenticationScheme: This specifies the authentication scheme to use, which is cookie-based authentication in this case.
                        // ClaimsPrincipal represents the user and their associated claims.
                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity),
                            authProperties);

                        if (user.RoleName == "Admin")
                        {
                            _notyf.Success("Login Successfull");
                            return RedirectToAction("Index", "Admin");
                        }
                        else if (user.RoleName == "Librarian")
                        {
                            _notyf.Success("Login Successfull");
                            return RedirectToAction("Index", "Lib");
                        }
                        else
                        {
                            _notyf.Success("Login Successfull");
                            return RedirectToAction("Index", "User");
                        }
                    }
                }
                return View(loginDto);
            }
            catch (Exception ex)
            {
                _notyf.Warning("Exception occured");
                return View("Login", loginDto);
            }
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Register(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return View(registerDto);
            }

            try
            {
                var existingUser = _auth.authenticateUser(registerDto.Email, registerDto.Password);

                if (existingUser == null)
                {
                    // Register the user if they do not exist
                    var registrationResult = _home.RegisterUser(registerDto);

                    if (registrationResult)
                    {
                        _notyf.Success("Registration successful! Please log in.");
                        return RedirectToAction("Login", "Home");
                    }
                    else
                    {
                        // User already exists
                        _notyf.Error("Server Error");
                        return View(registerDto);
                    }
                }
                else
                {
                    _notyf.Error("User exists");
                    ModelState.AddModelError("Email", "A user with this email already exists.");
                    return View(registerDto);
                }

            }
            catch (Exception ex)
            {
                _notyf.Warning("Exception occured");
                ModelState.AddModelError("", "An error occurred while processing your request. Please try again.");
                return RedirectToAction("Register", "Home");
            }
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme);
                _notyf.Success("Logout Successfull");
                return RedirectToAction("Login", "Home");
            }
            catch (Exception ex)
            {
                _notyf.Warning("Exception occured");
                return RedirectToAction("Login", "Home");
            }
        }


        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

    }
}
