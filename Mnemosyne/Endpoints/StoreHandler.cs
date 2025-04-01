using Mnemosyne.Data;
using Mnemosyne.DataModels;

namespace Mnemosyne.Endpoints
{
    public class StoreHandler(AppDbContext db, Serilog.ILogger log) : HandlerBase(db, log)
    { 
        public override async Task<IResult> HandleAsync(object requestParameter)
        {
            if (requestParameter is not Quote quote) return Results.BadRequest();
            try
            {
                if (quote.TimeStamp.Kind != DateTimeKind.Utc)
                {
                    quote.TimeStamp = quote.TimeStamp.ToUniversalTime();
                }

                _db.Quotes.Add(quote);
                await _db.SaveChangesAsync();
                return Results.Ok();
            } // try
            catch (ArgumentException argex)
            {
                _log.Error(argex, "Invalid request parameters");
                return Results.BadRequest();
            } // catch ArgumentException
            catch (Exception ex)
            {
                _log.Error(ex, "Error while adding values");
                return Results.StatusCode(StatusCodes.Status500InternalServerError);
            } // catch Exception
        } // HandleAsync
    } // StoreHandler
} // namespace
