using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardsGame.ClientServer.Http.Response;
using MonsterTradingCardsGame.DBconn;
using MonsterTradingCardsGame.DBconn.Tables;
using Newtonsoft.Json;

namespace MonsterTradingCardsGame.ClientServer
{
    internal class RequestHandler
    {
        private DB _db = new DB();
        public HttpResponse Response;

        // kleinere methoden

        public HttpResponse HandleRequest(Http.Request.HttpRequest? request)
        {
            
            _db.Connect();
            if (request == null)
            {
                //return _db.CreateHttpResponse(HttpStatusCode.BadRequest, "Request failed");
            }
            switch (request.Header.Url)
            {
                case "/users":
                    Response = _db.UserRoute(request);
                    break;
                case "/sessions":
                    // login
                    Response = _db.SessionRoute(request);
                    break;
                case "/packages":
                    // Create packages
            
                    //Response = _dbPkg.CreatePackage(request.Body.Data, _db.Conn);
                    break;
                case "/transactions":
                    if (request.Header.Url.Split()[1] == "packages")
                    {
                        // Aqcuire packages
                        //_db.AddCardToStack(request.Header.AuthKey.Split('-')[0], "s", _db.Conn);
                        break;
                    }
                    //Response = "afjlsk";
                    break;
                case "/cards":
                    // show cards (stack)
                    //Response = "AKJFdb";
                    break;
                case "/deck":
                    if (request.Body != null && request.Body.Data != null)
                    {
                        //Response = "alfkjdns"; // configure deck
                        break;
                    }
                    var data = request.Header.Url.Split()[0];
                    //Response = data.Split('?').Length == 1 ?
                        // Show Deck
                        //"akfjdsb" : "aflkds"; // Show Different Config
                    break;
                case "/stats":
                    // show Stats
                    //Response = _dbUser.UserStats(request.Header.AuthKey.Split('-')[0], _db.Conn);
                    break;
                case "/score":
                    //Response = "ajklfdb";
                    break;
                case "/tradings":
                    switch (request.Header.Method)
                    {
                        case "/GET":
                            // check Trading deals
                            //Response = "aöfs";
                            break;
                        case "/POST":
                            if (request.Header.Url.Length == 1)
                            {
                                // Create Trading deal
                                break;
                            }
                            // Trade (check for self trade, invalid user, invalid card)
                            //Response = "afsdjk";
                            break;
                        case "/DELETE":
                            //Response = "afsdlk";
                            break;
                    }
                    //Response = "afdk";
                    break;
                default:
                    //Response = "Invalid request";
                    break;
            }
            _db.Disconnect();
            return Response;
        }
    }
}
