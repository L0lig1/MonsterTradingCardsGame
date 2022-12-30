﻿
using MonsterTradingCardsGame.CardNamespace;
using Npgsql;


using MonsterTradingCardsGame.ClientServer.Http.Response;

namespace MonsterTradingCardsGame.DBconn.Tables
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
                if (name.Contains("regular")) return "normal";
                if (name.Contains(type)) return type;
            }

            return new CardTypes().ValidElementTypes[new Random().Next(0, 3)];
        }

        public string GetCardTypeFromName(string name)
        {
            return name.Contains("spell") ? "spell" : "monster";
        }
        

    }
}
