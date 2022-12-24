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
    public class DbTradings : DbHandler
    {
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

        public HttpResponse CheckTradingDeal(NpgsqlConnection conn)
        {
            try
            {
                var resp = ExecQuery(Sql.Commands["CheckTradingDeal"], 5, null, conn, true);
                return resp.Item1
                    ? CreateHttpResponse(HttpStatusCode.OK, ParseTradingDeal(resp.Item2))
                    : throw new Exception("There are no deals currently!");
            }
            catch (Exception e)
            {
                return CreateHttpResponse(HttpStatusCode.Conflict, "Problem while retrieving trading deals: " + e.Message);
            }
        }

        public HttpResponse CreateTradingDeal(string user, dynamic bodyData, NpgsqlConnection conn)
        {
            try
            {
                return ExecNonQuery(
                    Sql.Commands["CreateTradingDeal"],
                    new string[,]
                    {
                        { "tid", bodyData.Id }, { "ctt", bodyData.CardToTrade }, { "ct", bodyData.Type },
                        { "dmg", bodyData.MinimumDamage.ToString() }, { "user", user }
                    }, 
                    conn
                )
                    ? CreateHttpResponse(HttpStatusCode.Created, "Trading Deal Created successfully")
                    : CreateHttpResponse(HttpStatusCode.Conflict, "Conflict while creating trading deal");
            }
            catch (Exception e)
            {
                return CreateHttpResponse(HttpStatusCode.Conflict, "Trade deal could not be created because: " + e.Message);
                //throw;
            }
        }

        public HttpResponse DeleteTradingDeal(string tId, NpgsqlConnection conn)
        {
            return ExecNonQuery(Sql.Commands["DeleteTradingDeal"], new [,]{ { "tid", tId } }, conn)
                ? CreateHttpResponse(HttpStatusCode.Created, "Trading Deal Deleted successfully")
                : CreateHttpResponse(HttpStatusCode.Conflict, "Conflict while deleting trading deal");
        }
        
    }
}
