using ExchangeMocker.Models;

namespace ExchangeMocker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("ExchangeMocker starting up");

            var builder = WebApplication.CreateSlimBuilder(args);

            var app = builder.Build();

            var apiGroup = app.MapGroup("/api");
            apiGroup.MapGet("/tickets", (string name) =>
            {
                return Results.Json(new Quote(name));
            });
            app.Run();
        } // void Main
    } // class Program
} // namespace
