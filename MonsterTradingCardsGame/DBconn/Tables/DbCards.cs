
using Npgsql;


using MonsterTradingCardsGame.ClientServer.Http.Response;

namespace MonsterTradingCardsGame.DBconn.Tables
{
    internal class DbCards : DbHandler
    {
        public bool CreateCard(string id, string name, int dmg, NpgsqlConnection conn)
        {
            try
            {
                return ExecNonQuery(Sql.Commands["CreateCard"], new[,]{ { "name", name }, { "dmg", dmg.ToString() }, { "c_id", id } }, conn) 
                    ? true 
                    : throw new Exception("AJKDBG");
            }
            catch (NpgsqlException e)
            {
                if (e.Message.Split(':')[0] == "23505") // unique = card already there
                {
                    return true;
                }

                return false; // evtl throw
            }
        }
        

    }
}
