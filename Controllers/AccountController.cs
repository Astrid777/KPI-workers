using CustomIdentityApp.Models;
using CustomIdentityApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;

namespace CustomIdentityApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        private ApplicationContext _context;


        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;

            _context = context;
        }


        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.Departments = new SelectList(_context.Department.ToList(), "Id", "Name");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_userManager.FindByEmailAsync(model.Email).Result == null)
                {
                    User user = new User { Email = model.Email, UserName = model.Email, Position = model.Position, Name = model.Name, DepartmentId = model.DepartmentId, IsFamiliarized = false };

                    // добавляем пользователя
                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        //по умолчанию присваиваем ролль пользователя
                        await _userManager.AddToRoleAsync(user, "Пользователь");

                        // генерация токена для пользователя
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                        var callbackUrl = Url.Action(
                            "ConfirmEmail",
                            "Account",
                            new { userId = user.Id, code = code },
                            protocol: HttpContext.Request.Scheme);

                        EmailService emailService = new EmailService();
                        await emailService.SendEmailAsync(model.Email, "Подтвердите почту",
                            $"Подтвердите регистрацию, перейдя по ссылке: <a href='{callbackUrl}'>Ссылка</a>");

                        return Content("Для завершения регистрации проверьте электронную почту и перейдите по ссылке, указанной в письме");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }

                }
                else
                {
                    User user = _userManager.FindByEmailAsync(model.Email).Result;

                    
                    
                    
                    if (user.EmailConfirmed == false)
                    {
                        user.DepartmentId = model.DepartmentId;
                        user.Position = model.Position;
                        user.Name = model.Name;
                    
                        await _userManager.UpdateAsync(user);

                        // генерация токена для пользователя
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                        var callbackUrl = Url.Action(
                            "ConfirmEmail",
                            "Account",
                            new { userId = user.Id, code = code },
                            protocol: HttpContext.Request.Scheme);

                        EmailService emailService = new EmailService();
                        await emailService.SendEmailAsync(model.Email, "Подтвердите почту",
                            $"Подтвердите регистрацию, перейдя по ссылке: <a href='{callbackUrl}'>Ссылка</a>");

                        return Content("Для завершения регистрации проверьте электронную почту и перейдите по ссылке, указанной в письме");
                    }
                    else
                        return Content("Пользователь уже есть в системе");

                
                }
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
                return RedirectToAction("Index", "Home");
            else
                return View("Error");
        }

        //Авторизация!
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result =
                    await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    // проверяем, принадлежит ли URL приложению
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // удаляем аутентификационные куки
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
