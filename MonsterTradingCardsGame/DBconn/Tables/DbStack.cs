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
    public class DbStack
    {
        public HttpResponse CreateHttpResponse(HttpStatusCode status, string body)
        {
            return new HttpResponse
            {
                Header = new ClientServer.Http.Response.HttpResponseHeader(status, "text/plain", body.Length),
                Body = new HttpResponseBody(body)
            };
        }

        public HttpResponse ShowStack(string username, NpgsqlConnection Conn)
        {
            using var command = new NpgsqlCommand("SELECT cards.name, stack.amount " +
                                                  "FROM users JOIN stack ON users.u_id = stack.user_id JOIN cards ON cards.c_id = stack.card_id " +
                                                  "WHERE stack.user_id = (SELECT u_id FROM users WHERE username = @user)", Conn);
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
                return CreateHttpResponse(HttpStatusCode.Conflict, "agdhfdsfgsdfga"); ;
            }
        }

        public HttpResponse AddCardToStack(string? username, string card_id, NpgsqlConnection Conn)
        {
            // myb change u_id to username
            using var command = new NpgsqlCommand("INSERT INTO public.stack(user_id, card_id, amount) " +
                                                                           "VALUES((SELECT u_id " +
                                                                                   "FROM users " +
                                                                                   "WHERE username = @user), @card, 1)", Conn);
            command.Parameters.AddWithValue("@user", username);
            command.Parameters.AddWithValue("@card", card_id);
            try
            {
                var worked = command.ExecuteNonQuery();
                return worked == 1
                    ? CreateHttpResponse(HttpStatusCode.Created, $"Card has been added to {username}'s stack!")
                    : CreateHttpResponse(HttpStatusCode.Conflict, $"Problem occurred adding card to {username}'s stack!");
            }
            catch (Exception e)
            {
                if (e.Message.Split(':')[0] == "23505") // unique = card already there
                {
                    using var c2 = new NpgsqlCommand("Update stack SET amount = (" +
                                                                "SELECT amount " +
                                                                "FROM stack JOIN users on stack.user_id = users.u_id " +
                                                                "WHERE username = @user " +
                                                                  "AND card_id = @card" +
                                                            ") + 1 " +
                                                            "WHERE user_id = (" +
                                                                "SELECT u_id " +
                                                                "FROM users " +
                                                                "WHERE username = @user" +
                                                            ") " +
                                                              "AND card_id = @card", Conn);
                    c2.Parameters.AddWithValue("@user", username);
                    c2.Parameters.AddWithValue("@card", card_id);
                    try
                    {
                        var worked = c2.ExecuteNonQuery();
                        return worked == 1
                            ? CreateHttpResponse(HttpStatusCode.Created, $"Card has been added to {username}'s stack!")
                            : CreateHttpResponse(HttpStatusCode.Created, $"Problem occurred adding card to {username}'s stack!");
                    }
                    catch (Exception ee)
                    {

                    }
                }
                return CreateHttpResponse(HttpStatusCode.Conflict, e.Message.Split(Environment.NewLine)[0].ToString());
                //throw DuplicateNameException();
            }
        }
    }
}
