using MonsterTradingCardsGame.CardNamespace;

namespace MonsterTradingCardsGame.Users
{
    public class User
    {
        public string Name;
        public List<Card> Deck;
        public int Elo;

        public User(string name, List<Card> deck, int elo)
        {
            Elo = elo;
            Name = name;
            Deck = deck;
        }

        public string PrintDeck()
        {
            return Deck.Aggregate(string.Empty, (current, card) => current + (card.PrintCard() + Environment.NewLine));
        }
        
        public Card ReturnRandomCardFromDeck()
        {
            var rand = new Random();
            return Deck[rand.Next(0, Deck.Count)];
        }
    }
}


