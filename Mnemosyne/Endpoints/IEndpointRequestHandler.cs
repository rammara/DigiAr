namespace Mnemosyne.Endpoints
{
    public interface IEndpointRequestHandler
    {
        Task<IResult> HandleAsync(object requestParameter);
    } // IEndpointRequestHandler
} // namespace
