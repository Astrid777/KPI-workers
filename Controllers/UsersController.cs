using CustomIdentityApp.Models;
using CustomIdentityApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.Extensions.Configuration;

namespace CustomIdentityApp.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        UserManager<User> _userManager;
        RoleManager<IdentityRole> _roleManager;
        public IConfiguration _AppConfiguration;
        private readonly ApplicationContext _context;

        public UsersController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ApplicationContext context, IConfiguration AppConfiguration)
        {
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var uvmList = new List<UsersViewModel>();
            string role = "";

            if (User.IsInRole("Администратор"))
            {
                //список всех пользователей
                var users = _context.Users.Include(i => i.Department).ToList();

                foreach (var user in users)
                {
                    if (user.Email != "admin@mail.ru")
                    {
                        role = "";
                        role = _userManager.GetRolesAsync(user).Result.FirstOrDefault();

                        //string departmentName = "";

                        UsersViewModel uvm = new UsersViewModel
                        {
                            Id = user.Id,
                            Name = user.Name,
                            Email = user.Email,
                            Position = user.Position,
                            Department = user.Department.Name,
                            Role = role
                        };
                        uvmList.Add(uvm);
                    }
                }

            }

            if (User.IsInRole("Модератор"))
            {
                User currentUser = _context.Users.Include(i => i.Department).Where(w => w.Email == User.Identity.Name).FirstOrDefault();

                var depUser = currentUser.Department;

                //список всех пользователей по отделу
                var users = _context.Users.Include(i => i.Department).Where(w => w.DepartmentId == depUser.Id).ToList();

                foreach (var user in users)
                {
                    role = "";
                    role = _userManager.GetRolesAsync(user).Result.FirstOrDefault();

                    //string departmentName = "";

                    var userRaitings = _context.Ratings.Where(w => w.UserId == user.Id).Select(s => s.Value).ToList();

                    int sum = 0;

                    foreach (var item in userRaitings)
                        sum = sum + item;

                    if (role == "Пользователь")
                    {
                        UsersViewModel uvm = new UsersViewModel
                        {
                            Id = user.Id,
                            Name = user.Name,
                            Email = user.Email,
                            Position = user.Position,
                            Department = user.Department.Name,
                            Role = role,
                            SummPoints = sum,
                            IsFamiliarized = user.IsFamiliarized

                        };
                        uvmList.Add(uvm);
                    }
                }
            }

            return View(uvmList);
        }


        public IActionResult Create()
        {
            //все отделы
            if (User.IsInRole("Администратор"))
                ViewBag.Departments = new SelectList(_context.Department.ToList(), "Id", "Name");
            else
            {
                //только отдел модератора
                User user = _context.Users.Include(i => i.Department).Where(w => w.UserName == User.Identity.Name).FirstOrDefault();
                ViewBag.Departments = new SelectList(new List<Department>() { user.Department }, "Id", "Name");
            }

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User
                {
                    EmailConfirmed = false,
                    DepartmentId = model.DepartmentId,
                    Position = model.Position,
                    Email = model.Email,
                    UserName = model.Email,
                    Name = model.Name,  
                };

                var result = await _userManager.CreateAsync(user, model.Password);


                if (result.Succeeded)
                {
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

                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            EditUserViewModel model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Position = user.Position,
                DepartmentId = user.DepartmentId
            };

            ViewBag.Departments = new SelectList(_context.Department, "Id", "Name");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    user.Email = model.Email;
                    user.UserName = model.Email;
                    user.Name = model.Name;

                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return View(model);
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            User user = await _userManager.FindByIdAsync(id);

            //сначала удаляем оценки, связанные с пользователем
            var indicatorsToRemove = _context.Ratings.Where(w => w.UserId == user.Id);
            _context.RemoveRange(indicatorsToRemove);

            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
            }

            return RedirectToAction("Index");
        }

        //Смена пароля
        public async Task<IActionResult> ChangePassword(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ChangePasswordViewModel model = new ChangePasswordViewModel { Id = user.Id, Email = user.Email };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    IdentityResult result =
                        await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
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
                    ModelState.AddModelError(string.Empty, "Пользователь не найден");
                }
            }
            return View(model);
        }

        //GET
        public IActionResult PersonalArea(int? id)
        {
            User user = _context.Users.Include(i => i.Department).Where(w => w.UserName == User.Identity.Name).FirstOrDefault();
            ViewBag.UserId = user.Id;
            ViewBag.UserPosition = user.Position;

            ViewBag.UserPersonalName = "";
            if (user.Name != null)
                ViewBag.UserPersonalName = user.Name;

            ViewBag.UserIsFamiliarized = false;

            if (user.IsFamiliarized == true)
            {
                ViewBag.UserIsFamiliarized = user.IsFamiliarized;
                ViewBag.FamiliarizationTime = user.FamiliarizationTime;
            }

            var userIndicatorsVM = new List<RatingViewModel>();
            ViewBag.UserDepartment = "";

            if (user.Department != null)
                ViewBag.UserDepartment = user.Department.Name;

            //см показатели в рейтинге по пользователю
            var userIndicators = _context.Ratings.Include(i => i.Indicator).Where(w => w.UserId == user.Id).ToList();

            if (userIndicators.Count > 0)
            {
                foreach (var r in userIndicators)
                {
                    userIndicatorsVM.Add(new RatingViewModel
                    {
                        Id = r.Id,
                        IndicatorName = r.Indicator.Name,
                        Value = r.Value
                    });
                }
            }
            return View(userIndicatorsVM);
        }

        [HttpPost]
        public IActionResult PersonalArea()
        {
            User user = _userManager.FindByNameAsync(User.Identity.Name).Result;

            user.IsFamiliarized = true;
            user.FamiliarizationTime = DateTime.Now;
            _context.Users.Update(user);

            _context.SaveChanges();

            return Content(user.Email);
        }

        public ActionResult Export()
        {
            using (XLWorkbook workbook = new XLWorkbook(XLEventTracking.Disabled))
            {
                var worksheet = workbook.Worksheets.Add("Оценки");

                worksheet.Cell("A1").Value = "№";
                worksheet.Cell("B1").Value = "Почта";
                worksheet.Cell("C1").Value = "ФИО";
                worksheet.Cell("D1").Value = "Отдел";
                worksheet.Cell("E1").Value = "Должность";
                worksheet.Cell("F1").Value = "Баллы";

                worksheet.Row(1).Style.Font.Bold = true;


                if (User.IsInRole("Модератор"))
                {
                    User currentUser = _context.Users.Include(i => i.Department).Where(w => w.Email == User.Identity.Name).FirstOrDefault();

                    var depUser = currentUser.Department;

                    //список всех пользователей по отделу
                    var users = _context.Users.Include(i => i.Department).Where(w => w.DepartmentId == depUser.Id).ToList();

                    for (int i=0; i<users.Count; i++)
                    {
                        string role = "";
                        role = _userManager.GetRolesAsync(users[i]).Result.FirstOrDefault();

                        var userRaitings = _context.Ratings.Where(w => w.UserId == users[i].Id).Select(s => s.Value).ToList();

                        int sum = 0;

                        foreach (var item in userRaitings)
                            sum = sum + item;

                        if (role == "Пользователь")
                        {
                            worksheet.Cell(i + 2, 1).Value = i;
                            worksheet.Cell(i + 2, 2).Value = users[i].Email;
                            worksheet.Cell(i + 2, 3).Value = users[i].Name;
                            worksheet.Cell(i + 2, 4).Value = users[i].Department.Name;
                            worksheet.Cell(i + 2, 5).Value = users[i].Position;
                            worksheet.Cell(i + 2, 6).Value = sum;
                        }
                    }
                }


                //нумерация строк/столбцов начинается с индекса 1 (не 0)
                
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Flush();

                    return new FileContentResult(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = $"Показатели_{DateTime.UtcNow.ToShortDateString()}.xlsx"
                    };
                }
            }
        }


    }
}