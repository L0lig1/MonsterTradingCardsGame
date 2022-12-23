using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardsGame.CardNamespace;

using MonsterTradingCardsGame.ClientServer.Http.Request;
using MonsterTradingCardsGame.ClientServer.Http.Response;
using MonsterTradingCardsGame.DBconn.Tables;
using Npgsql;

namespace MonsterTradingCardsGame.DBconn
{

    // DB.AquirePcks(){
    //     var package = new DbPackages().GetPackage();
    //         StorePackageinStack
    //             Deletepackage
    // }

    public class DB

    {
        //private DbPackages _dbPackages = new DbPackages();
        private readonly DbUsers _dbUser = new DbUsers();

        private const string ConnString = "Server=127.0.0.1;" +
                                          "Username=postgres;" +
                                          "Database=MonterTradingCardGame;" +
                                          "Port=5432;" +
                                          "Password=bruhchungus;" +
                                          "SSLMode=Prefer";

        public NpgsqlConnection? Conn;

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

        
    }
}