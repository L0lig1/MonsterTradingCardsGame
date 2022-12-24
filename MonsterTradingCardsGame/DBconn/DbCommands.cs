using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardsGame.DBconn
{
    public class DbCommands
    {
        public Dictionary<string, string> Commands = new()
        {
            /* Users */ 
            { "RegisterUser", "INSERT INTO public.users(username, pw, coins, elo) " +
                                                "VALUES(@user, @pw, 20, 100);" },
            { "LoginUser", "SELECT username, pw " +
                           "FROM users " +
                           "WHERE username = @user " +
                             "AND pw = @pw;" },
            { "UpdateUser", "UPDATE public.users " +
                            "SET bio=@bio, image=@img, second_name=@newname " +
                            "WHERE username=@user;" },
            { "UseCoins", "UPDATE users " +
                          "SET coins = ((" +
                            "SELECT coins " +
                             "FROM users " +
                             "WHERE username = @user" +
                          ") - @ctp) " +
                          "WHERE username = @user; " },
            { "HasEnoughCoins", "SELECT coins " +
                                "FROM users " +
                                "WHERE username = @user " },
            { "UserStats", "SELECT elo " +
                           "FROM users " +
                           "WHERE username = @user;" },
            { "UpdateUserStats", "UPDATE users " +
                                 "SET elo = (" +
                                    "SELECT elo " +
                                    "FROM users " +
                                    "WHERE username = @user" +
                                 ") + @pts " +
                                 "WHERE username = @user; " },
            { "Scoreboard", "SELECT username, elo " +
                            "FROM users;" },

            /* Cards */
            { "CreateCard", "INSERT INTO public.cards(name, damage, c_id) " +
                                              "VALUES(@name, @dmg, @c_id)" },

            /* Stack */
            { "ShowStack", "SELECT cards.name, stack.amount " +
                           "FROM users JOIN stack USING(username) JOIN cards ON cards.c_id = stack.card_id " +
                           "WHERE stack.username = @user" },
            { "AddToStack", "INSERT INTO public.stack(username, card_id, amount) " +
                                              "VALUES(@user, @card, 1)" },
            { "AddCardToStackDuplicate", "Update stack SET amount = (" +
                                             "SELECT amount " +
                                             "FROM stack JOIN users on stack.username = users.username " +
                                             "WHERE username = @user " +
                                               "AND card_id = @card" +
                                         ") + 1 " +
                                         "WHERE user_id = @user" +
                                           "AND card_id = @card" },

            /* Packages */ 
            { "CreatePackage", "INSERT INTO packages(p_id, card_id) " +
                                             "VALUES(@p_id, @c_id)" },
            { "SelectRandomPackageId", "SELECT p_id " +
                                       "FROM packages " +
                                       "LIMIT 1" },
            { "GetPackage", "SELECT p_id, card_id " +
                            "FROM packages " +
                            "WHERE p_id = @p_id" },
            { "DeletePackage", "DELETE " +
                               "FROM public.packages " +
                               "WHERE p_id = @p_id" },

            /* Trading */
            { "CreateTradingDeal", "INSERT INTO public.trading(trade_id, cardtotrade, cardtype, mindmg, username) " +
                                                       "VALUES(@tid, @ctt, @ct, @dmg, @user);" },
            { "DeleteTradingDeal", "DELETE FROM public.trading WHERE trade_id = @tid;" },

        };
    }
}
