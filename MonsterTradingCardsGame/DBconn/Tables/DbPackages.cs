﻿using System.Net;
using MonsterTradingCardsGame.ClientServer.Http.Response;


namespace MonsterTradingCardsGame.DbConn.Tables
{
    public class DbPackages : DbHandler
    {

        private readonly DbCards _dbCards = new();

        public bool DeletePackage(string pId)
        {
            try
            {
                return ExecNonQuery(Sql.Commands["DeletePackage"], new string[,]{{"p_id", pId}}) ? true : throw new Exception();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new Exception("Error retrieving package!");
            }
        }

        public string SelectRandomPackageId()
        {
            try
            {
                var resp = ExecQuery(Sql.Commands["SelectRandomPackageId"], 1, null, null, true);
                return resp.Item1
                    ? resp.Item2
                    : throw new Exception("There are no packages in the store");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public string[] GetPackageByRandId()
        {
            try
            {
                var pId = SelectRandomPackageId();
                var resp = ExecQuery(Sql.Commands["GetPackage"], 2, null, new string[,]{{"p_id", pId } }, true);
                return resp.Item1
                    ? resp.Item2.Split(Environment.NewLine)
                    : throw new Exception("Could not get packages!");
            }
            catch (Exception e)
            {
                throw new Exception("Package could not be found due to the following error: " + Environment.NewLine +
                                    e.Message);
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

        public HttpResponse CreatePackage(dynamic cards)
        {
            var packageId = GetRandomString();

            foreach (var card in cards)
            {
                try
                {
                    if (!ExecNonQuery(Sql.Commands["CreatePackage"], new string[,] { { "p_id", packageId }, { "c_id", card.Id.ToString() } }))
                    {
                        throw new Exception("Didn't work");
                    }
                    _dbCards.CreateCard(card.Id.ToString(), card.Name.ToString(), (int)card.Damage);
                }
                catch (Exception e)
                {

                    if (e.Message.Split(':')[0] == "23505")
                        throw new Exception("Package already exists");

                    return CreateHttpResponse(HttpStatusCode.Conflict, "Could not create package!");
                }
            }

            return CreateHttpResponse(HttpStatusCode.Created, "Package created!");
        }

    }
}
