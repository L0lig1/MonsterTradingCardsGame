using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardsGame.ClientServer.Http;
using MonsterTradingCardsGame.DBconn;

namespace MonsterTradingCardsGame.ClientServer
{
    internal class RequestHandler
    {
        private DB _db = new DB();
        public string Response = " ";

        // kleinere methoden
        public string HandleRequest(Http.Http? request)
        {
            
            _db.Connect();
            if (request == null) Response = "Failed";
            switch (request.Header.Url)
            {
                case "/users":
                    if (request.Header.Url.Split('/').Length == 2 && request.Body != null && request.Body.Data != null)
                    {
                        Response = _db.RegisterUser(request.Body?.Data?.Username.ToString(), request.Body?.Data?.Password.ToString());
                        break;
                    } 
                    if (request.Header.Url.Split('/').Length > 2 && request.Body != null && request.Body.Data != null)
                    {
                        // Edit User Data
                        Response = _db.UpdateUser(request.Header.Url.Split()[1], request.Body.Data.Name.ToString(), request.Body.Data.Bio.ToString(), request.Body.Data.Image.ToString());
                        break;
                    }

                    Response = "Failed";
                    break;
                case "/sessions":
                    // login
                    Response = _db.LoginUser(request.Body.Data.Username.ToString(), request.Body.Data.Password.ToString());
                    break;
                case "/packages":
                    // Create packages
                    Response = _db.CreatePackage(request.Body.Data);
                    break;
                case "/transactions":
                    if (request.Header.Url.Split()[1] == "packages")
                    {
                        // Aqcuire packages
                        _db.AddCardToStack(request.Header.AuthKey.Split('-')[0], "s", _db.Conn);
                        break;
                    }
                    Response = "afjlsk";
                    break;
                case "/cards":
                    // show cards (stack)
                    Response = "AKJFdb";
                    break;
                case "/deck":
                    if (request.Body != null && request.Body.Data != null)
                    {
                        Response = "alfkjdns"; // configure deck
                        break;
                    }
                    var data = request.Header.Url.Split()[0];
                    Response = data.Split('?').Length == 1 ?
                        // Show Deck
                        "akfjdsb" : "aflkds"; // Show Different Config
                    break;
                case "/stats":
                    // show Stats
                    Response = _db.UserStats(request.Header.AuthKey.Split('-')[0]);
                    break;
                case "/score":
                    Response = "ajklfdb";
                    break;
                case "/tradings":
                    switch (request.Header.Method)
                    {
                        case "/GET":
                            // check Trading deals
                            Response = "aöfs";
                            break;
                        case "/POST":
                            if (request.Header.Url.Length == 1)
                            {
                                // Create Trading deal
                                break;
                            }
                            // Trade (check for self trade, invalid user, invalid card)
                            Response = "afsdjk";
                            break;
                        case "/DELETE":
                            Response = "afsdlk";
                            break;
                    }
                    Response = "afdk";
                    break;
                default:
                    Response = "Invalid request";
                    break;
            }
            _db.Disconnect();
            return Response;
        }
    }
}
