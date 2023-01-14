using System.Net;
using MonsterTradingCardsGame.ClientServer.Http.Response;
using MonsterTradingCardsGame.DbConn.Tables;
using Npgsql;


namespace MonsterTradingCardsGame.DbConn
{
    public class DbHandler 
    {
        protected readonly Db _db = new();
        public DbCommands Sql = new();

        public HttpResponse CreateHttpResponse(HttpStatusCode status, string body)
        {
            return new HttpResponse
            {
                Header = new ClientServer.Http.Response.HttpResponseHeader(status, "text/plain", body.Length),
                Body = new HttpResponseBody(body)
            };
        }

        public void AddParamWithValue(NpgsqlCommand command, string[,] values)
        {
            for (var i = 0; i < values.GetLength(0); i++)
            {
                command.Parameters.AddWithValue(
                    $"@{values[i,0]}", 
                    values[i, 0] == "dmg" || values[i, 0] == "ctp" || values[i, 0] == "pts"
                        ? int.Parse(values[i,1]) 
                        : values[i, 1]);
            }
        }
            
        public virtual bool ExecNonQuery(string cmd, string[,]? values)
        {
            _db.Connect();
            if (_db.Conn == null) throw new Exception("Could not connect to DB");
            using var command = new NpgsqlCommand(cmd, _db.Conn);
            if(values != null) AddParamWithValue(command, values);
            try
            {
                var worked = command.ExecuteNonQuery();
                // CHECK THIS
                //if (worked <= 0)
                //{
                //    throw new Exception("Error: Query not successful");
                //}
                //return CreateHttpResponse(HttpStatusCode.OK, "Could not create package!");
                _db.Disconnect();
                return true;

            }
            catch (Exception e)
            {
                _db.Disconnect();
                throw new Exception(e.Message); // CreateHttpResponse(HttpStatusCode.Conflict, "Could not create package!");
            }
        }

        private static string GetQueryResponse(int queryRespSize, int[]? intIndexes, NpgsqlDataReader reader)
        {
            try
            {
                var resp = string.Empty;
                const string delim = "@";
                while (reader.Read())
                {
                    for (var i = 0; i < queryRespSize; i++)
                    {
                        if (reader.IsDBNull(i))
                        {
                            resp += "null" + delim;
                        }
                        else
                        {
                            resp += intIndexes != null && intIndexes.Contains(i)
                                ? reader.GetInt32(i) + delim
                                : reader.GetString(i) + delim;
                        }
                    }

                    resp = resp.Remove(resp.Length - 1);
                    resp += Environment.NewLine;
                }
                resp = resp.Remove(resp.Length - Environment.NewLine.Length);
                reader.Close();
                return resp;
            }
            catch (Exception e)
            {
                reader.Close();
                throw new Exception(e.Message);
            }
        }

        private static string GetUserByIdResponse(NpgsqlDataReader reader)
        {
            if (!reader.Read()) throw new Exception("Could not get User data");
            var resp = $"Name:     {(reader.IsDBNull(6) ? "" : reader.GetString(6))}{Environment.NewLine}" + 
                             $"Username: {reader.GetString(0)}{Environment.NewLine}" +
                             $"Password: {reader.GetString(1)}{Environment.NewLine}" +
                             $"Coins:    {(reader.IsDBNull(2) ? "" : reader.GetInt32(2))}{Environment.NewLine}" +
                             $"ELO:      {(reader.IsDBNull(3) ? "" : reader.GetInt32(3))}{Environment.NewLine}" +
                             $"Bio:      {(reader.IsDBNull(4) ? "" : reader.GetString(4))}{Environment.NewLine}" +
                             $"Image:    {(reader.IsDBNull(5) ? "" : reader.GetString(5))}{Environment.NewLine}";
            reader.Close();
            return resp;
        }

        public virtual (bool, string) ExecQuery(string cmd, int returnColumnSize, int[]? intIndexes, string[,]? values, bool recvResp)
        {
            _db.Connect();
            if (_db.Conn == null) throw new Exception("Could not connect to DB");
            using var command = new NpgsqlCommand(cmd, _db.Conn);
            if (values != null) AddParamWithValue(command, values);
            try
            {
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    command.Parameters.Clear();
                    var resp = (true, recvResp 
                        ? cmd.Contains("SELECT * FROM users") 
                            ? GetUserByIdResponse(reader) 
                            : GetQueryResponse(returnColumnSize, intIndexes, reader) 
                        : "");
                    _db.Disconnect();
                    return resp;
                }

                command.Parameters.Clear();
                reader.Close();
                _db.Disconnect();
                return (false, "No results found");

            }
            catch (Exception e)
            {
                _db.Disconnect();
                return (false, $"Query Failed! {e.Message}"); //CreateHttpResponse(HttpStatusCode.Conflict, "Could not create package!");
            }
        }

    }
}
