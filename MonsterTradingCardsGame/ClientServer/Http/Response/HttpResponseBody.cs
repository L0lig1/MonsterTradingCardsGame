
namespace MonsterTradingCardsGame.ClientServer.Http.Response
{
    public class HttpResponseBody
    {
        public string GetHttpBody()
        {
            return Environment.NewLine +
                   $"{Data}" +
                   Environment.NewLine +
                   Environment.NewLine;
        }
        public HttpResponseBody(dynamic data)
        {
            Data = data;
        }


        public dynamic? Data { get; set; }
    }
}
