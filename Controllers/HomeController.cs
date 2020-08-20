using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using loginReg.Models;
using Microsoft.AspNetCore.Identity; // For Password Hashing
using Microsoft.AspNetCore.Mvc; // For MVC
using Microsoft.AspNetCore.Http; // For Session

namespace loginReg.Controllers {
    public class HomeController : Controller {

        //*********** CONTEXT

        private MyContext _context;

        public HomeController (MyContext context) {
            _context = context;
        }

        //*********** GET Request

        //** PAGES

        public IActionResult Index () {
            return View ();
        }

        [HttpGet ("loginpage")]
        public ViewResult LoginPage () {
            return View ();
        }

        [HttpGet ("success/{Id}")]
        public IActionResult Success (int Id) {
            if (HttpContext.Session.GetInt32 ("UserId") == null) {
                return RedirectToAction ("loginpage");
            } else {
                NewUser name = _context.Users.FirstOrDefault (l => l.UserId == Id);
                int? num = HttpContext.Session.GetInt32 ("UserId");
                Console.WriteLine ($"I AM logged in. My Id => {num}");
                return View (name);
            }
        }

        //** METHODS

        [HttpGet ("logout")]
        public IActionResult Logout () {
            Console.WriteLine ($"I WAS login. My Id => {HttpContext.Session.GetInt32 ("UserId")}");
            HttpContext.Session.SetInt32 ("UserId", 0);
            Console.WriteLine ($"NOW IM out. Id => {HttpContext.Session.GetInt32 ("UserId")}");
            return View ("Index");
        }

        //*********** POST Request

        [HttpPost ("login")]
        public IActionResult Login (LoginUser log) {
            if (ModelState.IsValid) {

                NewUser userInDb = _context.Users.FirstOrDefault (u => u.Email == log.LoginEmail);
                Console.WriteLine (userInDb);
                if (userInDb == null) {
                    ModelState.AddModelError ("LoginEmail", "Invalid Email/Password");
                    return View ("loginpage");
                } else {
                    var hasher = new PasswordHasher<LoginUser> ();
                    var result = hasher.VerifyHashedPassword (log, userInDb.Password, log.LoginPassword);
                    if (result == 0) {
                        ModelState.AddModelError ("LoginPassword", "Invalid Email/Password");
                        return View ("loginpage");
                    } else {
                        HttpContext.Session.SetInt32 ("UserId", userInDb.UserId);
                        return RedirectToAction ("success", new { Id = userInDb.UserId });
                    }
                }
            } else {
                Console.WriteLine (log.LoginEmail);
                return View ("loginpage");
            }
        }

        [HttpPost ("register")]
        public IActionResult Register (NewUser user) {
            if (ModelState.IsValid) {
                if (_context.Users.Any (u => u.Email == user.Email)) {
                    ModelState.AddModelError ("Email", "Email already in use!");
                    return View ("Index");
                } else {
                    PasswordHasher<NewUser> Hasher = new PasswordHasher<NewUser> ();
                    user.Password = Hasher.HashPassword (user, user.Password);
                    _context.Users.Add (user);
                    _context.SaveChanges ();
                    HttpContext.Session.SetInt32 ("UserId", user.UserId);
                    Console.WriteLine ($"User id: {user.UserId}\nFirst Name: {user.FirstName}\nLastName: {user.LastName}\nEmail: {user.Email}\nSessionId: {HttpContext.Session.GetInt32("UserId")}");
                    return RedirectToAction ("success", new { Id = user.UserId });
                }
            } else {
                return View ("Index");
            }
        }
    }
}