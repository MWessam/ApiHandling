
namespace ApiHandling.Runtime
{
    public interface IEvent{}
    public struct ApiErrorEvent
    {
        public ErrorMessage ErrorMessage { get; private set; }
        public ApiErrorEvent(ErrorMessage errorMessage)
        {
            ErrorMessage = errorMessage;
        }
    }
}