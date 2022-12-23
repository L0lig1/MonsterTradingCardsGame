
using Npgsql;

namespace MonsterTradingCardsGame.DBconn.Tables
{
    public class DbPackages
    {

        public bool DeletePackage(string p_id, NpgsqlConnection Conn)
        {
            using var command = new NpgsqlCommand("DELETE FROM public.packages WHERE p_id = @p_id; ", Conn);
            command.Parameters.AddWithValue("@p_id", p_id);
            try
            {
                command.ExecuteNonQuery();
                var worked = command.ExecuteNonQuery();
                if (worked != 1)
                {
                    throw new Exception("Delete unsucceessful");
                }

                return true;
            }
            catch (Exception e)
            {

                throw new Exception("Package could not be found due to the following error: " + Environment.NewLine +
                                    e.Message);
                //throw DuplicateNameException();
            }
        }

        public string SelectRandomP_id(NpgsqlConnection Conn)
        {
            using var command = new NpgsqlCommand("SELECT p_id FROM packages LIMIT 1", Conn);
            try
            {
                var reader = command.ExecuteReader();
                string p_id = " ";
                while (reader.Read())
                {
                    p_id = reader.GetString(0);
                }

                reader.Close();
                return p_id;
            }
            catch (Exception e)
            {

                throw new Exception("Package could not be found due to the following error: " + Environment.NewLine +
                                    e.Message);
                //throw DuplicateNameException();
            }
        }

        public List<string> GetPackage(string p_id, NpgsqlConnection Conn)
        {
            using var command = new NpgsqlCommand("SELECT card_id, p_id FROM packages WHERE p_id = @p_id", Conn);
            command.Parameters.AddWithValue("@p_id", p_id);
            var package = new List<string>();
            try
            {
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    package.Add(reader.GetString(0));
                }

                reader.Close();
                return package;
            }
            catch (Exception e)
            {

                throw new Exception("Package could not be found due to the following error: " + Environment.NewLine +
                                    e.Message);
                //throw DuplicateNameException();
            }
        }


        public string GetRandomString()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();

            for (var i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new string(stringChars);
        }

        public void CreatePackage(dynamic cards, NpgsqlConnection Conn)
        {

            bool failed;
            foreach (var card in cards)
            {

                using var command = new NpgsqlCommand("INSERT INTO packages(p_id, card_id) " +
                                                      "VALUES(@p_id, @c_id)", Conn);
                command.Parameters.AddWithValue("@p_id", GetRandomString());
                command.Parameters.AddWithValue("@c_id", card.Id.ToString());
                try
                {
                    var worked = command.ExecuteNonQuery();
                    failed = worked != 1;
                    if (failed)
                    {
                        throw new Exception("Didn't work lol");
                    }

                }
                catch (Exception e)
                {

                    if (e.Message.Split(':')[0] == "23505")
                        throw new Exception("Package already exists");
                }
            }
        }
    }
}
