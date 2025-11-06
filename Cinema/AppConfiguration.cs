using Cinema.Data;
using Cinema.Models;
using Cinema.Repositories;
using Cinema.Repositories.IRepositories;
using Cinema.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace Cinema
{
    public static class AppConfiguration
    {
        public static void RegisterConfig(this IServiceCollection services, string connection)
        {
            services.AddDbContext<ApplicationDbContext>(option =>
            {
                //option.UseSqlServer(builder.Configuration.GetSection("ConnectionStrings")["DefaultConnection"]);
                //option.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]);
                option.UseSqlServer(connection);
            });

            services.AddIdentity<ApplicationUser, IdentityRole>(option =>
            {
                option.User.RequireUniqueEmail = true;
                option.Password.RequiredLength = 8;
                option.Password.RequireNonAlphanumeric = false;
                option.SignIn.RequireConfirmedEmail = true;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddTransient<IEmailSender, EmailSender>();

            services.AddScoped<IRepository<Category>, Repository<Category>>();
            services.AddScoped<IRepository<Actor>, Repository<Actor>>();
            services.AddScoped<IRepository<Movie>, Repository<Movie>>();
            services.AddScoped<IRepository<ShowTime>, Repository<ShowTime>>();
            services.AddScoped<IRepository<CinemaBranch>, Repository<CinemaBranch>>();
            services.AddScoped<IRepository<CinemaHall>, Repository<CinemaHall>>();
            services.AddScoped<IRepository<ApplicationUserOTP>, Repository<ApplicationUserOTP>>();
        }
    }
}
