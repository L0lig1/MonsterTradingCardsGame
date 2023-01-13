using System.Net;
using MonsterTradingCardsGame.Battle;
using MonsterTradingCardsGame.Authorization;
using MonsterTradingCardsGame.DbConn.Tables;
using MonsterTradingCardsGame.ClientServer.Http.Request;
using MonsterTradingCardsGame.ClientServer.Http.Response;

namespace MonsterTradingCardsGame.Server
{
    public class Router
    {
        private readonly DbPackages _dbPackages = new();
        private readonly DbUsers _dbUser = new();
        private readonly DbTradings _dbTradings = new();
        private readonly DbStack _dbStack = new();
        private readonly BattleLobby _game = new();

        public HttpResponse Route(string url, HttpRequest request, AuthorizationHandler authHandler)
        {
            return url switch
            {
                "battles"      => BattleRoute(request),
                "users"        => UserRoute(request),
                "sessions"     => SessionRoute(request, authHandler),
                "packages"     => PackagesRoute(request),
                "transactions" => TransactionsRoute(request),
                "cards"        => CardsRoute(request),
                "deck"         => DeckRoute(request),
                "stats"        => StatsRoute(request),
                "score"        => ScoreRoute(),
                "tradings"     => TradingsRoute(request),
                _ => CreateHttpResponse(HttpStatusCode.NotFound, "Invalid request")
            };
        }

        public HttpResponse CreateHttpResponse(HttpStatusCode status, string body)
        {
            return new HttpResponse
            {
                Header = new ClientServer.Http.Response.HttpResponseHeader(status, "text/plain", body.Length),
                Body = new HttpResponseBody(body)
            };
        }

        public HttpResponse UserRoute(HttpRequest request)
        {
            try
            {
                var splitUrl = request.Header.Url.Split('/');
                var userToken = request.Header.AuthKey?.Split('-')[0];
                if (splitUrl.Length > 2 && splitUrl[2] != userToken) 
                    throw new Exception("Unauthorized!");
                return request.Header.Method switch
                {
                    "POST" when splitUrl.Length == 2 && request.Body != null && request.Body.Data != null =>
                        _dbUser.RegisterUser(request.Body?.Data?.Username.ToString(), request.Body?.Data?.Password.ToString()),
                    "PUT" when splitUrl.Length > 2 && request.Body != null && request.Body.Data != null =>
                        _dbUser.UpdateUser(splitUrl[2], request.Body?.Data?.Name.ToString(), request.Body?.Data?.Bio.ToString(), request.Body?.Data?.Image.ToString()),
                    "GET" when splitUrl.Length > 2 && request.Body != null && request.Body.Data == null =>
                        _dbUser.GetUserById(splitUrl[2],request.Header.AuthKey?.Split('-')[0] ?? throw new InvalidOperationException()),
                    _ => throw new Exception("Invalid request!")
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return CreateHttpResponse(HttpStatusCode.BadRequest, "Error: " + e.Message);
            }
        }

        public HttpResponse SessionRoute(HttpRequest request, AuthorizationHandler authHandler)
        {
            var username = request.Body?.Data?.Username.ToString();
            HttpResponse resp = _dbUser.LoginUser(username, request.Body?.Data?.Password.ToString());
            if (authHandler._authorization.ContainsKey(username))
            {
                authHandler._authorization[username].Tries++;
                if (resp.Header.StatusCode == HttpStatusCode.OK && !authHandler.IsBanned(username))
                {
                    authHandler._authorization[username].LoggedInUntil = DateTime.Now.AddMinutes(59);
                    // if username tried already
                }
                
            }
            else
            {
                if (resp.Header.StatusCode == HttpStatusCode.OK)
                { 
                    authHandler._authorization[username] = new Authorization.Authorization(DateTime.Now.AddMinutes(59), 1);
                    // if username tried already
                }
                else
                {
                    authHandler._authorization[username] = new Authorization.Authorization(DateTime.Now, 1);
                }
                // if username tried already
            }
            return resp;
        }

        public HttpResponse PackagesRoute(HttpRequest request)
        {
            try
            {
                if (request.Body?.Data != null)
                {
                    return request.Header.AuthKey?.Split('-')[0] == "admin" 
                        ? _dbPackages.CreatePackage(request.Body?.Data)
                        : CreateHttpResponse(HttpStatusCode.Unauthorized, "Only admins can create packages");
                }
                return CreateHttpResponse(HttpStatusCode.Conflict, "Could not create Package!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e); 
                throw;
            }
        }

        public HttpResponse TransactionsRoute(HttpRequest request)
        {
            try
            {
                var username = request.Header.AuthKey?.Split('-')[0];
                if (request.Header.Url.Split('/')[2] == "packages" && username != null)
                {
                    // Aqcuire packages
                    _dbUser.HasEnoughCoins(username);

                    var packages = _dbPackages.GetPackageByRandId();
                    foreach (var package in packages)
                    {
                        var response = _dbStack.AddCardToStack(username ?? throw new InvalidOperationException(),
                            package.Split('@')[1]);
                        if (response.Header.StatusCode != HttpStatusCode.OK)
                        {
                            return response;
                        }
                    }


                    _dbPackages.DeletePackage(packages[0].Split('@')[0]);
                    _dbUser.UseCoins(username, 5);
                    return CreateHttpResponse(HttpStatusCode.OK, $"Cards added to {username}'s stack!");
                }

            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return CreateHttpResponse(HttpStatusCode.Conflict, "Error occured: " + Environment.NewLine + e.Message); //throw;
            }
            return CreateHttpResponse(HttpStatusCode.Conflict, "Error occured"); //throw;
        }

        public HttpResponse CardsRoute(HttpRequest request)
        {
            try
            {
                return _dbStack.ShowStack(request.Header.AuthKey?.Split('-')[0] ?? throw new InvalidOperationException());

            }
            catch (Exception e)
            {
                return CreateHttpResponse(HttpStatusCode.Conflict, $"Error occured: {e.Message}"); //throw;
            }
        }

        public HttpResponse StatsRoute(HttpRequest request)
        {
            try
            {
                return _dbUser.UserStats(request.Header.AuthKey?.Split('-')[0] ?? throw new InvalidOperationException());
            }
            catch (Exception e)
            {
                return CreateHttpResponse(HttpStatusCode.Conflict, $"Error occured: {e.Message}");
            }
        }

        public HttpResponse ScoreRoute()
        {
            try
            {
                return _dbUser.Scoreboard();
            }
            catch (Exception e)
            {
                return CreateHttpResponse(HttpStatusCode.Conflict, $"Error occured: {e.Message}");
            }
        }

        public HttpResponse TradingsRoute(HttpRequest request)
        {
            try
            {
                switch (request.Header.Method)
                {
                    case "GET":
                        // check Trading deals
                        return _dbTradings.GetTradingDeals();
                    case "POST":
                        if (request.Header.Url == "/tradings")
                        {
                            return _dbTradings.CreateTradingDeal(request.Header.AuthKey?.Split('-')[0], request.Body?.Data);
                        }
                        // Trade (check for self trade, invalid user, invalid card)
                        var subUrl = request.Header.Url.Split('/');
                        if (subUrl.Length == 3 && request.Body != null)
                            return _dbTradings.Trade(subUrl[2], request.Body.Data?.ToString(), request.Header.User);
                        throw new Exception("Invalid request!");
                    case "DELETE":
                        return _dbTradings.DeleteTradingDeal(request.Header.Url.Split('/')[2]);
                }

                throw new Exception("Invalid request!");
            }
            catch (Exception e)
            {
                return CreateHttpResponse(HttpStatusCode.Conflict, $"Error occured: {e.Message}");
            }
        }

        public HttpResponse DeckRoute(HttpRequest request)
        {
            try
            {
                if (request.Body != null && request.Body.Data != null)
                {
                    return _dbStack.ConfigureDeck(request.Header.User, request.Body?.Data); // configure deck
                }

                if (request.Header.User != null) return _dbStack.GetDeckOnlyCardNames(request.Header.User);
                //Response = data.Split('?').Length == 1 ?
                // Show Deck
                //"akfjdsb" : "aflkds"; // Show Different representation
                throw new Exception("Invalid request!");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public HttpResponse BattleRoute(HttpRequest request)
        {
            try
            {
                if (request.Header?.User == null) throw new Exception("Invalid request");

                var response = _game.StartLobby(request.Header.User);

                return CreateHttpResponse(HttpStatusCode.OK, response);
            }
            catch (Exception e)
            {
                return CreateHttpResponse(HttpStatusCode.Conflict, e.Message);
            }
        }

    }
}