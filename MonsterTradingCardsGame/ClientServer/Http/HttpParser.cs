using System.Net.Mime;
using MonsterTradingCardsGame.ClientServer.Http.Request;
using MonsterTradingCardsGame.ClientServer.Http.Response;
using Newtonsoft.Json;

namespace MonsterTradingCardsGame.ClientServer.Http
{
    internal class HttpParser
    {

        public Request.HttpRequest ParseHttpData(string request)
        {
            string method = string.Empty, httpVersion = string.Empty, url = string.Empty;
            string? authorizationKey = string.Empty, authorizationType = string.Empty;
            dynamic? data = null;
            request = request.Trim();
            var requestSplit = request.Split(Environment.NewLine);
            try
            {
                foreach (var line in requestSplit)
                {
                    if (!line.Contains(':') && !string.IsNullOrEmpty(line) && !line.Contains('['))
                    {
                        httpVersion = GetFirstLine(line, "HTTP/")[1];
                        method = GetFirstLine(line, " ")[0];
                        //Url = line.Split(' ')[1].Split('/').Skip(1).ToArray(); // Gets Url and seperates by '/'
                        url = GetFirstLine(line, " ")[1];
                    } 
                    else if (line.Contains('{') || line.Contains('}') || line.Contains('[')) // data
                    {
                        // check for invalid JSON
                        data = GetData(line);
                    }
                    else if (line.Contains("Authorization: "))
                    {
                        //authorizationUser = line.Split("Authorization: ")[1].Split(" ")[1].Split('-')[0];
                        authorizationKey = GetAuthKey(line);
                        authorizationType = GetAuthType(line);
                    }
                }
                return new HttpRequest
                {
                    Header = new HttpRequestHeader(httpVersion, method, url, authorizationType, authorizationKey),
                    Body = new HttpRequestBody(data)
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private string[] GetFirstLine(string line, string delim)
        {
            return line.Split(delim);
        }
        public string GetAuthType(string line)
        {
            return line.Split("Authorization: ")[1].Split(" ")[0];
        }
        public string GetAuthKey(string line)
        {
            return line.Split("Authorization: ")[1].Split(" ")[1];
        }
        public dynamic GetData(string line)
        {
            return JsonConvert.DeserializeObject<dynamic>(line) ?? throw new InvalidOperationException();
        }

        
        
    }
}
