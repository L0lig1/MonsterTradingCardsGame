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
        private readonly Router _router = new ();
        public HttpResponse Response = new();
        private Dictionary<string, string> _authorization = new();

        // kleinere methoden


        public HttpResponse HandleRequest(Http.Request.HttpRequest? request)
        {
            try
            {
                _router.Connect();
                if (request == null)
                {
                    return _router.CreateHttpResponse(HttpStatusCode.BadRequest, "Request failed");
                }

                Console.WriteLine(request.Header.Url.Split('/')[1]);

                if (request.Header.Url.Split('/')[1] != "sessions" && request.Header.Url.Split('/').Length != 2)
                {

                }

                Response = request.Header.Url.Split('/')[1] switch
                {
                    "battles"      => _router.BattleRoute(request),
                    "users"        => _router.UserRoute(request),
                    "sessions"     => _router.SessionRoute(request),
                    "packages"     => _router.PackagesRoute(request),
                    "transactions" => _router.TransactionsRoute(request),
                    "cards"        => _router.CardsRoute(request),
                    "deck"         => _router.DeckRoute(request),
                    "stats"        => _router.StatsRoute(request),
                    "score"        => _router.ScoreRoute(),
                    "tradings"     => _router.TradingsRoute(request),
                    _ => _router.CreateHttpResponse(HttpStatusCode.NotFound, "Invalid request")
                };
                _router.Disconnect();
                return Response;
            }
            catch (Exception e)
            {
                _router.Disconnect();
                return _router.CreateHttpResponse(HttpStatusCode.NotFound, "Invalid request");
            }
        }
    }
}
