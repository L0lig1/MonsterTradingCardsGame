using System.Net.Mime;
using Newtonsoft.Json;
using MonsterTradingCardsGame.ClientServer.Http.Request;
using MonsterTradingCardsGame.ClientServer.Http.Response;


namespace MonsterTradingCardsGame.ClientServer.Http
{
    public class HttpParser
    {
        public Request.HttpRequest ParseHttpData(string request)
        {
            string method = string.Empty, httpVersion = string.Empty, url = string.Empty;
            string? authorizationKey = string.Empty, user = string.Empty, authorizationType = string.Empty;
            dynamic? data = null;
            var headerOver = false;
            request = request.Trim();
            var requestSplit = request.Split(Environment.NewLine);
            try
            {
                foreach (var line in requestSplit)
                {
                    if (!line.Contains(':') && !string.IsNullOrEmpty(line) && !headerOver)
                    {
                        httpVersion = GetFirstLine(line, "HTTP/")[1];
                        method = GetFirstLine(line, " ")[0];
                        url = GetFirstLine(line, " ")[1];
                    } 
                    else if (headerOver || line.Contains('{') || line.Contains('}') || line.Contains('['))
                    {
                        // check for invalid JSON
                        data = GetData(line);
                    }
                    else if (line == string.Empty)
                    {
                        headerOver = true;
                    }
                    else if (!headerOver && line.Contains("Authorization: "))
                    {
                        authorizationType = GetAuthType(line);
                        authorizationKey = GetAuthKey(line);
                        user = GetAuthKey(line).Split('-')[0];
                    }
                }
                return new HttpRequest
                {
                    Header = new HttpRequestHeader(httpVersion, method, url, authorizationType, authorizationKey, user),
                    Body = new HttpRequestBody(data),
                    IsValid = true
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new HttpRequest
                {
                    Header = new HttpRequestHeader(httpVersion, method, url, authorizationType, authorizationKey, user),
                    Body = new HttpRequestBody(data),
                    IsValid = false
                };
            }
        }

        private static string[] GetFirstLine(string line, string delim)
        {
            return line.Split(delim);
        }
        public static string GetAuthType(string line)
        {
            return line.Split("Authorization: ")[1].Split(" ")[0];
        }
        public static string GetAuthKey(string line)
        {
            return line.Split("Authorization: ")[1].Split(" ")[1];
        }
        public static dynamic GetData(string line)
        {
            return JsonConvert.DeserializeObject<dynamic>(line) ?? throw new InvalidOperationException();
        }
        
    }
}
