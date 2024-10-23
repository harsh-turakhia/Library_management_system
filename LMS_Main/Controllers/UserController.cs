using AspNetCoreHero.ToastNotification.Abstractions;
using LMS_Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS_Main.Controllers
{
    public class UserController : Controller
    {
        private readonly INotyfService _notyf;
        private readonly IUser _user;
        public UserController(INotyfService notyf, IUser user)
        {
            _notyf = notyf;
            _user = user;
        }

        [Authorize(Roles = "User")]
        [HttpGet]   
        public IActionResult Index()
        {
            try
            {
                var data = _user.GetHomePageData();
                return View(data);
            }
            catch
            {
                return View();
            }
        }


        [Authorize(Roles = "User")]
        [HttpGet]
        public IActionResult UserSearchHandler(string query)
        {
            try
            {
                var data = _user.UserBookSearchHandler(query);
                return Json(data);
            }
            catch
            {
                _notyf.Warning("Please try again..!");
                return View("Index" , "User");
            }
        }


        [Authorize(Roles = "User")]
        [HttpGet]
        public IActionResult MyBooks()
        {
            try
            {
                var UserId = "";
                if (User.Identity.IsAuthenticated)
                {
                    UserId = User.FindFirst("UserId")?.Value;
                }

                var data = _user.GetMyBooks(UserId);
                return View(data);
            }
            catch
            {
                _notyf.Warning("Please try again..!");
                return View("Index", "User");
            }
        }
    }
}
