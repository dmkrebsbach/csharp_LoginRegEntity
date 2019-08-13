using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using LoginRegEntity.Models;
using System.Linq;
using Microsoft.AspNetCore.Http; // FOR USE OF SESSIONS
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace LoginRegEntity.Controllers
{
    public class HomeController : Controller{
        private MyContext dbContext;

        public HomeController(MyContext context)
        {
            dbContext = context;
        }

        [HttpGet("")]               // GETS Main Registration and Login Page
        public IActionResult Index(){
            return View("Index");
        }

        [HttpPost("register")]
        public IActionResult CreateUser(User user)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use!");
                    return View("Index");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user, user.Password);

                dbContext.Users.Add(user);
                dbContext.SaveChanges();

                HttpContext.Session.SetString("user", user.UserId.ToString());

                return RedirectToAction("Success");
            }
            else
            {
                return View("Index");
            }
        }

        [HttpGet("success")]
        public IActionResult Success()
        {
            return View("Success");
        }

        [HttpPost("login")]
        public IActionResult LoginUser(LoginUser user)
        {
            if(ModelState.IsValid)
            {
                var dbUser = dbContext.Users.FirstOrDefault(u => u.Email == user.loginEmail);
                if(dbUser == null)
                {
                    ModelState.AddModelError("Email", "Email does not exist; please create account");
                    return View("Index");
                }

                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(user, dbUser.Password, user.loginPassword);
                if(result == 0)
                {
                    ModelState.AddModelError("Password", "Password does not match Account on File");
                    return View("Index");
                }
                HttpContext.Session.SetString("user", dbUser.UserId.ToString());

                return RedirectToAction("Success");
            }
            else
            {
                return View("Index");
            }
        }



    }
}