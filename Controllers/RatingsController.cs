using CustomIdentityApp.Models;
using CustomIdentityApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomIdentityApp.Controllers
{
    [Authorize(Roles = "Администратор, Модератор")]
    public class RatingsController : Controller
    {
        private readonly ApplicationContext _context;

        public RatingsController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: Marks
        public async Task<IActionResult> Index(string id)
        {
            //получаем пользователя
            User user = _context.Users.Where(w => w.Id == id).FirstOrDefault();

            //показатели отдела
            var indicatorsDepartment = _context.Indicator.Include(i => i.Department).Where(w => w.DepartmentId == user.DepartmentId).ToList();
            var indicatorsInRating = _context.Ratings.Where(w => w.User == user).Select(s => s.Indicator).ToList();

            ViewBag.UserBag = user.Name;

            List<RatingViewModel> lrvm = new List<RatingViewModel>();

            //проверяем, есть ли показатель. Если нет, добавляем в базу
            if (user != null)
            {
                foreach (var i in indicatorsDepartment)
                {
                    if (!indicatorsInRating.Contains(i))
                    {
                        _context.Ratings.Add(new Rating
                        {
                            IndicatorId = i.Id,
                            Indicator = i,
                            UserId = user.Id,
                            User = user,
                            Value = 0
                        });
                    }
                }

                _context.SaveChanges();


                var Rates = _context.Ratings.Where(w => w.UserId == id).ToList();

                foreach (var r in Rates)
                {
                    RatingViewModel rvm = new RatingViewModel()
                    {
                        Id = r.Id,
                        UserId = user.Id,
                        IndicatorId = r.IndicatorId,
                        IndicatorName = r.Indicator.Name,
                        Value = r.Value
                    };

                    lrvm.Add(rvm);
                }
            }
            ViewData["Value"] = new SelectList(new List<string>() { "0", "1", "2", "3" });

            return View(lrvm);
        }

        [HttpPost]
        public async Task<IActionResult> Index(List<RatingViewModel> Rates)
        {
            if (ModelState.IsValid)
            {
                foreach (var r in Rates)
                {
                    //перебираем полученные показатели с представления и ищем их по id
                    Rating ra = _context.Ratings.Include(i => i.Indicator).Where(w => w.Id == r.Id).FirstOrDefault();

                    //обновляем каждое свойство
                    ra.IndicatorId = r.IndicatorId;
                    ra.Indicator = _context.Indicator.Include(i => i.Department).Where(w => w.Id == r.IndicatorId).FirstOrDefault();
                    ra.UserId = r.UserId;
                    ra.User = _context.Users.Find(r.UserId);
                    ra.Value = r.Value;

                    //после обновления сохраняем данные в бд
                    _context.Ratings.Update(ra);
                }
            }

            _context.SaveChanges();

            return RedirectToAction("Index", "Users");
        }
    }
}
