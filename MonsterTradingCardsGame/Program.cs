using MonsterTradingCardsGame.DbConn;


namespace MonsterTradingCardsGame
{
    internal class Program
    {
        public static void Main()
        {
            var db = new Db();
            db.TruncateAll();
            var server = new Server.Server();
            server.Start();
        }
    }
}

