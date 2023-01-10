using MonsterTradingCardsGame.ClientServer.Http.Request;
using MonsterTradingCardsGame.ClientServer.Http.Response;
using Npgsql;
using System.Net;
using MonsterTradingCardsGame.Battle;
using MonsterTradingCardsGame.DbConn.Tables;


namespace MonsterTradingCardsGame.DbConn
{

    public class Router
    {
        private readonly DbPackages _dbPackages = new();
        private readonly DbUsers _dbUser = new();
        private readonly DbTradings _dbTradings = new();
        private readonly DbStack _dbStack = new();
        private readonly BattleLobby _game = new();

        private const string Server = "127.0.0.1";
        private const string Username = "postgres";
        private const string Database = "MonterTradingCardGame";
        private const string Port = "5432";
        private const string Password = "bruhchungus";
        private const string SSLMode = "Prefer";

        public NpgsqlConnection? Conn;

        public void TruncateAll()
        {
            try
            {
                var db = new DbHandler();
                Connect();
                string[] tables = { "cards", "packages", "stack", "trading", "users" };
                if (tables.Any(table => Conn == null || !db.ExecNonQuery($"TRUNCATE TABLE {table}", null, Conn)))
                {
                    throw new Exception("PROBLEM TRUNCATING TABLES");
                }

                Console.WriteLine("Successfully Truncated Tables");
                Disconnect();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
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
                return request.Header.Method switch
                {
                    "POST" when splitUrl.Length == 2 && request.Body != null && request.Body.Data != null =>
                        _dbUser.RegisterUser(request.Body?.Data?.Username.ToString(), request.Body?.Data?.Password.ToString(), Conn),
                    "PUT" when splitUrl.Length > 2 && request.Body != null && request.Body.Data != null =>
                        _dbUser.UpdateUser(splitUrl[2], request.Body?.Data?.Name.ToString(), request.Body?.Data?.Bio.ToString(), request.Body?.Data?.Image.ToString(), Conn),
                    "GET" when splitUrl.Length > 2 && request.Body != null && request.Body.Data == null =>
                        _dbUser.GetUserById(splitUrl[2],request.Header.AuthKey?.Split('-')[0] ?? throw new InvalidOperationException(), Conn),
                    _ => throw new Exception("Invalid request!")
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return CreateHttpResponse(HttpStatusCode.BadRequest, "Error: " + e.Message);
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

                    var packages = _dbPackages.GetPackageByRandId(Conn);
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
                if (Conn == null || request.Header?.User == null) throw new Exception("Invalid request");

                var response = _game.StartLobby(Conn, request.Header.User);

                return CreateHttpResponse(HttpStatusCode.OK, response);
            }
            catch (Exception e)
            {
                return CreateHttpResponse(HttpStatusCode.Conflict, e.Message);
            }
        }

    }

}