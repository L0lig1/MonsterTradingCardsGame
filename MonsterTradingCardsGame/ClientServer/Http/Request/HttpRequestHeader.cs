
namespace MonsterTradingCardsGame.ClientServer.Http.Request
{
    public class HttpRequestHeader
    {
        private readonly string[] _validHttpMethods = { "GET", "POST", "PUT", "DELETE" };
        private readonly string[] _validAuthTypes = { "Basic" };

        private string? _contentType;
        private string? _authType;
        private string  _method = "def";
        private string  _url = "def";

        public string Method
        {
            get => _method;
            set
            {
                if (_validHttpMethods.Contains(value))
                {
                    _method = value;
                }
                else
                {
                    // throw invalidmethodexception
                }
            }
        }

        public string? AuthType
        {
            get => _authType;
            set
            {
                if (_validAuthTypes.Contains(value))
                {
                    _authType = value;
                }
                else
                {
                    // throw invalidctypeexception
                }
            }
        }

        public string Url
        {
            get => _url;
            set
            {
                //if (Uri.TryCreate(value, UriKind.Absolute, out var uriResult) && uriResult.Scheme == Uri.UriSchemeHttp)
                //{
                    _url = value; // check for valid url
                //}
                //else
                //{
                    // throw invalidurlexception
                //}
            }
        }

        public string HttpVersion { get; set; }

        public string? AuthKey { get; set; }

        public HttpRequestHeader(string httpVersion, string method, string url, string? authType, string? authKey)
        {
            HttpVersion = httpVersion;
            Method = method;
            Url = url;
            AuthType = authType;
            AuthKey = authKey;
        }
    }
}
