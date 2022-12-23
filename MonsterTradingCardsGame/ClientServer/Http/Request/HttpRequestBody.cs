
namespace MonsterTradingCardsGame.ClientServer.Http.Request
{
    public class HttpRequestBody
    {
        public HttpRequestBody(dynamic data)
        {
            Data = data;
        }

        public dynamic? Data { get; set; }
    }
}
