using Microsoft.EntityFrameworkCore;
using Mnemosyne.Data;
using Mnemosyne.DataModels;

namespace Mnemosyne.Endpoints
{
    public class DiffHandler(AppDbContext db, Serilog.ILogger log) : HandlerBase(db, log)
    {
        public override async Task<IResult> HandleAsync(object requestParameter)
        {
            if (requestParameter is not DiffRequest diffrq) return Results.BadRequest();

            try
            {
                var quoteA = await _db.Quotes
                    .Where(q => q.Name == diffrq.QuoteA)
                    .OrderByDescending(q => q.TimeStamp)
                    .FirstOrDefaultAsync(q => q.TimeStamp <= diffrq.TargetTime);

                var quoteB = await _db.Quotes
                    .Where(q => q.Name == diffrq.QuoteA)
                    .OrderByDescending(q => q.TimeStamp)
                    .FirstOrDefaultAsync(q => q.TimeStamp <= diffrq.TargetTime);

                if (quoteA is null || quoteB is null)
                {
                    return Results.NoContent();
                }

                var response = new DiffResponse(quoteA, quoteB);
                return Results.Json(response);
            } // try
            catch (Exception ex)
            {
                _log.Error(ex, "Error calculating futures diff: {Message}", ex.Message);
                return Results.StatusCode(StatusCodes.Status500InternalServerError);
            } // catch
        } // HandleAsync
    } // DiffHandler
} // namespace
