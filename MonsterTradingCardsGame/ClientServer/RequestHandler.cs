using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardsGame.DBconn;

namespace MonsterTradingCardsGame.ClientServer
{
    internal class RequestHandler
    {
        private DB _db = new DB();
        public string Response = " ";
        public string HandleRequest(HttpParser? request)
        {
            _db.Connect();
            if (request == null) Response = "Failed";
            switch (request.Url?[0])
            {
                case "users":
                    if (request.Url.Length == 1 && request.Data != null)
                    {
                        Response = _db.RegisterUser(request.Data.Username.ToString(), request.Data.Password.ToString());
                        break;
                    } 
                    if (request.Url.Length > 1 && request.Data != null)
                    {
                        // Edit User Data
                        Response = _db.UpdateUser(request.Url[1], request.Data.Name.ToString(), request.Data.Bio.ToString(), request.Data.Image.ToString());
                        break;
                    }

                    Response = "Failed";
                    break;
                case "sessions":
                    // login
                    Response = _db.LoginUser(request.Data.Username.ToString(), request.Data.Password.ToString());
                    break;
                case "packages":
                    _db.CreatePackage(request.Data);
                    // Create packages
                    Response = "aklsf";
                    break;
                case "transactions":
                    if (request.Url[1] == "packages")
                    {
                        // Aqcuire packages
                        break;
                    }
                    Response = "afjlsk";
                    break;
                case "cards":
                    // show cards (stack)
                    Response = "AKJFdb";
                    break;
                case "deck":
                    if (request.Data != null)
                    {
                        Response = "alfkjdns"; // configure deck
                        break;
                    }
                    var data = request.Url[0];
                    Response = data.Split('?').Length == 1 ?
                        // Show Deck
                        "akfjdsb" : "aflkds"; // Show Different Config
                    break;
                case "stats":
                    // show Stats
                    Response = _db.UserStats(request.AuthorizationUser);
                    break;
                case "score":
                    Response = "ajklfdb";
                    break;
                case "tradings":
                    switch (request.Method)
                    {
                        case "GET":
                            // check Trading deals
                            Response = "aöfs";
                            break;
                        case "POST":
                            if (request.Url.Length == 1)
                            {
                                // Create Trading deal
                                break;
                            }
                            // Trade (check for self trade, invalid user, invalid card)
                            Response = "afsdjk";
                            break;
                        case "DELETE":
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
