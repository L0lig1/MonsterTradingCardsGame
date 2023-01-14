using System.Net;
using MonsterTradingCardsGame.ClientServer.Http.Response;


namespace MonsterTradingCardsGame.DbConn.Tables
{
    public class DbUsers
    {

        public virtual HttpResponse RegisterUser(string username, string password, DbHandler dbHandler)
        {
            try
            {
                var br = dbHandler.ExecNonQuery(dbHandler.Sql.Commands["RegisterUser"],
                    new[,] { { "user", username }, { "pw", password } });


                return br
                    ? dbHandler.CreateHttpResponse(HttpStatusCode.Created, "User has been registered") 
                    : dbHandler.CreateHttpResponse(HttpStatusCode.InternalServerError, "Problem occurred adding user!");
            }
            catch (Exception e)
            {
                return e.Message.Split(':')[0] == "23505" // unique = user already there
                    ? dbHandler.CreateHttpResponse(HttpStatusCode.Conflict, "User with that username already exists!") 
                    : dbHandler.CreateHttpResponse(HttpStatusCode.BadRequest, "A Problem occured");
            }
        }

        public HttpResponse LoginUser(string username, string password, DbHandler dbHandler)
        {
            try
            {
                var response = dbHandler.ExecQuery(dbHandler.Sql.Commands["LoginUser"], 0,null, new[,] { { "user", username }, { "pw", password } }, false);
                return response.Item1
                    ? dbHandler.CreateHttpResponse(HttpStatusCode.OK, $"Login successful!{Environment.NewLine}Welcome to MTCG, {username}! Your token: {username}-mtcgToken")
                    : dbHandler.CreateHttpResponse(HttpStatusCode.Unauthorized, "Login Failed!");
            }
            catch (Exception e)
            {
                return dbHandler.CreateHttpResponse(HttpStatusCode.Unauthorized, "Login Failed! Error: " + e.Message);
            }
        }
        
        public HttpResponse UpdateUser(string username, string name2, string bio, string img, DbHandler dbHandler)
        {
            try
            {
                return dbHandler.ExecNonQuery(dbHandler.Sql.Commands["UpdateUser"], new[,] { { "newname", name2 }, { "user", username }, { "bio", bio }, { "img", img } })
                    ? dbHandler.CreateHttpResponse(HttpStatusCode.OK, "Update successful!")
                    : dbHandler.CreateHttpResponse(HttpStatusCode.Conflict, "Update failed!");
            }
            catch (Exception e)
            {
                return dbHandler.CreateHttpResponse(HttpStatusCode.Conflict, "Update failed! Error: " + e.Message);
            }
        }

        public bool UseCoins(string username, int coinToPay, DbHandler dbHandler)
        {
            try
            {
                return dbHandler.ExecNonQuery(dbHandler.Sql.Commands["UseCoins"], new [,] { { "user", username }, { "ctp", coinToPay.ToString() } });
            }
            catch (Exception e)
            {
                throw new Exception("An error occured while trying to use coins! " + e.Message);
            }
        }

        public bool HasEnoughCoins(string username, DbHandler dbHandler)
        {
            try
            {
                var response = dbHandler.ExecQuery(dbHandler.Sql.Commands["HasEnoughCoins"], 1, new[] { 0 }, new[,] { { "user", username } }, true);
                if (response.Item1 && int.Parse(response.Item2) >= 5)
                {
                    return response.Item1;
                }
                throw new Exception($"Not enough coins! You have {response.Item2}, but should at least have 5");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new Exception(e.Message);
            }
        }

        public HttpResponse UserStats(string username, DbHandler dbHandler)
        {
            try
            {
                var response = dbHandler.ExecQuery(dbHandler.Sql.Commands["UserStats"], 1, new []{0}, new[,] { { "user", username } }, true);
                return response.Item1
                    ? dbHandler.CreateHttpResponse(HttpStatusCode.OK, response.Item2)
                    : dbHandler.CreateHttpResponse(HttpStatusCode.Conflict, "User not found!");
            }
            catch (Exception e)
            {
                return dbHandler.CreateHttpResponse(HttpStatusCode.Conflict, e.Message);
            }
        }

        public HttpResponse UpdateUserStats(string username, int points, DbHandler dbHandler)
        {
            try
            {
                return dbHandler.ExecNonQuery(dbHandler.Sql.Commands["UpdateUserStats"], new string[,] { { "user", username }, { "pts", points.ToString() } })
                    ? dbHandler.CreateHttpResponse(HttpStatusCode.OK, "ELO has been changed")
                    : dbHandler.CreateHttpResponse(HttpStatusCode.Conflict, "Problem occurred while changing ELO!");
            }
            catch (Exception e)
            {
                return dbHandler.CreateHttpResponse(HttpStatusCode.Conflict, "Problem occurred while changing ELO!" + Environment.NewLine + "Error: " + e.Message);
            }
        }

        public HttpResponse Scoreboard(DbHandler dbHandler)
        {
            try
            {
                var resp = dbHandler.ExecQuery(dbHandler.Sql.Commands["Scoreboard"], 2, new[] { 1 }, null, true);
                return resp.Item1
                    ? dbHandler.CreateHttpResponse(HttpStatusCode.OK, resp.Item2)
                    : throw new Exception("Could not retrieve scoreboard");

            }
            catch (Exception e)
            {
                return dbHandler.CreateHttpResponse(HttpStatusCode.Conflict, "Scoreboard Error: " + e.Message); ;
            }
        }

        public HttpResponse GetUserById(string userUrl, string userToken, DbHandler dbHandler)
        {
            try
            {
                var response = dbHandler.ExecQuery(dbHandler.Sql.Commands["GetUserById"], 7, new []{2,3}, new[,] { { "user", userUrl } }, true);
                return response.Item1
                    ? dbHandler.CreateHttpResponse(HttpStatusCode.OK, $"{userUrl}'s Profile: {Environment.NewLine}{response.Item2}")
                    : dbHandler.CreateHttpResponse(HttpStatusCode.Conflict, "User not found!");
            }
            catch (Exception e)
            {
                return dbHandler.CreateHttpResponse(HttpStatusCode.Unauthorized, "Error: " + e.Message);
            }
        }

    }
}
