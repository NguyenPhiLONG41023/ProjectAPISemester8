using DataAccess.AutoMapper;
using DataAccess.DataAccess;
using DataAccess.Repository.Interfaces;
using DataAccess.Repository.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace EshopWebApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
			builder.Services.AddDbContext<NewProjectDBContext>(options =>
	            options.UseSqlServer(builder.Configuration.GetConnectionString("DB")));
			//Add session
			builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });

            // Add authentication services
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Auth/Login";
					options.AccessDeniedPath = "/Auth/AccessDenied";
                });

            // Add authorization services
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
            });
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireUserRole", policy => policy.RequireRole("User"));
            });

            builder.Services.AddAutoMapper(typeof(ApplicationMapper));
			builder.Services.AddScoped<IProductRepository, ProductRepository>();
			builder.Services.AddScoped<ProductRepository>();
			builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
			builder.Services.AddScoped<OrderDetailRepository>();
			builder.Services.AddScoped<IOrderRepository, OrderRepository>();
			builder.Services.AddScoped<OrderRepository>();

			var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

			app.UseAuthentication();

			app.UseAuthorization();

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
            );

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );

            app.Run();
        }
    }
}