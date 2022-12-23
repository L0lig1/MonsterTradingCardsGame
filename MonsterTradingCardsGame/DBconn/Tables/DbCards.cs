
using Npgsql;

namespace MonsterTradingCardsGame.DBconn.Tables
{
    internal class DbCards
    {
        public bool CreateCard(string id, string name, int dmg, NpgsqlConnection Conn)
        {

            using var command = new NpgsqlCommand("INSERT INTO public.cards(name, damage, c_id) " +
                                                  "VALUES(@name, @dmg, @id)", Conn);
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@dmg", dmg);
            command.Parameters.AddWithValue("@id", id);
            try
            {
                var worked = command.ExecuteNonQuery();
                Console.WriteLine(worked == 1 ? "Card has been added" : "Problem occurred adding card!");
                return true;
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
