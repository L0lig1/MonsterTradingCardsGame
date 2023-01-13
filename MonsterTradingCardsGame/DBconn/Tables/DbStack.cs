using System.Net;
using MonsterTradingCardsGame.CardNamespace;
using MonsterTradingCardsGame.ClientServer.Http.Response;


namespace MonsterTradingCardsGame.DbConn.Tables
{
    public class DbStack : DbHandler
    {

        public HttpResponse ShowStack(string username)
        {
            try
            {
                var resp = ExecQuery(Sql.Commands["ShowStack"], 1, null, new string[,] { { "user", username } }, true);
                return resp.Item1
                    ? CreateHttpResponse(HttpStatusCode.OK, resp.Item2)
                    : throw new Exception("Stack could not be shown");
            }
            catch (Exception e)
            {
                return CreateHttpResponse(HttpStatusCode.Conflict, "Error while showing stack" + e.Message); ;
            }
        }

        public HttpResponse AddCardToStack(string username, string cardId)
        {
            try
            {
                return ExecNonQuery(Sql.Commands["AddCardToStack"], new [,]{ { "user", username }, { "card", cardId } })
                    ? CreateHttpResponse(HttpStatusCode.OK, $"Card has been added to {username}'s stack!")
                    : CreateHttpResponse(HttpStatusCode.Conflict, $"Problem occurred adding card to {username}'s stack!");
            }
            catch (Exception e)
            {
                if (e.Message.Split(':')[0] == "23505") // unique = card already there
                {
                    try
                    {
                        return ExecNonQuery(Sql.Commands["AddCardToStackDuplicate"], new string[,] { { "user", username }, { "card", cardId } })
                            ? CreateHttpResponse(HttpStatusCode.OK, $"Card has been added to {username}'s stack!")
                            : CreateHttpResponse(HttpStatusCode.Conflict, $"Problem occurred adding card to {username}'s stack!");
                    }
                    catch (Exception ee)
                    {
                        return CreateHttpResponse(HttpStatusCode.Conflict, ee.Message.Split(Environment.NewLine)[0]);
                    }
                }
                return CreateHttpResponse(HttpStatusCode.Conflict, e.Message.Split(Environment.NewLine)[0]);
            }
        }

        public bool RemoveCardFromStackById(string id, string user, int amount)
        {
            try
            {
                var resp = amount == 1
                    ? ExecNonQuery(Sql.Commands["DeleteCardFromStack"], new[,] { { "user", user }, { "card", id } })
                    : ExecNonQuery(Sql.Commands["DeleteCardFromStackDuplicate"], new[,] { { "user", user }, { "card", id } });
                return resp ? resp : throw new Exception("Card could not be removed from stack!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception(e.Message);
            }
        }

        public HttpResponse ConfigureDeck(string username, dynamic cards)
        {
            try
            {
                foreach (var card in cards)
                {
                    // check for locked cards + if cards belong to user
                    if(!ExecQuery(Sql.Commands["GetCardFromStackById"], 1, null, new string[,] { { "user", username }, { "card", card.ToString() } }, true).Item1) 
                        throw new Exception("You probably do not own the provided cards or a card provided is locked for trade");
                }
                if (cards.Count != 4) throw new Exception($"{cards.Count} cards provided instead of 4. Make sure to provide 4 cards!{Environment.NewLine}");
                foreach (var card in cards)
                {
                    var bruh = ExecNonQuery(Sql.Commands["AddCardToDeck"], new string[,] { { "user", username }, { "card", card.ToString() } });
                }
                return CreateHttpResponse(HttpStatusCode.OK, $"The provided cards have been added to {username}'s deck!");
            }
            catch (Exception e)
            {
                return CreateHttpResponse(HttpStatusCode.Conflict, $"Problem configuring deck. {e.Message}");
            }
        }

        public HttpResponse GetDeckOnlyCardNames(string username)
        {
            try
            {
                var resp = ExecQuery(Sql.Commands["GetDeckOnlyCardName"], 1, null, new [,] { { "user", username } }, true);
                return resp.Item1 
                    ? CreateHttpResponse(HttpStatusCode.OK, resp.Item2)
                    : throw new Exception("User deck not found!");
            }
            catch (Exception e)
            {
                return CreateHttpResponse(HttpStatusCode.Conflict, "Error retrieving deck: " + e.Message);
            }
        }

        public List<Card> GetDeck(string username)
        {
            try
            {
                var resp = ExecQuery(Sql.Commands["GetDeck"], 5, new []{2}, new[,] { { "user", username } }, true);
                var cards = resp.Item2.Split(Environment.NewLine);
                var listOfCards = new List<Card>();
                foreach (var card in cards)
                {
                    Console.WriteLine(card);
                    var cardInfo = card.Split('@');
                    listOfCards.Add(new Card(cardInfo[0], cardInfo[1], int.Parse(cardInfo[2]), cardInfo[3], cardInfo[4]));
                }
                return listOfCards;
            }
            catch (Exception e)
            {
                throw new Exception($"Error retrieving deck: {e.Message}");
            }
        }

    }
}
