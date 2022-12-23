using System.Net;
using MonsterTradingCardsGame.ClientServer.Http.Response;
using Npgsql;

namespace MonsterTradingCardsGame.DBconn.Tables
{
    internal class DbUsers
    {
        public HttpResponse CreateHttpResponse(HttpStatusCode status, string body)
        {
            return new HttpResponse
            {
                Header = new ClientServer.Http.Response.HttpResponseHeader(status, "text/plain", body.Length),
                Body = new HttpResponseBody(body)
            };
        }

        public HttpResponse RegisterUser(string username, string password, NpgsqlConnection Conn)
        {
            using var command = new NpgsqlCommand("INSERT INTO public.users(u_id, username, pw, coins, elo) " +
                                                  "VALUES(default, @user, @pw, 20, 100); ", Conn);
            command.Parameters.AddWithValue("@user", username);
            command.Parameters.AddWithValue("@pw", password);
            try
            {
                var worked = command.ExecuteNonQuery();
                if (worked == 1)
                {
                    return CreateHttpResponse(HttpStatusCode.Created, "User has been registered");
                }
                else
                {
                    return CreateHttpResponse(HttpStatusCode.InternalServerError, "Problem occurred adding user!");
                }
            }
            catch (Exception e)
            {
                //return e.Message.Split(Environment.NewLine)[0].ToString();
                if (e.Message.Split(':')[0] == "23505") // unique = user already there
                {
                    //throw new Exception("POST Duplicate");
                    return CreateHttpResponse(HttpStatusCode.Conflict, "User with that username already exists!");
                }
                throw;
            }
        }

        public HttpResponse LoginUser(string username, string password, NpgsqlConnection Conn)
        {
            using var command = new NpgsqlCommand("SELECT username, pw " +
                                                  "FROM users " +
                                                  "WHERE username = @user " +
                                                  "AND pw = @pw; ", Conn);
            command.Parameters.AddWithValue("@user", username);
            command.Parameters.AddWithValue("@pw", password);
            command.Prepare();
            try
            {
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Close();
                    return CreateHttpResponse(HttpStatusCode.OK, $"Welcome to MTCG, {username}!" + Environment.NewLine + "Here is your token: " + Environment.NewLine + $"{username}-mtcgToken");
                }

                reader.Close();
                return CreateHttpResponse(HttpStatusCode.Unauthorized, "Login Failed!");

            }
            catch (Exception e)
            {
                return CreateHttpResponse(HttpStatusCode.Unauthorized, "Login Failed!");
                //throw DuplicateNameException();
            }
        }
        
        // Fix this
        public HttpResponse UpdateUser(string username, string name2, string bio, string img, NpgsqlConnection Conn)
        {
            using var command =
                new NpgsqlCommand(
                    "UPDATE public.users SET bio='adgda', image='@img', second_name='adagklg' WHERE username='Me';", Conn);
            command.Parameters.AddWithValue("@name2", name2);
            command.Parameters.AddWithValue("@user", username);
            command.Parameters.AddWithValue("@bio", bio);
            command.Parameters.AddWithValue("@img", img);

            try
            {
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Close();
                    return CreateHttpResponse(HttpStatusCode.OK, "Update successful!");
                }

                reader.Close();
                return CreateHttpResponse(HttpStatusCode.Conflict, "Update failed!");
            }
            catch (Exception e)
            {
                return CreateHttpResponse(HttpStatusCode.Conflict, "Update failed!");
                //throw DuplicateNameException();
            }
        }

        public bool UseCoins(string username, int coinToPay, NpgsqlConnection Conn)
        {
            using var command = new NpgsqlCommand("UPDATE users SET coins = ((SELECT coins FROM users WHERE username = @user) - @ctp) WHERE username = @user; ", Conn);
            command.Parameters.AddWithValue("@user", username);
            command.Parameters.AddWithValue("@ctp", coinToPay);

            try
            {
                var worked = command.ExecuteNonQuery();
                return worked == 1;
            }
            catch (Exception e)
            {
                
                Console.WriteLine(e.Message);
                return false;
                //throw DuplicateNameException();
            }
        }

        public bool HasEnoughCoins(string username, NpgsqlConnection Conn)
        {
            using var command = new NpgsqlCommand("SELECT coins " +
                                                  "FROM users " +
                                                  "WHERE username = @user ", Conn);
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
                return false;

            }
            catch (Exception e)
            {
                return false;
            }
        }

        public HttpResponse UserStats(string username, NpgsqlConnection Conn)
        {
            using var command = new NpgsqlCommand("Select elo from users where username = @user;", Conn);
            command.Parameters.AddWithValue("@user", username);

            try
            {
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {

                    while (reader.Read())
                    {
                        return CreateHttpResponse(HttpStatusCode.OK, $"{username} has {reader.GetInt32(0)} points");
                    }   
                }

                reader.Close();
                return CreateHttpResponse(HttpStatusCode.Conflict, "User not found");
            }
            catch (Exception e)
            {
                return CreateHttpResponse(HttpStatusCode.Conflict, $"asfafsafasfafs");
                //throw DuplicateNameException();
            }
        }

        public HttpResponse UpdateUserStats(string username, int points, NpgsqlConnection Conn)
        {
            using var command =
                new NpgsqlCommand(
                    "UPDATE users SET elo = (Select elo from users where username = @user) + @pts WHERE username = @user; ",
                    Conn);
            command.Parameters.AddWithValue("@user", username);
            command.Parameters.AddWithValue("@pts", points);

            try
            {
                var worked = command.ExecuteNonQuery();
                return worked == 1 
                    ? CreateHttpResponse(HttpStatusCode.OK, "ELO has been changed")
                    : CreateHttpResponse(HttpStatusCode.Conflict, "Problem occurred while changing ELO!");
            }
            catch (Exception e)
            {
                return CreateHttpResponse(HttpStatusCode.Conflict, "Problem occurred while changing ELO!");
                //throw DuplicateNameException();
            }
        }

        public HttpResponse Scoreboard(NpgsqlConnection Conn)
        {
            using var command = new NpgsqlCommand("Select username, elo from users;", Conn);
            try
            {
                var reader = command.ExecuteReader();
                string stack = "Username: ELO" + Environment.NewLine;
                while (reader.Read())
                {
                    stack += reader.GetString(0) + ": " + reader.GetInt32(1) + Environment.NewLine;
                }

                reader.Close();
                return CreateHttpResponse(HttpStatusCode.OK, stack);

            }
            catch (Exception e)
            {
                return CreateHttpResponse(HttpStatusCode.Conflict, "agdhfdsfgsdfga"); ;
            }
        }

        public void SelectAllUsers(NpgsqlConnection Conn)
        {
            using var command = new NpgsqlCommand("Select * from users;", Conn);
            try
            {
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine($"Reading from user id: {reader.GetInt32(0)}, {reader.GetString(1)}");
                }

                reader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //throw DuplicateNameException();
            }
        }
    }
}
