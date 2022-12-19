using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace MonsterTradingCardsGame.DBconn.Tables
{
    public interface DbStack
    {

        public string AddCardToStack(string username, string card_id, NpgsqlConnection Conn)
        {
            using var command = new NpgsqlCommand("INSERT INTO public.stack(user_id, card_id, amount) " +
                                                                           "VALUES((SELECT u_id " +
                                                                                   "FROM users " +
                                                                                   "WHERE username = @user), @card, 1)", Conn);
            command.Parameters.AddWithValue("@user", username);
            command.Parameters.AddWithValue("@card", card_id);
            try
            {
                var worked = command.ExecuteNonQuery();
                return worked == 1 ? "User has been added" : "Problem occurred adding user!";
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
                        return worked == 1 ? "User has been added" : "Problem occurred adding user!";
                    }
                    catch (Exception ee)
                    {

                    }
                }
                return e.Message.Split(Environment.NewLine)[0].ToString();
                //throw DuplicateNameException();
            }
        }
    }
}
