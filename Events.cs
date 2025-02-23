public struct ApiErrorEvent : IEvent
{
    public ErrorMessage ErrorMessage { get; private set; }
    public ApiErrorEvent(ErrorMessage errorMessage)
    {
        ErrorMessage = errorMessage;
    }
}