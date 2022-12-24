using System.Net;
using MonsterTradingCardsGame.ClientServer;
using MonsterTradingCardsGame.ClientServer.Http.Response;
using MonsterTradingCardsGame.DBconn;
using MonsterTradingCardsGame.DBconn.Tables;
using MonsterTradingCardsGame.StoreNamespace;
using Npgsql;
using user;


// for automatic game (deck creation, battle etc), change AUTOMATIC var in Store and User

namespace MonsterTradingCardsGame
{
    internal class Program
    {
        public static void Main(String[] args)
        {
            var server = new Server();
            server.Listen();

            //var d = new DbParent();
            //var db = new DB();
            //db.Connect();
            //d.ExecQuery(d.Sql.Commands["LoginUser"],0, new string[,] { {"user", "kienboec"}, {"pw", "daniel" }}, db.Conn,false);
        }
    }
}

