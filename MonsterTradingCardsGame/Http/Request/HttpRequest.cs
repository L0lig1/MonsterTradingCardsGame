namespace MonsterTradingCardsGame.ClientServer.Http.Request
{
    public class HttpRequest
    {
        public HttpRequestHeader Header = null!;
        public HttpRequestBody? Body;
        public bool IsValid;
    }
}
