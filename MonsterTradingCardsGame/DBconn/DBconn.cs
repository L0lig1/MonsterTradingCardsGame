using MonsterTradingCardsGame.ClientServer.Http.Request;
using MonsterTradingCardsGame.ClientServer.Http.Response;
using MonsterTradingCardsGame.DBconn.Tables;
using Npgsql;
using System.Net;
using System.Threading;
using MonsterTradingCardsGame.CardNamespace;
using user;

namespace MonsterTradingCardsGame.DBconn
{

    public class Router

    {
        private readonly DbPackages _dbPackages = new();
        private readonly DbUsers _dbUser = new();
        private readonly DbTradings _dbTradings = new();
        private readonly DbStack _dbStack = new();


        private const string Server = "127.0.0.1";
        private const string Username = "postgres";
        private const string Database = "MonterTradingCardGame";
        private const string Port = "5432";
        private const string Password = "bruhchungus";
        private const string SSLMode = "Prefer";

        public NpgsqlConnection? Conn;

        public void TruncateAll()
        {
            var db = new DbHandler();
            Connect();
            if (
                db.ExecNonQuery("TRUNCATE TABLE cards", null, Conn) &&
                db.ExecNonQuery("TRUNCATE TABLE packages", null, Conn) &&
                db.ExecNonQuery("TRUNCATE TABLE stack", null, Conn) &&
                db.ExecNonQuery("TRUNCATE TABLE trading", null, Conn) &&
                db.ExecNonQuery("TRUNCATE TABLE users", null, Conn))
            {
                Console.WriteLine("ALL TABLES TRUNCATED");
            }
            else
            {
                Console.WriteLine("PROBLEM TRUNCATING TABLES");
            }
            Disconnect();
        }

        public void Connect()
        {
            try
            {
                Conn = new NpgsqlConnection($"Server={Server};" +
                                            $"Username={Username};" +
                                            $"Database={Database};" +
                                            $"Port={Port};" +
                                            $"Password={Password};" +
                                            $"SSLMode={SSLMode}");
                Console.Out.WriteLine("Opening connection");
                Conn.Open();
                Console.WriteLine("Connected!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Connection failed due to the following error: " + Environment.NewLine + e);
                throw;
            }
        }

        public void Disconnect()
        {
            Conn?.Close();
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
                if (splitUrl.Length > 2 && splitUrl[2] != userToken) throw new Exception("Unauthorized!");
                if (splitUrl.Length == 2 && request.Body != null && request.Body.Data != null)
                {
                    return _dbUser.RegisterUser(request.Body?.Data?.Username.ToString(), request.Body?.Data?.Password.ToString(), Conn);
                }
                if (splitUrl.Length > 2 && request.Body != null && request.Body.Data != null)
                {
                    return _dbUser.UpdateUser(splitUrl[2], request.Body?.Data?.Name.ToString(), request.Body?.Data?.Bio.ToString(), request.Body?.Data?.Image.ToString(), Conn );
                }
                if (splitUrl.Length > 2 && request.Body != null && request.Body.Data == null)
                {
                    return _dbUser.GetUserById(splitUrl[2], request.Header.AuthKey?.Split('-')[0] ?? throw new InvalidOperationException(), Conn);
                }
                throw new Exception("Error!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return CreateHttpResponse(HttpStatusCode.BadRequest, "Error");
            }
        }

        public HttpResponse SessionRoute(HttpRequest request, Dictionary<string, DateTime> authorization)
        {
            HttpResponse resp = _dbUser.LoginUser(request.Body?.Data?.Username.ToString(), request.Body?.Data?.Password.ToString(), Conn);
            if (resp.Header.StatusCode == HttpStatusCode.OK)
            {
                authorization.Add(request.Body?.Data?.Username.ToString(), DateTime.Now.AddMinutes(59));
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
                        ? _dbPackages.CreatePackage(request.Body?.Data, Conn) 
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
                if (request.Header.Url.Split('/')[2] == "packages" && username != null && Conn != null)
                {
                    // Aqcuire packages
                    _dbUser.HasEnoughCoins(username, Conn);

                    var packages = _dbPackages.GetPackage(Conn);
                    foreach (var package in packages)
                    {
                        var response = _dbStack.AddCardToStack(username ?? throw new InvalidOperationException(),
                            package.Split('@')[1], Conn);
                        if (response.Header.StatusCode != HttpStatusCode.Created)
                        {
                            return response;
                        }
                    }


                    _dbPackages.DeletePackage(packages[0].Split('@')[0], Conn);
                    _dbUser.UseCoins(username, 5, Conn);
                    return CreateHttpResponse(HttpStatusCode.OK,
                        $"Cards added to {username}'s stack!");
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
                if (Conn != null) return _dbStack.ShowStack(request.Header.AuthKey?.Split('-')[0] ?? throw new InvalidOperationException(), Conn);
                throw new Exception("Db not connected");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public HttpResponse StatsRoute(HttpRequest request)
        {
            try
            {
                if (Conn != null)
                    return _dbUser.UserStats(request.Header.AuthKey?.Split('-')[0] ?? throw new InvalidOperationException(), Conn);
                throw new Exception("DB not conn");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public HttpResponse ScoreRoute()
        {
            try
            {
                if (Conn != null) return _dbUser.Scoreboard(Conn);
                throw new Exception("Db not conn");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public HttpResponse TradingsRoute(HttpRequest request)
        {
            try
            {
                if (Conn == null) throw new Exception("Db not conn");
                switch (request.Header.Method)
                {
                    case "GET":
                        // check Trading deals
                        return _dbTradings.GetTradingDeals(Conn);
                    case "POST":
                        if (request.Header.Url == "/tradings")
                        {
                            return _dbTradings.CreateTradingDeal(request.Header.AuthKey?.Split('-')[0], request.Body?.Data, Conn);
                        }
                        // Trade (check for self trade, invalid user, invalid card)
                        var subUrl = request.Header.Url.Split('/');
                        if (subUrl.Length == 3 && request.Body != null)
                            return _dbTradings.Trade(subUrl[2], request.Body.Data?.ToString(), request.Header.User, Conn);
                        throw new Exception("Invalid request!");
                    case "DELETE":
                        return _dbTradings.DeleteTradingDeal(request.Header.Url.Split('/')[2], Conn);
                }

                throw new Exception("Invalid request!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                return CreateHttpResponse(HttpStatusCode.Conflict, "jlkagdn");
            }
        }

        public HttpResponse DeckRoute(HttpRequest request)
        {
            try
            {
                if (request.Body != null && request.Body.Data != null)
                {
                    return _dbStack.ConfigureDeck(request.Header.User, request.Body?.Data, Conn); // configure deck
                }

                if (request.Header.User != null && Conn != null) return _dbStack.GetDeckOnlyCardNames(request.Header.User, Conn);
                //Response = data.Split('?').Length == 1 ?
                // Show Deck
                //"akfjdsb" : "aflkds"; // Show Different representation
                throw new Exception("adlfkn");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public HttpResponse BattleRoute(HttpRequest request)
        {
            try
            {
                // thread should wait for other thread to come
                // when other thread comes, exe battle
                var celo1 = int.Parse(_dbUser.UserStats("kienboec", Conn).Body?.Data);
                var celo2 = int.Parse(_dbUser.UserStats("altenhof", Conn).Body?.Data);
                
                var battle = new Battle.Battle(new User("kienboec", _dbStack.GetDeck("kienboec", Conn), celo1),
                                               new User("altenhof", _dbStack.GetDeck("altenhof", Conn), celo2));
                var battleLog = battle.Fight();
                Console.WriteLine(battleLog.Item1 + Environment.NewLine + battleLog.Item2 + Environment.NewLine + battleLog.Item3);
                _dbUser.UpdateUserStats("kienboec", battleLog.Item2, Conn);
                _dbUser.UpdateUserStats("altenhof", battleLog.Item3, Conn);
                /*
                 * Battle:
                 * 2 User class
                 * Get Decks 
                 * Fill Deck Class with Cards
                 *
                 */
                // when battle over, give result to each thread
                return CreateHttpResponse(HttpStatusCode.OK, battleLog.Item1);
            }
            catch (Exception e)
            {
                return CreateHttpResponse(HttpStatusCode.Conflict, e.Message);
            }
        }
    }
}