﻿using System.Net;
using MonsterTradingCardsGame.ClientServer.Http.Response;
using Npgsql;


namespace MonsterTradingCardsGame.DbConn.Tables
{
    public class DbUsers : DbHandler
    {

        public HttpResponse RegisterUser(string username, string password)
        {
            try
            {
                return ExecNonQuery(Sql.Commands["RegisterUser"], new[,]{ { "user", username }, { "pw", password } } ?? throw new InvalidOperationException()) 
                    ? CreateHttpResponse(HttpStatusCode.Created, "User has been registered") 
                    : CreateHttpResponse(HttpStatusCode.InternalServerError, "Problem occurred adding user!");
            }
            catch (Exception e)
            {
                return e.Message.Split(':')[0] == "23505" // unique = user already there
                    ? CreateHttpResponse(HttpStatusCode.Conflict, "User with that username already exists!") 
                    : CreateHttpResponse(HttpStatusCode.BadRequest, "A Problem occured");
            }
        }

        public HttpResponse LoginUser(string username, string password)
        {
            try
            {
                var response = ExecQuery(Sql.Commands["LoginUser"], 0,null, new[,] { { "user", username }, { "pw", password } }, false);
                return response.Item1
                    ? CreateHttpResponse(HttpStatusCode.OK, $"Login successful!{Environment.NewLine}Welcome to MTCG, {username}! Your token: {username}-mtcgToken")
                    : CreateHttpResponse(HttpStatusCode.Unauthorized, "Login Failed!");
            }
            catch (Exception e)
            {
                return CreateHttpResponse(HttpStatusCode.Unauthorized, "Login Failed! Error: " + e.Message);
            }
        }
        
        public HttpResponse UpdateUser(string username, string name2, string bio, string img)
        {
            try
            {
                return ExecNonQuery(Sql.Commands["UpdateUser"], new[,] { { "newname", name2 }, { "user", username }, { "bio", bio }, { "img", img } })
                    ? CreateHttpResponse(HttpStatusCode.OK, "Update successful!")
                    : CreateHttpResponse(HttpStatusCode.Conflict, "Update failed!");
            }
            catch (Exception e)
            {
                return CreateHttpResponse(HttpStatusCode.Conflict, "Update failed! Error: " + e.Message);
            }
        }

        public bool UseCoins(string username, int coinToPay)
        {
            try
            {
                return ExecNonQuery(Sql.Commands["UseCoins"], new [,] { { "user", username }, { "ctp", coinToPay.ToString() } });
            }
            catch (Exception e)
            {
                throw new Exception("An error occured while trying to use coins! " + e.Message);
            }
        }

        public bool HasEnoughCoins(string username)
        {
            try
            {
                var response = ExecQuery(Sql.Commands["HasEnoughCoins"], 1, new[] { 0 }, new[,] { { "user", username } }, true);
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

        public HttpResponse UserStats(string username)
        {
            try
            {
                var response = ExecQuery(Sql.Commands["UserStats"], 1, new []{0}, new[,] { { "user", username } }, true);
                return response.Item1
                    ? CreateHttpResponse(HttpStatusCode.OK, response.Item2)
                    : CreateHttpResponse(HttpStatusCode.Conflict, "User not found!");
            }
            catch (Exception e)
            {
                return CreateHttpResponse(HttpStatusCode.Conflict, e.Message);
            }
        }

        public HttpResponse UpdateUserStats(string username, int points)
        {
            try
            {
                return ExecNonQuery(Sql.Commands["UpdateUserStats"], new string[,] { { "user", username }, { "pts", points.ToString() } })
                    ? CreateHttpResponse(HttpStatusCode.OK, "ELO has been changed")
                    : CreateHttpResponse(HttpStatusCode.Conflict, "Problem occurred while changing ELO!");
            }
            catch (Exception e)
            {
                return CreateHttpResponse(HttpStatusCode.Conflict, "Problem occurred while changing ELO!" + Environment.NewLine + "Error: " + e.Message);
            }
        }

        public HttpResponse Scoreboard()
        {
            try
            {
                var resp = ExecQuery(Sql.Commands["Scoreboard"], 2, new[] { 1 }, null, true);
                return resp.Item1
                    ? CreateHttpResponse(HttpStatusCode.OK, resp.Item2)
                    : throw new Exception("Could not retrieve scoreboard");

            }
            catch (Exception e)
            {
                return CreateHttpResponse(HttpStatusCode.Conflict, "Scoreboard Error: " + e.Message); ;
            }
        }

        public HttpResponse GetUserById(string userUrl, string userToken)
        {
            try
            {
                var response = ExecQuery(Sql.Commands["GetUserById"], 7, new []{2,3}, new[,] { { "user", userUrl } }, true);
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
