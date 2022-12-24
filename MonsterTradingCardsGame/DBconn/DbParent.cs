using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardsGame.ClientServer.Http.Response;
using Npgsql;

namespace MonsterTradingCardsGame.DBconn
{
    public class DbParent
    {
        public DbCommands Sql = new();
        public static HttpResponse CreateHttpResponse(HttpStatusCode status, string body)
        {
            return new HttpResponse
            {
                Header = new ClientServer.Http.Response.HttpResponseHeader(status, "text/plain", body.Length),
                Body = new HttpResponseBody(body)
            };
        }
        

        public void AddParamWithValue(NpgsqlCommand command, string[,]? values)
        {
            for (var i = 0; i < values.GetLength(0); i++)
            {
                command.Parameters.AddWithValue($"@{values[i,0]}", values[i, 0] == "dmg" ? int.Parse(values[i,1]) : values[i, 1]);
            }
        }
            
        public bool ExecNonQuery(string cmd, string[,]? values, NpgsqlConnection conn)
        {
            using var command = new NpgsqlCommand(cmd, conn);
            AddParamWithValue(command, values);
            try
            {
                var worked = command.ExecuteNonQuery();
                if (worked != 1)
                {
                    throw new Exception("Didn't work lol");
                }
                //return CreateHttpResponse(HttpStatusCode.OK, "Could not create package!");
                return true;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message); //CreateHttpResponse(HttpStatusCode.Conflict, "Could not create package!");
            }
        }

        private static string GetQueryResponse(int queryRespSize, NpgsqlDataReader reader)
        {
            var resp = string.Empty;
            while (reader.Read())
            {
                for (var i = 0; i < queryRespSize; i++)
                {
                    resp += reader.GetString(i) + "@";
                }

                resp = resp.Remove(resp.Length - 1);
                resp += Environment.NewLine;
            }
            return resp;
        }

        public (bool, string) ExecQuery(string cmd, int queryRespSize, string[,]? values, NpgsqlConnection conn, bool recvResp)
        {
            using var command = new NpgsqlCommand(cmd, conn);
            AddParamWithValue(command, values);
            try
            {
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    return (true, recvResp ? GetQueryResponse(queryRespSize, reader) : "");
                }

                reader.Close();
                return (false, "No results found");

            }
            catch (Exception e)
            {
                return (false, "Query Failed! " + e.Message); //CreateHttpResponse(HttpStatusCode.Conflict, "Could not create package!");
            }
        }



    }
}
