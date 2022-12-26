using MonsterTradingCardsGame.ClientServer;
using MonsterTradingCardsGame.DBconn;
using MonsterTradingCardsGame.DBconn.Tables;


namespace MonsterTradingCardsGame
{
    internal class Program
    {
        public static void Main(String[] args)
        {
            var db = new Router();
            db.TruncateAll();
            var server = new Server();
            server.Start();
        }
    }
}

