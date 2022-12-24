using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using MonsterTradingCardsGame.ClientServer.Http.Response;
using Npgsql;

namespace MonsterTradingCardsGame.DBconn.Tables
{
    public class DbStack : DbParent
    {

        public HttpResponse ShowStack(string username, NpgsqlConnection conn)
        {
            using var command = new NpgsqlCommand("SELECT cards.name, stack.amount " +
                                                  "FROM users JOIN stack USING(username) JOIN cards ON cards.c_id = stack.card_id " +
                                                  "WHERE stack.username = @user", conn);
            command.Parameters.AddWithValue("@user", username);
            command.Prepare();
            try
            {
                var reader = command.ExecuteReader();
                string stack = "Card Name: amount" + Environment.NewLine;
                while (reader.Read())
                {
                    stack += reader.GetString(0) + ": " + reader.GetInt32(1).ToString() + Environment.NewLine;
                }

                reader.Close();
                return CreateHttpResponse(HttpStatusCode.OK, stack);

            }
            catch (Exception e)
            {
                return CreateHttpResponse(HttpStatusCode.Conflict, "agdhfdsfgsdfga" + e.Message); ;
            }
        }

        public HttpResponse AddCardToStack(string username, string cardId, NpgsqlConnection conn)
        {
            try
            {
                return ExecNonQuery(Sql.Commands["AddCardToStack"], new [,]{ { "user", username }, { "card", cardId } }, conn)
                    ? CreateHttpResponse(HttpStatusCode.Created, $"Card has been added to {username}'s stack!")
                    : CreateHttpResponse(HttpStatusCode.Conflict, $"Problem occurred adding card to {username}'s stack!");
            }
            catch (Exception e)
            {
                if (e.Message.Split(':')[0] == "23505") // unique = card already there
                {
                    try
                    {
                        return ExecNonQuery(Sql.Commands["AddCardToStackDuplicate"], new string[,] { { "user", username }, { "card", cardId } }, conn)
                            ? CreateHttpResponse(HttpStatusCode.Created, $"Card has been added to {username}'s stack!")
                            : CreateHttpResponse(HttpStatusCode.Created, $"Problem occurred adding card to {username}'s stack!");
                    }
                    catch (Exception ee)
                    {
                        return CreateHttpResponse(HttpStatusCode.Conflict, ee.Message.Split(Environment.NewLine)[0]);
                    }
                }
                return CreateHttpResponse(HttpStatusCode.Conflict, e.Message.Split(Environment.NewLine)[0]);
            }
        }
    }
}
