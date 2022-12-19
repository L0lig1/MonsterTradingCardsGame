
namespace MonsterTradingCardsGame.ClientServer.http
{
    public class HttpBody
    {
        public HttpBody(dynamic data)
        {
            Data = data;
        }

        public dynamic? Data { get; set; }
    }
}
