using Hermes.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;

namespace Hermes.Controllers
{
    public class HomeController : Controller
    {
        private readonly Serilog.ILogger _logger;
        private readonly IConfiguration _config;
        private readonly string? _MnemosyneApiKey;

        const string DefaultExchangeUrl = "http://exchangemocker";
        const string DefaultStorageUrl = "http://mnemosyne";

        public HomeController(Serilog.ILogger logger, IConfiguration configuration)
        {
            _logger = logger;
            _config = configuration;
            _MnemosyneApiKey = _config["ApiKey"];
        } // ctor

        public IActionResult Index()
        {
            TempData["ExchangeUrl"] = DefaultExchangeUrl;
            TempData["StorageUrl"] = DefaultStorageUrl;
            return View();
        } // Index

        [HttpPost]
        public async Task<IActionResult> StoreQuote(string exchange, string storage, string name)
        {
            var httpClient = new HttpClient();

            if (string.IsNullOrEmpty(exchange)) exchange = DefaultExchangeUrl;

            if (string.IsNullOrEmpty(_MnemosyneApiKey))
            {
                _logger.Error("No api key stored in the configuration");
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            httpClient.DefaultRequestHeaders.Add("X-Api-Key", _MnemosyneApiKey);

            var receivedData = await 
                httpClient.GetFromJsonAsync<Quote>($"{exchange}/api/tickets?name={name}");

            if (receivedData is null)
            {
                _logger.Error("Error retrieving information on quote {name}", name);
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            var valueToBeSent = JsonContent.Create(receivedData, typeof(Quote));

            var postResponse = await httpClient.PostAsJsonAsync($"{storage}/api/store", valueToBeSent);
            if (postResponse.IsSuccessStatusCode)
            {
                _logger.Debug("Data has been sent to Mnemosyne");
                return Json(receivedData);
            }
            _logger.Error("Data has not been sent.");
            return StatusCode(StatusCodes.Status503ServiceUnavailable);
        } // StoreQuote

        [HttpPost]
        public async Task<IActionResult> GetDifference(string nameA, string nameB, DateTime timestamp)
        {
            var httpClient = new HttpClient();

            if (string.IsNullOrEmpty(_MnemosyneApiKey))
            {
                _logger.Error("No api key stored in the configuration");
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            httpClient.DefaultRequestHeaders.Add("X-Api-Key", _MnemosyneApiKey);

            var diffRq = new DiffRequest()
            {
                QuoteA = nameA,
                QuoteB = nameB,
                TimeStamp = timestamp
            };

            var valueToBeSent = JsonContent.Create(diffRq, typeof(DiffRequest));

            var postResponse = await httpClient.PostAsJsonAsync("http://mnemosyne/api/store", valueToBeSent);

            if (postResponse.IsSuccessStatusCode)
            {
                try
                {
                    var received = await postResponse.Content.ReadFromJsonAsync<DiffResponse>();
                    if (received is not null)
                        return Json(new { result = $"Prices Difference = {received.PriceDifference}" });
                    
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error decerializing Mnemosyne response");
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
            else if (HttpStatusCode.NoContent == postResponse.StatusCode)
            {
                return Json(new { result = "No quotes were found at the given time or before it" });
            }
            else if (HttpStatusCode.InternalServerError == postResponse.StatusCode)
            {
                return Json(new { result = "Quote service reported an error" });
            }
            _logger.Error("Data has not been sent.");
            return StatusCode(StatusCodes.Status503ServiceUnavailable);
        } // GetDifference


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
