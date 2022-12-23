using MonsterTradingCardsGame.ClientServer.Http.Request;
using MonsterTradingCardsGame.ClientServer.Http.Response;
using MonsterTradingCardsGame.DBconn.Tables;
using Npgsql;
using System.Net;

namespace MonsterTradingCardsGame.DBconn
{

    // DB.AquirePcks(){
    //     var package = new DbPackages().GetPackage();
    //         StorePackageinStack
    //             Deletepackage
    // }

    public class DB

    {
        private DbPackages _dbPackages = new DbPackages();
        private readonly DbUsers _dbUser = new DbUsers();
        private readonly DbStack _dbStack = new DbStack();

        private const string ConnString = "Server=127.0.0.1;" +
                                          "Username=postgres;" +
                                          "Database=MonterTradingCardGame;" +
                                          "Port=5432;" +
                                          "Password=bruhchungus;" +
                                          "SSLMode=Prefer";

        public NpgsqlConnection Conn;

        public HttpResponse HttpResponse;



        public void Connect()
        {
            try
            {
                Conn = new NpgsqlConnection(ConnString);
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
            Conn.Close();
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
            if (request.Header.Url.Split('/').Length == 2 && request.Body != null && request.Body.Data != null)
            {
                return _dbUser.RegisterUser(request.Body?.Data?.Username.ToString(), request.Body?.Data?.Password.ToString(), Conn);
            }
            if (request.Header.Url.Split('/').Length > 2 && request.Body != null && request.Body.Data != null)
            {
                // Edit User Data
                return _dbUser.UpdateUser(
                    request.Header.Url.Split()[1],
                    request.Body.Data.Name.ToString(), 
                    request.Body.Data.Bio.ToString(),
                    request.Body.Data.Image.ToString(),
                    Conn
                );
            }
            return _dbUser.CreateHttpResponse(HttpStatusCode.BadRequest, "Error");
        }

        public HttpResponse SessionRoute(HttpRequest request)
        {
            return _dbUser.LoginUser(request.Body?.Data?.Username.ToString(), request.Body?.Data?.Password.ToString(), Conn);
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
                if (request.Header.Url.Split('/')[2] == "packages")
                {
                    // Aqcuire packages
                    //_db.AddCardToStack(request.Header.AuthKey.Split('-')[0], "s", _db.Conn);

                    if (!_dbUser.HasEnoughCoins(username, Conn))
                    {
                        return CreateHttpResponse(HttpStatusCode.Conflict, "Not enough coins!");
                    }

                    _dbUser.UseCoins(username, 5, Conn);
                    var packages = _dbPackages.GetPackage(Conn);
                    foreach (var package in packages)
                    {
                        var response = _dbStack.AddCardToStack(request.Header.AuthKey?.Split('-')[0],
                            package.Split('@')[1], Conn);
                        if (response.Header.StatusCode != HttpStatusCode.Created)
                        {
                            return response;
                        }
                    }


                    _dbPackages.DeletePackage(packages[0].Split('@')[0], Conn);
                    return CreateHttpResponse(HttpStatusCode.OK,
                        $"Cards added to {request.Header.AuthKey?.Split('-')[0]}'s stack!");
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

                return _dbStack.ShowStack(request.Header.AuthKey?.Split('-')[0], Conn);
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

                return _dbUser.UserStats(request.Header.AuthKey?.Split('-')[0], Conn);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public HttpResponse ScoreRoute(HttpRequest request)
        {
            try
            {

                return _dbUser.Scoreboard(Conn);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}