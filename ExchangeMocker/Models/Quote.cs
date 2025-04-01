using System.Text.Json.Serialization;

namespace ExchangeMocker.Models
{
    [JsonSerializable(typeof(Quote))]
    public class Quote
    {
        public string Name { get; init; }
        public decimal Price { get; init; }
        public long Timestamp { get; init; } = DateTime.Now.Ticks;
        public Quote(string name)
        {
            const double MINVALUE = 75000;
            const double MAXVALUE = 120000;
            Name = name;
            var rng = new Random();
            Price = (decimal)(rng.NextDouble() * (MAXVALUE - MINVALUE) + MINVALUE);
        } // Quote
    } // class Quote
} // namespace
