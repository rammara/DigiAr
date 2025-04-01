namespace Hermes.Models
{
    public class DiffResponse(Quote? quoteA, Quote? quoteB)
    {
        public Quote? QuoteA { get; set; } = quoteA;
        public Quote? QuoteB { get; set; } = quoteB;
        public decimal PriceDifference { get => QuoteA?.Price ?? 0 - QuoteB?.Price ?? 0; }
    } // class DiffResponse
} // namespace
