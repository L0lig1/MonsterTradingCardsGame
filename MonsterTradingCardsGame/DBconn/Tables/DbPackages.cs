
using MonsterTradingCardsGame.ClientServer.Http.Response;
using Npgsql;
using System.Net;

namespace MonsterTradingCardsGame.DBconn.Tables
{
    public class DbPackages
    {
        private readonly DbCards _dbCards = new DbCards();
        public bool DeletePackage(string p_id, NpgsqlConnection Conn)
        {
            using var command = new NpgsqlCommand("DELETE FROM public.packages WHERE p_id = @p_id", Conn);
            command.Parameters.AddWithValue("@p_id", p_id);
            try
            {
                command.ExecuteNonQuery();
                var worked = command.ExecuteNonQuery();
                if (worked == -1)
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

        public List<string> GetPackage(NpgsqlConnection Conn)
        {
            using var command = new NpgsqlCommand("SELECT p_id, card_id FROM packages WHERE p_id = @p_id", Conn);
            command.Parameters.AddWithValue("@p_id", SelectRandomP_id(Conn));
            var package = new List<string>();
            try
            {
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    package.Add(reader.GetString(0) + "@" + reader.GetString(1));
                }

                reader.Close();
                if(package.Count > 0)
                    return package;
                throw new Exception("There are no packages in the store!");
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

        public HttpResponse CreateHttpResponse(HttpStatusCode status, string body)
        {
            return new HttpResponse
            {
                Header = new ClientServer.Http.Response.HttpResponseHeader(status, "text/plain", body.Length),
                Body = new HttpResponseBody(body)
            };
        }

        public HttpResponse CreatePackage(dynamic cards, NpgsqlConnection Conn)
        {

            bool failed;
            var packageId = GetRandomString();

            foreach (var card in cards)
            {
                _dbCards.CreateCard(card.Id.ToString(), card.Name.ToString(), (int)card.Damage, Conn);
                using var command = new NpgsqlCommand("INSERT INTO packages(p_id, card_id) " +
                                                      "VALUES(@p_id, @c_id)", Conn);
                command.Parameters.AddWithValue("@p_id", packageId);
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

                    return CreateHttpResponse(HttpStatusCode.Conflict, "Could not create package!");
                }
            }
            return CreateHttpResponse(HttpStatusCode.Created, $"Package created!");
        }
    }
}
