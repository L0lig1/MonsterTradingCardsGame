using Newtonsoft.Json;

namespace MonsterTradingCardsGame.ClientServer
{
    internal class HttpParser
    {
        public string? Method;
        public string? AuthorizationUser;
        public string? AuthorizationToken;
        public dynamic? Data;
        public string[]? Url;
        public string? Request;

        public void ParseData()
        {
            Request = Request.Trim();
            var requestSplit = Request.Split(Environment.NewLine);
            foreach (var line in requestSplit)
            {
                if (!line.Contains(':') && !string.IsNullOrEmpty(line))
                {
                    Method = line.Split(' ')[0];
                    Url = line.Split(' ')[1].Split('/').Skip(1).ToArray(); // Gets Url and seperates by '/'
                } 
                else if (line.Contains('{') || line.Contains('}'))
                {
                   
                    // check for invalid JSON
                    // Array of JSONs doesn't work
                    Data = JsonConvert.DeserializeObject<dynamic>(line);
                }
                else if (line.Contains("Authorization: "))
                {
                    AuthorizationUser = line.Split("Authorization: ")[1].Split(" ")[1].Split('-')[0];
                    AuthorizationToken = line.Split("Authorization: ")[1].Split(" ")[1].Split('-')[1];
                }
            }
        }
    }
}
