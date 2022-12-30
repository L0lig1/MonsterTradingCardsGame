using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardsGame.CardNamespace;
using MonsterTradingCardsGame.ClientServer.Http.Response;
using Npgsql;

namespace MonsterTradingCardsGame.DBconn.Tables
{
    public class DbStack : DbHandler
    {

        public HttpResponse ShowStack(string username, NpgsqlConnection conn)
        {
            try
            {
                var resp = ExecQuery(Sql.Commands["ShowStack"], 1, null, new string[,] { { "user", username } }, conn, true);
                return resp.Item1
                    ? CreateHttpResponse(HttpStatusCode.OK, resp.Item2)
                    : throw new Exception("Stack could not be shown");
            }
            catch (Exception e)
            {
                return CreateHttpResponse(HttpStatusCode.Conflict, "Error while showing stack" + e.Message); ;
            }
        }

        public HttpResponse AddCardToStack(string username, string cardId, NpgsqlConnection conn)
        {
            try
            {
                return ExecNonQuery(Sql.Commands["AddCardToStack"], new [,]{ { "user", username }, { "card", cardId } }, conn)
                    ? CreateHttpResponse(HttpStatusCode.Created, $"Card has been added to {username}'s stack!")
                    : CreateHttpResponse(HttpStatusCode.Conflict, $"Problem occurred adding card to {username}'s stack!");
            }
            catch (Exception e)
            {
                if (e.Message.Split(':')[0] == "23505") // unique = card already there
                {
                    try
                    {
                        return ExecNonQuery(Sql.Commands["AddCardToStackDuplicate"], new string[,] { { "user", username }, { "card", cardId } }, conn)
                            ? CreateHttpResponse(HttpStatusCode.Created, $"Card has been added to {username}'s stack!")
                            : CreateHttpResponse(HttpStatusCode.Created, $"Problem occurred adding card to {username}'s stack!");
                    }
                    catch (Exception ee)
                    {
                        return CreateHttpResponse(HttpStatusCode.Conflict, ee.Message.Split(Environment.NewLine)[0]);
                    }
                }
                return CreateHttpResponse(HttpStatusCode.Conflict, e.Message.Split(Environment.NewLine)[0]);
            }
        }

        public bool RemoveCardFromStackById(string id, string user, int amount, NpgsqlConnection conn)
        {
            try
            {
                var resp = amount == 1
                    ? ExecNonQuery(Sql.Commands["DeleteCardFromStack"], new[,] { { "user", user }, { "card", id } }, conn)
                    : ExecNonQuery(Sql.Commands["DeleteCardFromStackDuplicate"], new[,] { { "user", user }, { "card", id } }, conn);
                return resp ? resp : throw new Exception("Card could not be removed from stack!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception(e.Message);
            }
        }

        public HttpResponse ConfigureDeck(string username, dynamic cards, NpgsqlConnection conn)
        {
            try
            {
                if (cards.Count != 4) throw new Exception($"{cards.Count} cards provided instead of 4. Make sure to provide 4 cards!{Environment.NewLine}");
                foreach (var card in cards)
                {
                    ExecNonQuery(Sql.Commands["AddCardToDeck"], new string[,] { { "user", username }, { "card", card.ToString() } }, conn);
                }
                return CreateHttpResponse(HttpStatusCode.OK, $"The provided cards have been added to {username}'s deck!");
            }
            catch (Exception e)
            {
                return CreateHttpResponse(HttpStatusCode.Conflict, e.Message + "You do not own the provided cards.");
            }
        }

        public HttpResponse GetDeckOnlyCardNames(string username, NpgsqlConnection conn)
        {
            try
            {
                var resp = ExecQuery(Sql.Commands["GetDeckOnlyCardName"], 1, null, new [,] { { "user", username } }, conn, true);
                return resp.Item1 
                    ? CreateHttpResponse(HttpStatusCode.OK, resp.Item2)
                    : throw new Exception("User deck not found!");
            }
            catch (Exception e)
            {
                return CreateHttpResponse(HttpStatusCode.Conflict, "Error retrieving deck: " + e.Message);
            }
        }

        public List<Card> GetDeck(string username, NpgsqlConnection conn)
        {
            try
            {
                var resp = ExecQuery(Sql.Commands["GetDeck"], 5, new []{2}, new[,] { { "user", username } }, conn, true);
                var cards = resp.Item2.Split(Environment.NewLine);
                var listOfCards = new List<Card>();
                foreach (var card in cards)
                {
                    var cardInfo = card.Split('@');
                    listOfCards.Add(new Card(cardInfo[0], cardInfo[1], int.Parse(cardInfo[2]), cardInfo[3], cardInfo[4]));
                }
                return listOfCards;
            }
            catch (Exception e)
            {
                throw new Exception("Error retrieving deck: " + e.Message);
            }
        }
    }
}
