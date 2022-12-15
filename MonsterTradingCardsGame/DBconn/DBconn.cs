using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace MonsterTradingCardsGame.DBconn
{
    public class DB
    {
        public NpgsqlConnection? Conn;
        private const string ConnString = "Server=localhost;" + 
                                          "Username=postgres;" + 
                                          "Database=MonterTradingCardGame;" + 
                                          "Port=5432;" + 
                                          "Password=bruhchungus;" + 
                                          "SSLMode=Prefer";

        public void Connect()
        {
            Conn = new NpgsqlConnection(ConnString);
            Console.Out.WriteLine("Opening connection");
            Conn.Open();
        }

        public void SelectAll()
        {
            using var command = new NpgsqlCommand("SELECT * FROM user ", Conn);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine($"Reading from user id: {reader.GetString(0)}");
            }
            reader.Close();
        }
    }
}