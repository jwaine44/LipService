using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LipService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LipService.Controllers;
    
public class UserController : Controller
{
    private int? uid
    {
        get
        {
            return HttpContext.Session.GetInt32("uid");
        }
    }

    private bool loggedIn
    {
        get
        {
            return uid != null;
        }
    }

    private LipServiceContext _context;

    public UserController(LipServiceContext context)
    {
        _context = context;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        if(loggedIn)
        {
            return RedirectToAction("Dashboard");
        }
        
        return View("Index");
    }

    [HttpPost("/register")]
    public IActionResult Register(User newUser)
    {
        if(ModelState.IsValid)
        {
            if(_context.Users.Any(user => user.Email == newUser.Email))
            {
                ModelState.AddModelError("Email", "Already exists!");
            }
        }

        if(ModelState.IsValid == false)
        {
            return Index();
        }
        
        PasswordHasher<User> hasher = new PasswordHasher<User>();
        newUser.Password = hasher.HashPassword(newUser, newUser.Password);

        _context.Users.Add(newUser);
        _context.SaveChanges();

        HttpContext.Session.SetInt32("uid", newUser.UserId);
        HttpContext.Session.SetString("Username", newUser.Username);

        return RedirectToAction("Dashboard");
    }

    [HttpPost("/login")]
    public IActionResult Login(LoginUser loginUser)
    {
        if(ModelState.IsValid == false)
        {
            return Index();
        }

        User? dbUser = _context.Users.FirstOrDefault(user => user.Username == loginUser.LoginUsername);

        if(dbUser == null)
        {
            ModelState.AddModelError("LoginUsername", "Invalid entry!");
            return Index();
        }

        PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();
        PasswordVerificationResult result = hasher.VerifyHashedPassword(loginUser, dbUser.Password, loginUser.LoginPassword);

        if(result == 0)
        {
            ModelState.AddModelError("LoginPassword", "Invalid entry!");
            return Index();
        }

        HttpContext.Session.SetInt32("uid", dbUser.UserId);
        HttpContext.Session.SetString("Username", dbUser.Username);
        return RedirectToAction("Dashboard");
    }

    [HttpPost("/logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }

    [HttpGet("/dashboard")]
    public IActionResult Dashboard()
    {
        if(!loggedIn)
        {
            return RedirectToAction("Index");
        }

        return View("Dashboard");
    }
}