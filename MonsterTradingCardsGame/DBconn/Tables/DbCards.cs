using MonsterTradingCardsGame.CardNamespace;
using Npgsql;

namespace MonsterTradingCardsGame.DbConn.Tables
{
    public class DbCards : DbHandler
    {
        public bool CreateCard(string id, string name, int dmg, NpgsqlConnection conn)
        {
            try
            {
                return ExecNonQuery(Sql.Commands["CreateCard"], 
                    new[,]{ 
                        { "name", name }, 
                        { "dmg", dmg.ToString() }, 
                        { "c_id", id }, 
                        { "ct", GetCardTypeFromName(name)},
                        { "et", GetElemTypeFromName(name)}
                    }, conn) 
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

        public string GetElemTypeFromName(string name)
        {
            foreach (var type in new CardTypes().ValidElementTypes)
            {
                name = name.ToLower();
                if (name.Contains("Regular")) return "Normal";
                if (name.Contains(type)) return type;
            }

            return new CardTypes().ValidElementTypes[new Random().Next(0, 3)];
        }

        public string GetCardTypeFromName(string name)
        {
            return name.Contains("Spell") ? "spell" : "monster";
        }
        
    }
}
