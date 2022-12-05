using CustomIdentityApp.Models;  // пространство имен модели User
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace CustomIdentityApp
{
    public class RoleInitializer
    {
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            string adminEmail = "admin@mail.ru";
            string password = "123456";

            if (await roleManager.FindByNameAsync("Администратор") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("Администратор"));
            }

            if (await roleManager.FindByNameAsync("Модератор") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("Модератор"));
            }

            if (await roleManager.FindByNameAsync("Пользователь") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("Пользователь"));
            }

            //создать админа
            if (await userManager.FindByNameAsync(adminEmail) == null)
            {
                User admin = new User { Email = adminEmail, UserName = adminEmail };
                IdentityResult result = await userManager.CreateAsync(admin, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Администратор");
                }
            }
        }
    }
}