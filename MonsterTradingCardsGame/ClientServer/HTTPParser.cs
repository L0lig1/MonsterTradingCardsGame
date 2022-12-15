using Newtonsoft.Json;

namespace MonsterTradingCardsGame.ClientServer
{
    internal class HttpParser
    {
        public string? Method;
        public dynamic? Data;
        public string[]? Url;


        public HttpParser(string request)
        {
            request = request.Trim();
            var requestSplit = request.Split(Environment.NewLine);
            foreach (var line in requestSplit)
            {
                if (!line.Contains(':') && !string.IsNullOrEmpty(line))
                {
                    Method = line.Split(' ')[0];
                    Url = line.Split(' ')[1].Split('/').Skip(1).ToArray(); // Gets Url and seperates by '/'
                } else if (line.Contains('{') && line.Contains('}'))
                {
                    // check for invalid JSON
                    // Array of JSONs doesn't work
                    Data = JsonConvert.DeserializeObject<dynamic>(line);
                }
            }
        }
    }
}
