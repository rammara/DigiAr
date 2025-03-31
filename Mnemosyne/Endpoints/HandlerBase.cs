using Mnemosyne.Data;

namespace Mnemosyne.Endpoints
{
    public abstract class HandlerBase(AppDbContext db, Serilog.ILogger log) : IEndpointRequestHandler
    {
        protected readonly AppDbContext _db = db;
        protected readonly Serilog.ILogger _log = log;

        public abstract Task<IResult> HandleAsync(object requestParameter);
    } // HandlerBase
} // namespace
