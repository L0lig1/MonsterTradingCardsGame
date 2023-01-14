

namespace MonsterTradingCardsGame.DbConn
{
    internal class Query
    {
        public Query(string sqlCommand, string colNr, string[,]? sqlCommandParams)
        {
            SqlCommand = sqlCommand;
            ColNr = colNr;
            SqlCommandParams = sqlCommandParams;
        }

        public string SqlCommand { get; set; }
        public string ColNr { get; set; }
        public string[,]? SqlCommandParams{ get; set; }
    }

    public class DbCommands
    {
        public Dictionary<string, string> Commands = new()
        {
            /* Users */ 
            {
                "RegisterUser", "INSERT INTO public.users(username, pw, coins, elo) " +
                                                  "VALUES(@user, @pw, 20, 100);"
            },
            { 
                "LoginUser", "SELECT username, pw " +
                             "FROM users " +
                             "WHERE username = @user " +
                               "AND pw = @pw;"
            },
            { 
                "GetUserById", "SELECT * " +
                               "FROM users " +
                               "WHERE username = @user"
            },
            { 
                "UpdateUser", "UPDATE public.users " +
                              "SET bio=@bio, image=@img, second_name=@newname " +
                              "WHERE username=@user;"
            },
            { 
                "UseCoins", "UPDATE users " +
                            "SET coins = ((" +
                                "SELECT coins " +
                                "FROM users " +
                                "WHERE username = @user" +
                            ") - @ctp) " +
                            "WHERE username = @user; "
            },
            { 
                "HasEnoughCoins", "SELECT coins " +
                                  "FROM users " +
                                  "WHERE username = @user "
            },
            { 
                "UserStats", "SELECT elo " +
                             "FROM users " +
                             "WHERE username = @user;"
            },
            { 
                "UpdateUserStats", "UPDATE users " +
                                   "SET elo = @pts " +
                                   "WHERE username = @user; "
            },
            { 
                "Scoreboard", "SELECT username, elo " +
                              "FROM users "
            },

            /* Cards */
            { 
                "CreateCard", "INSERT INTO public.cards(name, damage, c_id, cardtype, elemtype) " +
                                                "VALUES(@name, @dmg, @c_id, @ct, @et)"
            },
            {
                "GetCardById", "SELECT * " +
                               "FROM cards " +
                               "WHERE c_id = @card "
            },

            /* Stack */
            { 
                "AddCardToStack", "INSERT INTO public.stack(username, card_id, amount, deck, locked_for_trade) " +
                                                    "VALUES(@user, @card, 1, false, false)"
            },
            { 
                "ShowStack", "SELECT cards.name " +
                             "FROM users JOIN stack USING(username) JOIN cards ON cards.c_id = stack.card_id " +
                             "WHERE stack.username = @user "
            },
            {
                "GetCardFromStackById", "SELECT name " +
                                        "FROM stack JOIN cards ON cards.c_id = stack.card_id " +
                                        "WHERE stack.username = @user " +
                                          "AND cards.c_id = @card " +
                                          "AND stack.locked_for_trade != true"
            },
            {
                "GetCardAllFromStackById", "SELECT username, cardtype, damage, amount " +
                                           "FROM stack JOIN cards ON cards.c_id = stack.card_id " +
                                           "WHERE stack.username = @user " +
                                             "AND cards.c_id = @card "
            },
            { 
                "AddCardToStackDuplicate", "Update stack " +
                                           "SET amount = (" +
                                               "SELECT amount " +
                                               "FROM stack " +
                                               "WHERE stack.username = @user " +
                                                 "AND card_id = @card" +
                                           ") + 1 " +
                                           "WHERE stack.username = @user " +
                                             "AND card_id = @card"
            },
            { 
                "DeleteCardFromStackDuplicate", "Update stack " +
                                                "SET amount = (" +
                                                    "SELECT amount " +
                                                    "FROM stack " +
                                                    "WHERE stack.username = @user " +
                                                      "AND card_id = @card" +
                                                ") - 1 " +
                                                "WHERE stack.username = @user " +
                                                  "AND card_id = @card"
            },
            {
                "DeleteCardFromStack", "DELETE " +
                                       "FROM public.stack " +
                                       "WHERE stack.username = @user " +
                                         "AND card_id = @card"
            },
            /* Deck */ 
            {
                "GetDeckOnlyCardName", "SELECT cards.name "+
                                       "FROM stack JOIN cards ON stack.card_id = cards.c_id " +
                                       "WHERE username = @user " +
                                         "AND deck = true"
            },
            {
                "GetDeck", "SELECT cards.c_id, name, damage, elemtype, cardtype " +
                           "FROM stack JOIN cards ON stack.card_id = cards.c_id " +
                           "WHERE username = @user " +
                             "AND deck = true"
            },
            { 
                "AddCardToDeck", "UPDATE stack " +
                                 "SET deck = true " +
                                 "WHERE username = @user " +
                                  "AND card_id = @card;"},            
            { 
                "RemoveCardFromDeck", "UPDATE stack " +
                                      "SET deck = false " +
                                      "WHERE username = @user " +
                                        "AND card_id = @card;"
            },
            { 
                "LockCardForTrade", "UPDATE stack " +
                                    "SET locked_for_trade = true, deck = false " +
                                    "WHERE username = @user " +
                                      "AND card_id = @card "
            },            
            { 
                "UnlockCardFromTrade", "UPDATE stack " +
                                       "SET locked_for_trade = true " +
                                       "WHERE username = @user " +
                                         "AND card_id = @card "
            },

            /* Packages */ 
            { 
                "CreatePackage", "INSERT INTO packages(p_id, card_id) " +
                                               "VALUES(@p_id, @c_id)"
            },
            { 
                "SelectRandomPackageId", "SELECT p_id " +
                                         "FROM packages " +
                                         "LIMIT 1" },
            { 
                "GetPackage", "SELECT p_id, card_id " +
                              "FROM packages " +
                              "WHERE p_id = @p_id"
            },
            { 
                "DeletePackage", "DELETE " +
                                 "FROM public.packages " +
                                 "WHERE p_id = @p_id"
            },

            /* Trading */
            { 
                "CreateTradingDeal", "INSERT INTO public.trading(trade_id, cardtotrade, cardtype, mindmg, username) " +
                                                         "VALUES(@tid, @ctt, @ct, @dmg, @user);"
            },
            { 
                "GetTradingDeals", "SELECT trade_id, username, cards.name, trading.cardtype, mindmg " +
                                   "FROM public.trading JOIN cards ON trading.cardtotrade = cards.c_id;"
            },
            { 
                "GetTradingDealById", "SELECT username, trading.cardtype, mindmg, amount, cardtotrade " +
                                      "FROM public.trading JOIN cards ON trading.cardtotrade = cards.c_id " +
                                                          "JOIN stack USING(username) " +
                                      "WHERE trade_id = @tid " +
                                      "LIMIT 1 "
            },
            {
                "DeleteTradingDeal", "DELETE " +
                                     "FROM public.trading " +
                                     "WHERE trade_id = @tid;"
            },


            {
                "CreateAllTablesIfNotExists",
                // Cards
                "CREATE TABLE IF NOT EXISTS public.cards " +
                "( " +
                    "name character varying COLLATE pg_catalog.\"default\" NOT NULL, "+
                    "damage integer, "+
                    "cardtype character varying COLLATE pg_catalog.\"default\", "+
                    "elemtype character varying COLLATE pg_catalog.\"default\", "+
                    "c_id character varying COLLATE pg_catalog.\"default\" NOT NULL, "+
                    "CONSTRAINT cards_pkey PRIMARY KEY(c_id) "+
                ") " +
                "TABLESPACE pg_default; " +
                "ALTER TABLE IF EXISTS public.cards " +
                    "OWNER to postgres; " +
                // Packages
                "CREATE TABLE IF NOT EXISTS public.packages " +
                "( " +
                    "p_id character varying COLLATE pg_catalog.\"default\" NOT NULL, "+
                    "card_id character varying COLLATE pg_catalog.\"default\" NOT NULL "+
                ") "+
                "TABLESPACE pg_default; "+
                "ALTER TABLE IF EXISTS public.packages "+
                "OWNER to postgres; "+
                // Stack
                "CREATE TABLE IF NOT EXISTS public.stack "+
                "( "+
                    "username character varying COLLATE pg_catalog.\"default\" NOT NULL, "+
                    "card_id character varying COLLATE pg_catalog.\"default\" NOT NULL, "+
                    "amount integer NOT NULL, "+
                    "deck boolean, "+
                    "locked_for_trade boolean, "+
                    "CONSTRAINT f UNIQUE(username, card_id) "+
                ") " +
                "TABLESPACE pg_default; " +
                "ALTER TABLE IF EXISTS public.stack " +
                    "OWNER to postgres; " +
                // Trading
                "CREATE TABLE IF NOT EXISTS public.trading " +
                "( " +
                    "trade_id character varying COLLATE pg_catalog.\"default\" NOT NULL, "+
                    "cardtotrade character varying COLLATE pg_catalog.\"default\" NOT NULL, "+
                    "cardtype character varying COLLATE pg_catalog.\"default\" NOT NULL, "+
                    "username character varying COLLATE pg_catalog.\"default\" NOT NULL, "+
                    "mindmg integer NOT NULL, "+
                    "CONSTRAINT trading_pkey PRIMARY KEY(trade_id) "+
                ") "+
                "TABLESPACE pg_default; "+
                "ALTER TABLE IF EXISTS public.trading "+
                    "OWNER to postgres; " +
                // Users
                "CREATE TABLE IF NOT EXISTS public.users " +
                "( " +
                    "username character varying COLLATE pg_catalog.\"default\" NOT NULL, "+
                    "pw character varying COLLATE pg_catalog.\"default\" NOT NULL, "+
                    "coins integer, "+
                    "elo integer DEFAULT 100, "+
                    "bio character varying COLLATE pg_catalog.\"default\", "+
                    "image character varying COLLATE pg_catalog.\"default\", "+
                    "second_na"+
                    "me character varying COLLATE pg_catalog.\"default\", "+
                    "CONSTRAINT u_prim PRIMARY KEY(username), "+
                    "CONSTRAINT deez UNIQUE(username) "+
                ") "+
                "TABLESPACE pg_default; "+
                "ALTER TABLE IF EXISTS public.users "+
                "OWNER to postgres; "
            }

        };
    }
}
