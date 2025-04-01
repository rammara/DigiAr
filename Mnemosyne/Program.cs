using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mnemosyne.Data;
using Mnemosyne.DataModels;
using Mnemosyne.Endpoints;
using Mnemosyne.Security;
using Serilog;

namespace Mnemosyne
{
    public class Program
    {
        public static void Main(string[] args)
        {

            Console.WriteLine("Mnemosyne starting up");

            var builder = WebApplication.CreateSlimBuilder(args);

            builder.Services.AddSerilog(config => config
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                );


            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("dev")));

            builder.Services.AddTransient<AuthKeyMiddleware>();
            builder.Services.AddScoped<StoreHandler>();
            builder.Services.AddScoped<DiffHandler>();

            var app = builder.Build();

            var dbApis = app.MapGroup("/api");
            dbApis.MapPost("/store", async (Quote quote, StoreHandler handler) =>
            {
                return await handler.HandleAsync(quote);
            });

            dbApis.MapPost("/getdiff", async (DiffRequest diffRequest, DiffHandler handler) =>
            {
                return await handler.HandleAsync(diffRequest);
            });

            dbApis.MapGet("/echo", ([FromQuery] string value) =>
            {
                var response = value ?? "no parameter given";
                return Results.Ok(response);
            });

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.Migrate();
            }

            app.Run();
        } // void Main
    } // class Program
} // namespace
