using System.Net;
using MonsterTradingCardsGame.ClientServer.Http.Response;
using MonsterTradingCardsGame.DbConn;


namespace MonsterTradingCardsGame.ClientServer
{
    public class Authorization
    {
        public DateTime LoggedInUntil;
        public int Tries;

        public Authorization(DateTime loggedInUntil, int tries)
        {
            LoggedInUntil = loggedInUntil;
            Tries = tries;
        }
    }

    public class AuthorizationHandler
    {
        public Dictionary<string, Authorization> _authorization = new();

        private bool IsLoggedIn(string username)
        {
            if(_authorization.ContainsKey(username) && _authorization[username].LoggedInUntil > DateTime.Now)
                return true;
            throw new Exception("User is not Logged in!");
        }

        public bool IsBanned(string username)
        {
            if (_authorization.ContainsKey(username) && _authorization[username].Tries < 3)
                return true;
            throw new Exception("User is permanently banned!");
        }

        public bool AuthorizationNeeded(string[] url, Http.Request.HttpRequest request)
        {
            return url[1] != "sessions" && !(url[1] == "users" && request.Header.Method == "POST" && url.Length == 2);
        }

        public bool IsAuthorized(string? username)
        {
            try
            {
                if (username == null)
                    throw new Exception("No username was provided!");
                if (IsLoggedIn(username) && !IsBanned(username))
                    return true;

                _authorization.Remove(username);
                return false;
            }
            catch (Exception e)
            {
                throw new Exception($"Authorization failed: {e.Message}");
            }
        }
    }

    internal class RequestHandler
    {
        private readonly Router _router = new ();
        public HttpResponse Response = new();
        private AuthorizationHandler _authHandler = new();

        private HttpResponse Route(string url, Http.Request.HttpRequest request)
        {
            return url switch
            {
                "battles" => _router.BattleRoute(request),
                "users" => _router.UserRoute(request),
                "sessions" => _router.SessionRoute(request, _authHandler),
                "packages" => _router.PackagesRoute(request),
                "transactions" => _router.TransactionsRoute(request),
                "cards" => _router.CardsRoute(request),
                "deck" => _router.DeckRoute(request),
                "stats" => _router.StatsRoute(request),
                "score" => _router.ScoreRoute(),
                "tradings" => _router.TradingsRoute(request),
                _ => _router.CreateHttpResponse(HttpStatusCode.NotFound, "Invalid request")
            };
        }

        public HttpResponse HandleRequest(Http.Request.HttpRequest? request, AuthorizationHandler authorization)
        {
            _authHandler = authorization;
            try
            {
                if (request == null)
                {
                    return _router.CreateHttpResponse(HttpStatusCode.BadRequest, "Invalid request");
                }

                var splitUrl = request.Header.Url.Split('/');

                if (_authHandler.AuthorizationNeeded(splitUrl, request))
                {
                    if (!_authHandler.IsAuthorized(request.Header.User))
                    {
                        return _router.CreateHttpResponse(HttpStatusCode.Unauthorized, "Unauthorized");
                    }
                }
                _router.Connect();
                Response = Route(splitUrl[1], request);
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
