using MonsterTradingCardsGame.DbConn;


namespace MonsterTradingCardsGame
{
    internal class Program
    {

        public static void Main()
        {
            if (!new DbHandler().ExecNonQuery(new DbCommands().Commands["CreateAllTablesIfNotExists"], null)) return;
            var db = new Db();
            db.TruncateAll();
            var server = new Server.Server();
            server.Start();
        }
    }
}

