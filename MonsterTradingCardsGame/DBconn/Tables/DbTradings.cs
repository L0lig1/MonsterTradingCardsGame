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
    public class DbTradings : DbParent
    {

        public HttpResponse CreateTradingDeal(string user, dynamic bodyData, NpgsqlConnection conn)
        {
            return ExecNonQuery(
                Sql.Commands["CreateTradingDeal"],
                new string[,]
                {
                    { "tid", bodyData.Id }, { "ctt", bodyData.CardToTrade }, { "ct", bodyData.Type },
                    { "dmg", bodyData.MinimumDamage }, { "user", user }
                }, 
                conn
            )
                ? CreateHttpResponse(HttpStatusCode.Created, "Trading Deal Created successfully")
                : CreateHttpResponse(HttpStatusCode.Conflict, "Conflict while creating trading deal");
        }

        public HttpResponse DeleteTradingDeal(string tId, NpgsqlConnection conn)
        {
            return ExecNonQuery(Sql.Commands["DeleteTradingDeal"], new [,]{ { "tid", tId } }, conn)
                ? CreateHttpResponse(HttpStatusCode.Created, "Trading Deal Deleted successfully")
                : CreateHttpResponse(HttpStatusCode.Conflict, "Conflict while deleting trading deal");
        }
    }
}
