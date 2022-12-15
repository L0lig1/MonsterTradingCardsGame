using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardsGame.ClientServer
{
    internal class RequestHandler
    {
        public string HandleRequest(HttpParser? request)
        {
            if (request == null) return "Failed";
            switch (request.Url?[0])
            {
                case "users":
                    if (request.Url.Length == 1)
                        // Register
                        return "afslj";
                    // Edit User Data
                    return "asfk";
                case "sessions":
                    // login
                    return "alkfds";
                case "packages":
                    // Create packages
                    return "aklsf";
                case "transactions":
                    if (request.Url[1] == "packages")
                    {
                        // Aqcuire packages
                    }
                    return "afjlsk";
                case "cards":
                    // show cards (stack)
                    return "AKJFdb";
                case "deck":
                    if (request.Data != null) 
                        return "alfkjdns"; // configure deck
                    var data = request.Url[0];
                    return data.Split('?').Length == 1 ?
                        // Show Deck
                        "akfjdsb" : "aflkds"; // Show Different Config
                case "stats":
                    // show Stats
                    return "afjskb";
                case "score":
                    return "ajklfdb";
                case "tradings":
                    switch (request.Method)
                    {
                        case "GET":
                            // check Trading deals
                            return "aöfs";
                        case "POST":
                            if (request.Url.Length == 1)
                            {
                                // Create Trading deal
                            }
                            // Trade (check for self trade, invalid user, invalid card)
                            return "afsdjk";
                        case "DELETE":
                            return "afsdlk";
                    }
                    return "afdk";
                default:
                    return "Invalid request";
            }
        }
    }
}
