using CustomIdentityApp.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CustomIdentityApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));


            services.AddTransient<IUserValidator<User>,
                    CustomUserValidator>(serv => new CustomUserValidator());

            // Добавляем сервис валидатора пароля
            services.AddTransient<IPasswordValidator<User>,
                    CustomPasswordValidator>(serv => new CustomPasswordValidator(6));

            services.AddIdentity<User, IdentityRole>()
               .AddEntityFrameworkStores<ApplicationContext>()
               .AddDefaultTokenProviders();


            services.AddControllersWithViews();

        }


        public void Configure(IApplicationBuilder app)
        {


            //app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();    // подключение аутентификации
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}