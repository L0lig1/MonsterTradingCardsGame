using Npgsql;


namespace MonsterTradingCardsGame.DbConn
{
    public class Db
    {
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
                if (tables.Any(table => Conn == null || !db.ExecNonQuery($"TRUNCATE TABLE {table}", null)))
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
            try
            {
                Conn?.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

    }
}
