using System.Security.Claims;
using Domain.DomainModels;
using Domain.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<CinemaApplicationUser> _userManager;
    private readonly SignInManager<CinemaApplicationUser> _signInManager;


    public AccountController(UserManager<CinemaApplicationUser> userManager,
        SignInManager<CinemaApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public IActionResult Register()
    {
        var model = new UserRegistrationDto();
        return View(model);
    }

    [HttpPost, AllowAnonymous]
    public async Task<IActionResult> Register(UserRegistrationDto request)
    {
        if (!ModelState.IsValid) return View(request);
        var userCheck = await _userManager.FindByEmailAsync(request.Email ?? string.Empty);
        if (userCheck == null)
        {
            var user = new CinemaApplicationUser
            {
                UserName = request.Email,
                NormalizedUserName = request.Email,
                Email = request.Email,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                ShoppingCart = new ShoppingCart()
            };
            var result = await _userManager.CreateAsync(user, request.Password ?? string.Empty);
            if (result.Succeeded)
            {
                return RedirectToAction("Login");
            }
            else
            {
                if (!result.Errors.Any()) return View(request);
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("message", error.Description);
                }

                return View(request);
            }
        }
        else
        {
            ModelState.AddModelError("message", "Email already exists.");
            return View(request);
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login()
    {
        var model = new UserLoginDto();
        return View(model);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(UserLoginDto model)
    {
        if (!ModelState.IsValid) return View(model);
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user != null && !user.EmailConfirmed)
        {
            ModelState.AddModelError("message", "Email not confirmed yet");
            return View(model);
        }

        if (await _userManager.CheckPasswordAsync(user, model.Password) == false)
        {
            ModelState.AddModelError("message", "Invalid credentials");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, true);

        if (result.Succeeded)
        {
            await _userManager.AddClaimAsync(user, new Claim("UserRole", "Admin"));
            return RedirectToAction("Index", "Home");
        }
        else if (result.IsLockedOut)
        {
            return View("AccountLocked");
        }
        else
        {
            ModelState.AddModelError("message", "Invalid login attempt");
            return View(model);
        }

    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login", "Account");
    }

    public async Task<IActionResult> ChangeUserRole()
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> ChangeUserRole(CinemaApplicationUser user)
    {
        var updatedUser = await _userManager.FindByIdAsync(user.Id);
        updatedUser.Role = user.Role;
        var result = await _userManager.UpdateAsync(updatedUser);
        if (result.Succeeded)
        {
            return RedirectToAction("Index", "Home");
        }
        else
        {
            return View(user);
        }
    }
}