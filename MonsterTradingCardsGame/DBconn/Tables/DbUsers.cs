using System.Net;
using MonsterTradingCardsGame.ClientServer.Http.Response;
using Npgsql;

namespace MonsterTradingCardsGame.DBconn.Tables
{
    internal class DbUsers : DbHandler
    {
        public new static HttpResponse CreateHttpResponse(HttpStatusCode status, string body)
        {
            return new HttpResponse
            {
                Header = new ClientServer.Http.Response.HttpResponseHeader(status, "text/plain", body.Length),
                Body = new HttpResponseBody(body)
            };
        }

        public HttpResponse RegisterUser(string username, string password, NpgsqlConnection conn)
        {
            try
            {
                return ExecNonQuery(Sql.Commands["RegisterUser"], new[,]{ { "user", username }, { "pw", password } }, conn) 
                    ? CreateHttpResponse(HttpStatusCode.Created, "User has been registered") 
                    : CreateHttpResponse(HttpStatusCode.InternalServerError, "Problem occurred adding user!");
            }
            catch (Exception e)
            {
                if (e.Message.Split(':')[0] == "23505") // unique = user already there
                {
                    //throw new Exception("POST Duplicate");
                    return CreateHttpResponse(HttpStatusCode.Conflict, "User with that username already exists!");
                }
                throw;
            }
        }

        public HttpResponse LoginUser(string username, string password, NpgsqlConnection conn)
        {
            try
            {
                var response = ExecQuery(Sql.Commands["LoginUser"], 0,null, new[,] { { "user", username }, { "pw", password } }, conn, false);
                return response.Item1
                    ? CreateHttpResponse(HttpStatusCode.OK, $"Login successful!{Environment.NewLine}Welcome to MTCG, {username}! Your token: {username}-mtcgToken")
                    : CreateHttpResponse(HttpStatusCode.Unauthorized, "Login Failed!");
            }
            catch (Exception e)
            {
                return CreateHttpResponse(HttpStatusCode.Unauthorized, "Login Failed! Error: " + e.Message);
            }
        }
        
        public HttpResponse UpdateUser(string username, string name2, string bio, string img, NpgsqlConnection conn)
        {
            try
            {
                return ExecNonQuery(Sql.Commands["UpdateUser"], new [,] { { "newname", name2 }, { "user", username }, { "bio", bio }, { "img", img } }, conn) 
                    ? CreateHttpResponse(HttpStatusCode.OK, "Update successful!")
                    : CreateHttpResponse(HttpStatusCode.Conflict, "Update failed!");
            }
            catch (Exception e)
            {
                return CreateHttpResponse(HttpStatusCode.Conflict, "Update failed! Error: " + e.Message);
            }
        }

        public bool UseCoins(string username, int coinToPay, NpgsqlConnection conn)
        {
            try
            {
                return ExecNonQuery(Sql.Commands["UseCoins"], new [,] { { "user", username }, { "ctp", coinToPay.ToString() } }, conn);
            }
            catch (Exception e)
            {
                throw new Exception("An error occured while trying to use coins! " + e.Message);
            }
        }

        public bool HasEnoughCoins(string username, NpgsqlConnection conn)
        {
            using var command = new NpgsqlCommand(Sql.Commands["HasEnoughCoins"], conn);
            command.Parameters.AddWithValue("@user", username);
            command.Prepare();
            try
            {
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var coins = reader.GetInt32(0);
                    if (coins >= 5)
                    {
                        reader.Close();
                        return true;
                    }

                }

                reader.Close();
                throw new Exception("Not enough coins!");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new Exception("Error while retrieving coins: " + e.Message);
            }
        }

        public HttpResponse UserStats(string username, NpgsqlConnection conn)
        {
            try
            {
                var response = ExecQuery(Sql.Commands["UserStats"], 1, new []{0}, new[,] { { "user", username } }, conn, true);
                return response.Item1
                    ? CreateHttpResponse(HttpStatusCode.OK, response.Item2)
                    : CreateHttpResponse(HttpStatusCode.Conflict, "User not found!");
            }
            catch (Exception e)
            {
                return CreateHttpResponse(HttpStatusCode.Conflict, "asfafsafasfafs" + e.Message);
            }
        }

        public HttpResponse UpdateUserStats(string username, int points, NpgsqlConnection conn)
        {
            try
            {
                return ExecNonQuery(Sql.Commands["UpdateUserStats"], new string[,] { { "user", username }, { "pts", points.ToString() } }, conn)
                    ? CreateHttpResponse(HttpStatusCode.OK, "ELO has been changed")
                    : CreateHttpResponse(HttpStatusCode.Conflict, "Problem occurred while changing ELO!");
            }
            catch (Exception e)
            {
                return CreateHttpResponse(HttpStatusCode.Conflict, "Problem occurred while changing ELO!" + Environment.NewLine + "Error: " + e.Message);
            }
        }

        public HttpResponse Scoreboard(NpgsqlConnection conn)
        {
            using var command = new NpgsqlCommand("Select username, elo from users;", conn);
            try
            {
                var reader = command.ExecuteReader();
                var stack = "Username: ELO" + Environment.NewLine;
                while (reader.Read())
                {
                    stack += reader.GetString(0) + ": " + reader.GetInt32(1) + Environment.NewLine;
                }

                reader.Close();
                return CreateHttpResponse(HttpStatusCode.OK, stack);

            }
            catch (Exception e)
            {
                return CreateHttpResponse(HttpStatusCode.Conflict, "agdhfdsfgsdfga" + e.Message); ;
            }
        }

        public HttpResponse GetUserById(string userUrl, string userToken, NpgsqlConnection conn)
        {
            try
            {
                var response = ExecQuery(Sql.Commands["GetUserById"], 7, null, new[,] { { "user", userUrl } }, conn, true);
                return response.Item1
                    ? CreateHttpResponse(HttpStatusCode.OK, $"{userUrl}'s Profile: {Environment.NewLine}{response.Item2}")
                    : CreateHttpResponse(HttpStatusCode.Conflict, "User not found!");
            }
            catch (Exception e)
            {
                return CreateHttpResponse(HttpStatusCode.Unauthorized, "Error: " + e.Message);
            }
        }
    }
}
