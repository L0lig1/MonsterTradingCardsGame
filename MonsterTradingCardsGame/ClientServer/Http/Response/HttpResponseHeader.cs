using System.Net;
using System.Reflection;

namespace MonsterTradingCardsGame.ClientServer.Http.Response
{
    public class HttpResponseHeader
    {

        private readonly string _contentType;
        private readonly int _contentLength;
        private HttpStatusCode _statusCode = HttpStatusCode.NotFound;
        private readonly string _version;

        public string GetResponse()
        {
            var enumDisplayStatus = (HttpStatusCode)_statusCode;
            var stringEnum = enumDisplayStatus.ToString();
            return $"HTTP/{_version} {(int)_statusCode} {_statusCode}" + Environment.NewLine +
                   $"Content-Type: {_contentType}" + Environment.NewLine +
                   $"Content-Length: {_contentLength}" + Environment.NewLine +
                   "";
        }

        public HttpStatusCode StatusCode
        {
            get => _statusCode;
            set
            {
                if (Enum.IsDefined(typeof(HttpStatusCode), value))
                {
                    _statusCode = value;
                }
                else
                {
                    // throw invalidstatuscode
                }
            }
        }
        

        public HttpResponseHeader(HttpStatusCode statusCode, string contentType, int contentLength)
        {
            _contentLength = 200;
            _contentType = contentType;
            StatusCode = statusCode;
            _version = "1.1";
        }
    }
}
