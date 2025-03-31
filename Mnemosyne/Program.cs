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
            dbApis.MapPost("/store", async (List<Quote> quotes, StoreHandler handler) =>
            {
                return await handler.HandleAsync(quotes);
            });

            dbApis.MapPost("/getdiff", async (DiffRequest diffRequest, DiffHandler handler) => {
                return await handler.HandleAsync(diffRequest);
            });

            dbApis.MapGet("/echo", ([FromQuery] string value) =>
            {
                var response = value ?? "no parameter given";
                return Results.Ok(response);
            });
            app.Run();
        }
    } // Main
} // namespace
