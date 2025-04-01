using Serilog;

namespace Hermes
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hermes starting up");

            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();


            builder.Services.AddSerilog(config => config
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                );

            var configuration = builder.Configuration;
            builder.Services.AddSingleton(configuration);

            var app = builder.Build();
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        } // void Main
    } // class Program
} // namespace
