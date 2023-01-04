using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardsGame.ClientServer.Http.Response;
using MonsterTradingCardsGame.DbConn;
using MonsterTradingCardsGame.DbConn.Tables;
using Newtonsoft.Json;

namespace MonsterTradingCardsGame.ClientServer
{
    internal class RequestHandler
    {
        private readonly Router _router = new ();
        public HttpResponse Response = new();
        private Dictionary<string, DateTime> _authorization = new();

        private bool IsAuthorized(string username)
        {
            try
            {
                if (_authorization[username] > DateTime.Now)
                    return _authorization.ContainsKey(username) && _authorization[username] > DateTime.Now;

                _authorization.Remove(username);
                return false;
            }
            catch (Exception e)
            {
                if (e.Message.Contains("The given key")) // when username not in _authorization
                    throw new Exception("User is not authorized! Log in or register!");
                throw new Exception("Authorization failed");
            }
        }

        public HttpResponse HandleRequest(Http.Request.HttpRequest? request, Dictionary<string, DateTime> authorization)
        {
            _authorization = authorization;
            try
            {
                _router.Connect();
                if (request == null)
                {
                    return _router.CreateHttpResponse(HttpStatusCode.BadRequest, "Invalid request");
                }

                var splitUrl = request.Header.Url.Split('/');

                Console.WriteLine(splitUrl[1]);

                if (splitUrl[1] != "sessions" && !(splitUrl[1] == "users" && request.Header.Method == "POST" && splitUrl.Length == 2))
                {
                    if (request.Header.User != null && !IsAuthorized(request.Header.User))
                    {
                        throw new Exception("User is not authorized! Log in or register!");
                    }
                }

                Response = splitUrl[1] switch
                {
                    "battles"      => _router.BattleRoute(request),
                    "users"        => _router.UserRoute(request),
                    "sessions"     => _router.SessionRoute(request, _authorization),
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
                return _router.CreateHttpResponse(HttpStatusCode.Conflict, $"Error: {e.Message}");
            }
        }
    }
}
