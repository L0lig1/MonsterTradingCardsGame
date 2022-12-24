
using MonsterTradingCardsGame.ClientServer.Http.Response;
using Npgsql;
using System.Net;

namespace MonsterTradingCardsGame.DBconn.Tables
{
    public class DbPackages : DbParent
    {
        private readonly DbCards _dbCards = new();

        public bool DeletePackage(string pId, NpgsqlConnection conn)
        {
            try
            {
                return ExecNonQuery(Sql.Commands["DeletePackage"], new string[,]{{"p_id", pId}}, conn) ? true : throw new Exception("FJASLN");
            }
            catch (Exception e)
            {

                throw new Exception("Package could not be found due to the following error: " + Environment.NewLine +
                                    e.Message);
            }
        }

        public string SelectRandomP_id(NpgsqlConnection conn)
        {
            try
            {
                var resp = ExecQuery(Sql.Commands["SelectRandomPackageId"], 1, null, conn, true);
                return resp.Item1
                    ? resp.Item2
                    : throw new Exception("There are no packages in the store");
            }
            catch (Exception e)
            {
                throw new Exception("Package could not be found due to the following error: " + Environment.NewLine + e.Message);
            }
        }

        public string[] GetPackage(NpgsqlConnection conn)
        {
            try
            {
                var pId = SelectRandomP_id(conn);
                var resp = ExecQuery(Sql.Commands["GetPackage"], 2, new string[,]{{"p_id", pId } }, conn, true);
                return resp.Item1
                    ? resp.Item2.Split(Environment.NewLine)
                    : throw new Exception("Could not get packages!");
            }
            catch (Exception e)
            {

                throw new Exception("Package could not be found due to the following error: " + Environment.NewLine +
                                    e.Message);
                //throw DuplicateNameException();
            }
        }

        public string GetRandomString()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();

            for (var i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new string(stringChars);
        }

        public HttpResponse CreatePackage(dynamic cards, NpgsqlConnection Conn)
        {
            var packageId = GetRandomString();

            foreach (var card in cards)
            {
                try
                {
                    if (!ExecNonQuery(Sql.Commands["CreatePackage"], new string[,] { { "p_id", packageId }, { "c_id", card.Id.ToString() } }, Conn))
                    {
                        throw new Exception("Didn't work lol");
                    }
                    _dbCards.CreateCard(card.Id.ToString(), card.Name.ToString(), (int)card.Damage, Conn);
                }
                catch (Exception e)
                {

                    if (e.Message.Split(':')[0] == "23505")
                        throw new Exception("Package already exists");

                    return CreateHttpResponse(HttpStatusCode.Conflict, "Could not create package!");
                }
            }

            return CreateHttpResponse(HttpStatusCode.Created, $"Package created!");
        }
    }
}
