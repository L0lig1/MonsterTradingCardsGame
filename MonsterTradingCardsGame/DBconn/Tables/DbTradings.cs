using System.Net;
using MonsterTradingCardsGame.ClientServer.Http.Response;


namespace MonsterTradingCardsGame.DbConn.Tables
{
    public class DbTradings : DbHandler
    {
        private readonly DbStack _dbStack = new();
        private static string ParseTradingDeal(string td)
        {
            var resp = "";
            var lines = td.Split(Environment.NewLine);
            string[] split;
            foreach (var line in lines)
            {
                split = line.Split('@');
                resp += $"Trade ID: {split[0]}{Environment.NewLine}" +
                        $"User: {split[1]}, Wants to trade: {split[2]}, type: {split[3]}, min dmg: {split[4]}{Environment.NewLine}";
            }
            return resp;
        }

        public HttpResponse GetTradingDeals()
        {
            try
            {
                var resp = ExecQuery(Sql.Commands["GetTradingDeals"], 5,new []{4}, null, true);
                return resp.Item1
                    ? CreateHttpResponse(HttpStatusCode.OK, ParseTradingDeal(resp.Item2))
                    : throw new Exception("There are no deals currently!");
            }
            catch (Exception e)
            {
                return CreateHttpResponse(HttpStatusCode.Conflict, "Problem while retrieving trading deals: " + e.Message);
            }
        }

        public HttpResponse CreateTradingDeal(string user, dynamic bodyData)
        {
            try
            {
                ExecNonQuery(Sql.Commands["LockCardForTrade"], new string[,] { { "user", user }, { "card", bodyData.CardToTrade } });
                return ExecNonQuery(
                    Sql.Commands["CreateTradingDeal"],
                    new string[,]
                    {
                        { "tid", bodyData.Id }, { "ctt", bodyData.CardToTrade }, { "ct", bodyData.Type },
                        { "dmg", bodyData.MinimumDamage.ToString() }, { "user", user }
                    }
                )
                    ? CreateHttpResponse(HttpStatusCode.Created, "Trading Deal Created successfully")
                    : CreateHttpResponse(HttpStatusCode.Conflict, "Conflict while creating trading deal");
            }
            catch (Exception e)
            {
                return CreateHttpResponse(HttpStatusCode.Conflict, "Trade deal could not be created because: " + e.Message);
            }
        }

        public HttpResponse DeleteTradingDeal(string tId)
        {
            return ExecNonQuery(Sql.Commands["DeleteTradingDeal"], new [,]{ { "tid", tId } })
                ? CreateHttpResponse(HttpStatusCode.Created, "Trading Deal Deleted successfully")
                : CreateHttpResponse(HttpStatusCode.Conflict, "Conflict while deleting trading deal");
        }

        public HttpResponse Trade(string tradeId, string t2CardToTrade, string t2User)
        {
            try
            {
                // get trading deal (if exists)
                var tradee1  = ExecQuery(
                    Sql.Commands["GetTradingDealById"], 
                    5, 
                    new[] {2,3},
                    new[,] { { "tid", tradeId } },
                    true
                );
                
                // get cardToTrade (if exists)
                var tradee2 = ExecQuery(
                    Sql.Commands["GetCardAllFromStackById"], 
                    4, 
                    new []{2,3},
                    new [,]{{"user", t2User}, {"card", t2CardToTrade}},
                    true
                );
                
                if (tradee1.Item1 == false || tradee2.Item1 == false)
                {
                    throw new Exception("Trade offer or provided card does not exist!");
                }

                var trade1Split = tradee1.Item2.Split('@'); // user, ct, dmg, amount, c_id
                var trade2Split = tradee2.Item2.Split('@'); // user, ct, dmg, amount

                // check username
                if (trade1Split[0] == trade2Split[0])
                {
                    throw new Exception( "Cannot trade with yourself!");
                }

                // check card type
                if (trade1Split[1] != trade2Split[1])
                {
                    throw new Exception( $"Card types do not match! Card should be of {trade1Split[1]} type");
                }

                // check min dmg
                if (int.Parse(trade2Split[2]) <= int.Parse(trade1Split[2]))
                {
                    throw new Exception( $"Minimum damage ({int.Parse(trade1Split[2])}) condition not satisfied.{Environment.NewLine}" +
                                         $"Your card has: {trade2Split[2]} damage");
                }

                // if successful
                // amount == 1 ? DELETE delete card from stack : amount - 1
                _dbStack.RemoveCardFromStackById(trade1Split[4], trade1Split[0], int.Parse(trade1Split[3]));
                _dbStack.RemoveCardFromStackById(t2CardToTrade, t2User, int.Parse(trade2Split[3]));

                // add each card to each stack
                _dbStack.AddCardToStack(trade1Split[0], t2CardToTrade);
                _dbStack.AddCardToStack(t2User, trade1Split[4]);

                // delete Trading deal
                DeleteTradingDeal(tradeId);

                return CreateHttpResponse(HttpStatusCode.OK, "Trade successful!");
            }
            catch (Exception e)
            {
                return CreateHttpResponse(HttpStatusCode.Conflict, $"Trade unsuccessful because of the following error: {Environment.NewLine}{e.Message}" );
            }
        }

    }
}
